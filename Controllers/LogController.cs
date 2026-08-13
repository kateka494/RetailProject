using Microsoft.AspNetCore.Mvc;
using RetailProject.Services;

namespace RetailProject.Controllers
{
    public class LogController : Controller
    {
        private readonly FileShareService _fileService;

        public LogController(FileShareService fileService)
        {
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            
            var logContent = await _fileService.ReadLogFileAsync();
            ViewBag.LogContent = logContent;
            return View();
        }
    }
}