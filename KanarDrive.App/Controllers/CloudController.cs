using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using KanarDrive.App.Models.Cloud;
using KanarDrive.Common.Entities.Identity;
using KanarDrive.Common.FilesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KanarDrive.App.Controllers
{
    [Authorize]
    public class CloudController : Controller
    {
        private readonly FilesService _filesService;
        private readonly ILogger<CloudController> _logger;
        private readonly UserManager<User> _userManager;

        public CloudController(ILogger<CloudController> logger, FilesService filesService,
            UserManager<User> userManager)
        {
            _logger = logger;
            _filesService = filesService;
            _userManager = userManager;
        }

        private async Task<User> GetUser()
        {
            return await _userManager.FindByNameAsync(User.Identity.Name);
        }

        private string RemoveCloudFromPath(string input)
        {
            return Decode(string.Join("/", Decode(input).Split('/').Skip(2).ToArray()));
        }

        private string Decode(string input)
        {
            return HttpUtility.UrlDecode(input)?.Replace('\\', '/');
        }

        [HttpGet("/cloud")]
        public async Task<IActionResult> Index()
        {
            var user = await GetUser();
            var info = _filesService.GetFolder(user, "");
            return View(new CloudModel(info)
            {
                Files = _filesService.EnumerateFiles(info),
                Directories = _filesService.EnumerateDirectories(info),
                CurrentSpace = _filesService.GetCurrentSpace(user),
                AvailableSpaceToUser = user.AvailableCloudSpace,
                DisplayReadyPath = ""
            });
        }

        [HttpGet("/cloud/{path}")]
        public async Task<IActionResult> Index(string path)
        {
            var user = await GetUser();
            var info = _filesService.GetFolder(user, Decode(path));
            return View(new CloudModel(info)
            {
                Files = _filesService.EnumerateFiles(info),
                Directories = _filesService.EnumerateDirectories(info),
                CurrentSpace = _filesService.GetCurrentSpace(user),
                AvailableSpaceToUser = user.AvailableCloudSpace,
                DisplayReadyPath = info.Path != null ? $"{Decode(info.Path)}/{info.Name}" : info.Name
            });
        }

        [HttpPost("/cloud/create-directory")]
        public async Task<IActionResult> CreateDirectory(string path, string name)
        {
            _filesService.CreateFolder(await GetUser(), RemoveCloudFromPath(path), name);
            return Ok();
        }

        [HttpDelete("/cloud/delete-file")]
        public async Task<IActionResult> DeleteFile(string id)
        {
            if (_filesService.RemoveFile(await GetUser(), id))
                return Ok();
            return BadRequest();
        }

        [HttpDelete("/cloud/delete-folder")]
        public async Task<IActionResult> DeleteFolder(string id)
        {
            if (_filesService.RemoveFolder(await GetUser(), id))
                return Ok();
            return BadRequest();
        }

        [HttpPut("/cloud/rename")]
        public async Task<IActionResult> Rename(string id, string newName)
        {
            if (_filesService.Rename(await GetUser(), id, newName))
                return Ok();
            return BadRequest();
        }

        [HttpPost("/cloud/upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 50_000_000_000)] //MAX 50 GB Upload
        [RequestSizeLimit(50_000_000_000)]
        public async Task<JsonResult> Upload()
        {
            var files = Request.Form.Files;
            if (files.Count() != 1)
            {
                return Json(new
                {
                    error = "Only single file upload is supported!!!"
                });
            }
            var file = files.Single();
            var path = Request.Form["path"];
            try
            {
                _filesService.AddFile(RemoveCloudFromPath(path), file.FileName, file.ContentType,
                                    file.OpenReadStream(), await GetUser());
            }
            catch (IOException ex)
            {
                return Json(new
                {
                    error = "Brak wystarczającej ilości miejsca na dyski aby przesłać ten plik!"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new 
                {
                    error = "Plik z identyczną nazwą jest już obecny w podanej lokacji!"
                });
            }
            return Json(new { });
        }

        [HttpGet("/cloud/download")]
        [AllowAnonymous]
        public async Task<IActionResult> Download(string id)
        {
            var file = _filesService.GetFile(id);

            if (file == null) return NotFound();
            
            if (file.Metadata["public"].AsBoolean)
            {
                return File(file.OpenRead(), file.Metadata["content_type"], file.Filename);
            }

            if (!User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
            
            if (file.Metadata["owner"] == (await GetUser()).UserName.ToLower())
            {
                return File(file.OpenRead(), file.Metadata["content_type"], file.Filename);
            }
            
            return NotFound();
        }

        [HttpPut("/cloud/move")]
        public async Task<IActionResult> MoveFile(string id, string newPath)
        {
            var path = RemoveCloudFromPath(newPath);
            if (_filesService.Move(await GetUser(), id, path))
                return Ok();
            return BadRequest();
        }

        [HttpPut("/cloud/share")]
        public async Task<IActionResult> ShareFile(string id) //Or un-share...
        {
            if (_filesService.ChangeWhetherShared(await GetUser(), id))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}