using Microsoft.AspNetCore.Mvc;

namespace AccountService.API.Controllers
{
    public class AdminContoller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
