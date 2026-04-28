using Microsoft.AspNetCore.Mvc;

namespace EBYS.Controllers
{
    public class UiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
