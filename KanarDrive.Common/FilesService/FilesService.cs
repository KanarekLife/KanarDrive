using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KanarDrive.Common.Entities.Cloud;
using KanarDrive.Common.Entities.Identity;
using LiteDB;

namespace KanarDrive.Common.FilesService
{
    public class FilesService
    {
        private readonly ILiteCollection<BsonDocument> _files;
        private readonly ILiteStorage<string> _fileStorage;
        private readonly ILiteCollection<UserDirectory> _folders;

        public FilesService(ILiteDatabase database)
        {
            _fileStorage = database.FileStorage;
            _folders = database.GetCollection<UserDirectory>("directories");
            _files = database.GetCollection("_files");
        }

        private static string GetUserName(User user)
        {
            return user.UserName.ToLower();
        }

        public IEnumerable<UserDirectory> EnumerateDirectories(UserDirectory directory)
        {
            switch (directory.Path)
            {
                case null when directory.Name == ".":
                    return _folders.Find(x => x.Owner == directory.Owner && x.Path == null);
                case null:
                    return _folders.Find(x => x.Owner == directory.Owner && x.Path == directory.Name);
                default:
                    return _folders.Find(x =>
                        x.Owner == directory.Owner && x.Path == directory.Path + '/' + directory.Name);
            }
        }

        public IEnumerable<LiteFileInfo<string>> EnumerateFiles(UserDirectory directory)
        {
            switch (directory.Path)
            {
                case null when directory.Name == ".":
                    return _fileStorage.Find(x => x.Metadata["owner"] == directory.Owner && x.Metadata["path"] == "");
                case null:
                    return _fileStorage.Find(x =>
                        x.Metadata["owner"] == directory.Owner && x.Metadata["path"] == directory.Name);
                default:
                    return _fileStorage.Find(x =>
                        x.Metadata["owner"] == directory.Owner &&
                        x.Metadata["path"] == directory.Path + '/' + directory.Name);
            }
        }

        public UserDirectory GetFolder(User user, string path)
        {
            var name = path.Split('/').Last();
            if (name == "") name = ".";
            var correctPath = string.Join("/", path.Split('/').SkipLast(1).ToArray());
            var folder = _folders.Find(x =>
                    x.Owner == GetUserName(user) && x.Path == (correctPath == "" ? null : correctPath) &&
                    x.Name == name)
                .ToArray();
            if (!folder.Any())
            {
                if (correctPath == "" && name == ".")
                {
                    var newFolder = new UserDirectory
                    {
                        Id = Guid.NewGuid(),
                        Name = ".",
                        Owner = GetUserName(user),
                        Path = correctPath
                    };
                    var id = _folders.Insert(newFolder);
                    return _folders.FindById(id);
                }
            }
            else
            {
                return folder.First();
            }

            return null;
        }

        public void CreateFolder(User user, string path, string name)
        {
            var folder = _folders.FindOne(x =>
                x.Owner == GetUserName(user) && x.Path == (path == "" ? null : path) && x.Name == name);
            if (folder == null)
                _folders.Insert(new UserDirectory
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Path = path,
                    Owner = GetUserName(user)
                });
        }

        public void AddFile(string path, string filename, string contentType, Stream content, User user)
        {
            var folder = GetFolder(user, path);
            if (folder == null) throw new InvalidOperationException("File with the same filename is already present!");
            if (GetAvailableSpace(user) < content.Length)
                throw new IOException("Not enough storage space available to upload this file!");
            if (EnumerateFiles(folder).All(x => x.Filename != filename))
                _fileStorage.Upload(Guid.NewGuid().ToString(), filename, content, new BsonDocument
                {
                    {"owner", GetUserName(user)},
                    {"path", path},
                    {"content_type", contentType},
                    {"public", false}
                });
        }

        public LiteFileInfo<string> GetFile(string id)
        {
            return _fileStorage.FindById(id);
        }

        public bool RemoveFile(User user, string id)
        {
            var file = _fileStorage.FindById(id);
            if (file != null && file.Metadata["owner"] == GetUserName(user))
                return _fileStorage.Delete(id);
            return false;
        }

        public bool RemoveFolder(User user, string id)
        {
            var guid = Guid.Parse(id);
            var folder = _folders.FindById(guid);
            if (_files.Find(x =>
                    folder.Path == null
                        ? x["metadata"]["path"] == folder.Name
                        : x["metadata"]["path"] == folder.Path + '/' + folder.Name)
                .Any() || folder.Owner != GetUserName(user))
            {
                return false;
            }

            _folders.Delete(guid);
            return true;
        }

        public bool Rename(User user, string id, string newName)
        {
            var file = _files.FindById(id);
            if (file != null && file["metadata"]["owner"] == GetUserName(user))
            {
                file["filename"] = newName;
                _files.Update(file);
                return true;
            }

            return false;
        }

        public bool Move(User user, string id, string newPath)
        {
            var file = _files.FindById(id);
            if (file != null && file["metadata"]["owner"] == GetUserName(user))
            {
                file["metadata"]["path"] = newPath;
                _files.Update(file);
                return true;
            }
            return false;
        }

        public bool ChangeWhetherShared(User user, string id)
        {
            var file = _files.FindById(id);
            if (file != null && file["metadata"]["owner"] == GetUserName(user))
            {
                file["metadata"]["public"] = !file["metadata"]["public"];
                _files.Update(file);
                return true;
            }
            return false;
        }

        public long GetCurrentSpace(User user)
        {
            return _fileStorage.Find(x => x.Metadata["owner"] == GetUserName(user)).Select(x => x.Length).Sum();
        }

        private long GetAvailableSpace(User user)
        {
            var max = Convert.ToInt64(user.AvailableCloudSpace) * 1_000_000_000;
            var used = GetCurrentSpace(user);
            return max - used;
        }
    }
}