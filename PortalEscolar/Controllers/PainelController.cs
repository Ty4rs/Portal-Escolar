using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalEscolar.Models;
using PortalEscolar.Services;
using PortalEscolar.Views.ViewModel;


namespace PortalEscolar.Controllers
{
    public class PainelController : Controller
    {
        readonly IMateriaService _materiaService;
        public PainelController(IMateriaService materiaService) { 
            _materiaService = materiaService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult CriarMateria()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CadastrarMateria(string Nome, int Cargahoraria)
        {
            if (!await _materiaService.VerificarMateria(Nome, Cargahoraria))
            {
                await _materiaService.CadastrarMateria(Nome, Cargahoraria);
                return RedirectToAction("index", "painel");
            }
           
            return RedirectToAction("criarMateria", "painel");
        }

        public async Task<IActionResult> CriarMateriaPeriodo(int Cargahoraria)
        {
            List<Materia> materiasc = await _materiaService.ListarMaterias();

            MateriasViewModel materias2 = new MateriasViewModel()
            {
                materias = new SelectList(
                    materiasc.Select(m => new
                    {
                        Text = m.Nome,
                        Value = m.IdMateria
                    }), "Value", "Text"),
                
            };
            return View(materias2);
        }

    }
}
