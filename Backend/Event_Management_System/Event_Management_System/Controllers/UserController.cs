using Microsoft.AspNetCore.Mvc;

namespace Event_Management_System.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
