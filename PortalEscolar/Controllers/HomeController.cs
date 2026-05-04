using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalEscolar.Models;
using PortalEscolar.Models.ViewModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PortalEscolar.Data;
using PortalEscolar.Services;
using Microsoft.EntityFrameworkCore;

namespace PortalEscolar.Controllers
{
    public class HomeController : Controller
    {
        private PortalescolarContext _context;

        public HomeController(PortalescolarContext Context)
        {
            _context = Context;
        }
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.Include(u=> u.Professore).Include(u=>u.Aluno).ToListAsync();
            return View(usuarios);
        }
        [Authorize]
        public IActionResult Privacy()
        {
            ViewData["username"] = User.Identity.Name;
            return View();
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
