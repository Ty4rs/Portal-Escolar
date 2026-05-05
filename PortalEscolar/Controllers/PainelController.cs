using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalEscolar.Models;
using PortalEscolar.Services;
using PortalEscolar.Views.ViewModel;
using System.Security.Claims;
using System.Linq;


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
        [Authorize(Roles = "PROFESSOR")]
        public async Task<IActionResult> CriarMateriaPeriodo(int Cargahoraria)
        {
            List<Materia> materiasc = await _materiaService.ListarMaterias();
            List<Periodo> periodos = await _materiaService.ListarPeriodos();

            MateriasViewModel materias2 = new MateriasViewModel()
            {
                materias = new SelectList(
                    materiasc.Select(m => new
                    {
                        Text = m.Nome,
                        Value = m.IdMateria
                    }), "Value", "Text"),
                periodos = new SelectList(
                    periodos.Select(m => new
                    {
                        Text = m.Nome,
                        Value = m.IdPeriodo
                    }), "Value", "Text")
            };
            return View(materias2);
        }
        [HttpPost]
        public async Task<IActionResult> CadastrarMateriaPeriodo(MateriasViewModel MateriaPeriodo)
        {
            int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (await _materiaService.CadastrarMateriaPeriodo(MateriaPeriodo, idUsuario))
            {
                return RedirectToAction("index", "painel");
            }
            return RedirectToAction("CriarMateriaPeriodo", "painel");
        }
        [Authorize(Roles = "ALUNO")]
        public async Task<IActionResult> CadastrarMatriculaMateria()
        {

            var materiasLista = await _materiaService.ListarMaterias();
            var materiasperiodos = await _materiaService.ListarMateriasPeriodos();
            var materias2 = materiasperiodos.Join(materiasLista, mp => mp.IdMateria,
                m=> m.IdMateria,
                (mp,m) => new
                {
                    IdMateriaPeriodo = mp.IdMateriaPeriodo,
                    NomeMateria = m.Nome
                }
                ).ToList();


            MatriculaMateriaViewModel materias = new MatriculaMateriaViewModel()
            {
                materiasPeriodos = new SelectList(
                    materias2.Select(m => new
                    {
                        Text = m.NomeMateria,
                        Value = m.IdMateriaPeriodo
                    }), "Value", "Text")
            };
            return View(materias);

        }
        [Authorize(Roles = "ALUNO")]
        [HttpPost]
        public async Task<IActionResult> CadastroMatriculaMateria(MatriculaMateriaViewModel matriculaMateria)
        {
            int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (await _materiaService.CadastrarMatriculaMateria(matriculaMateria, idUsuario))
            {
                return RedirectToAction("index", "painel");
            }
            return RedirectToAction("CadastrarMatriculaMateria", "painel");
        }

    }
}
