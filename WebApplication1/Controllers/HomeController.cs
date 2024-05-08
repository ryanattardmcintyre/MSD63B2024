using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, RedisRepository rr)
        {
            rr.IncrementCounter();
            _logger = logger;
        }

        public IActionResult Index([FromServices] LogsRepository lr)
        {
            try
            {
                lr.WriteLogEntry("HomeController-Log", "Just entered the Index action", Google.Cloud.Logging.Type.LogSeverity.Info);

                throw new Exception("Error generated on purpose so we test the log entries");
            }
            catch (Exception ex) {
                lr.WriteLogEntry("HomeController-Log", ex.Message, Google.Cloud.Logging.Type.LogSeverity.Error);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        public IActionResult MembersLanding()
        { return View(); }

        [Authorize]
        public async Task<IActionResult> Logout() //Task represents an asynchronous thread/process
        {
            await HttpContext.SignOutAsync(); //will erase the cookie holding the session alive
            return RedirectToAction("Index"); //the outcome: loads the home page for the user who has just been logged out

        }
    }
}