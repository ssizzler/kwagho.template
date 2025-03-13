using Microsoft.AspNetCore.Mvc;

namespace kwangho.mvc.Controllers
{
    public class UserController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
