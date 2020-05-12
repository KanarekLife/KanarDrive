using System;
using System.Collections.Generic;
using KanarDrive.Common.Entities.Cloud;
using LiteDB;

namespace KanarDrive.App.Models.Cloud
{
    public class CloudModel : UserDirectory
    {
        public long AvailableSpaceToUser;
        public long CurrentSpace;
        public IEnumerable<UserDirectory> Directories;
        public string DisplayReadyPath;
        public IEnumerable<LiteFileInfo<string>> Files;

        public CloudModel(UserDirectory userDirectory)
        {
            Id = Guid.Parse(userDirectory.Id.ToString());
            Name = userDirectory.Name;
            Owner = userDirectory.Owner;
            Path = userDirectory.Path;
        }
    }
}