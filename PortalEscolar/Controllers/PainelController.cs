using Microsoft.AspNetCore.Mvc;

namespace PortalEscolar.Controllers
{
    public class PainelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
