using DocumentReader.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Diagnostics;

namespace DocumentReader.MVC.Controllers
{
    [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
    public class HomeController : Controller
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;
        private readonly string _sharepointDomain;
        private readonly string _folderRoot = "C:/Users/laurieat/OneDrive - Microsoft/Documents";
        private readonly string _relativePath = "/sites/DemoSite";

        public HomeController(IConfiguration configuration, GraphServiceClient graphServiceClient, ITokenAcquisition tokenAcquisition)
        {
            _configuration = configuration;
            _sharepointDomain = _configuration["SharePoint:Domain"];
            _graphServiceClient = graphServiceClient;
            _tokenAcquisition=tokenAcquisition;
        }

        public async Task<IActionResult> Index()
        {
            // Trigger to authenticate to the MS Graph
            var me = await _graphServiceClient.Me.Request().GetAsync();

            return View(new DocumentInfo($"Demo/1/2/3", "Test.docx"));
        }

        [HttpPost]
        public async Task<IActionResult> Index(string folderPath, string fileName)
        {
            var filePath = $"{_folderRoot}/{folderPath}/{fileName}";

            if (!System.IO.File.Exists(filePath))
            {
                ViewBag.ErrorMessage = "File not found";
                return View();
            }
            //ViewBag.ErrorMessage = String.Empty;
            var ms = new MemoryStream();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.CopyTo(ms);
            }
            ms.Seek(0, SeekOrigin.Begin);

            // Damien Edwards examples can be found here:
            // https://damienbod.com/2020/11/20/using-microsoft-graph-api-in-asp-net-core/

            var saveLocation = $"TemporaryEditing/{getSharePointFileName(folderPath, fileName)}";

            var site = await _graphServiceClient
                .Sites[_sharepointDomain]
                .SiteWithPath(_relativePath)
                .Request()
                .GetAsync().ConfigureAwait(false);

            var drive = await _graphServiceClient
                .Sites[site.Id]
                .Drive
                .Request()
                .GetAsync().ConfigureAwait(false);

            // Upload the file to SharePoint
            try
            {
                await _graphServiceClient
                    .Sites[site.Id]
                    .Drives[drive.Id]
                    .Root
                    .ItemWithPath(saveLocation)
                    .Content
                    .Request()
                    .PutAsync<DriveItem>(ms);

                // This is required if Checkin/Checkout is enabled on the document library
                await _graphServiceClient
                    .Sites[site.Id]
                    .Drives[drive.Id]
                    .Root
                    .ItemWithPath(saveLocation)
                    .Checkin()
                    .Request()
                    .PostAsync();

                return Redirect($"ms-word:ofe|u|https://{_sharepointDomain}{_relativePath}/Shared%20Documents/{saveLocation}");
            }
            catch (ServiceException graphException)
            {
                // If the file is already checked out, it will fail when trying to copy it into SharePoint
                ViewBag.ErrorMessage = graphException.Error.Message;
                return View();
            }

            // Open in the browser
            //return Redirect(savedFile.WebUrl);
        }

        private string getSharePointFileName(string folderPath, string fileName)
        {
            // Remove drive letter
            // Use folder names in file name using dashes
            // Indicate end of folders with an underscore
            folderPath = folderPath.Substring(folderPath.IndexOf("/") + 1);
            return $"{folderPath.Replace("/", "-")}_{fileName}";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}