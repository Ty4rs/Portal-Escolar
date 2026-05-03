using Microsoft.AspNetCore.Mvc;

namespace PortalEscolar.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
}
