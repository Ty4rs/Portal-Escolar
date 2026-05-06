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
        readonly IUsuarioService _usuarioService;
        public PainelController(IMateriaService materiaService, IUsuarioService usuarioService) { 
            _materiaService = materiaService;
            _usuarioService = usuarioService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "PROFESSOR")]
        public IActionResult CriarMateria()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CadastrarMateria(string Nome, int Cargahoraria)
        {
            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }

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

            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }

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
            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }

            var materiasLista = await _materiaService.ListarMaterias();
            var materiasperiodos = await _materiaService.ListarMateriasPeriodos();
            var materias2 = materiasperiodos.Join(materiasLista, mp => mp.IdMateria,
                m=> m.IdMateria,
                (mp,m) => new
                {
                    IdMateriaPeriodo = mp.IdMateriaPeriodo,
                    NomeMateria = m.Nome,
                    IdProfessor = mp.IdProfessor
                }
                ).ToList();
            var professores = await _materiaService.ListarProfessores();
            var materias3 = materias2.Join(professores, mp=> mp.IdProfessor, p=> p.IdProfessor, (mp, p)=> new
            {
                    IdMateriaPeriodo = mp.IdMateriaPeriodo,
                    NomeMateria = mp.NomeMateria,
                    NomeProfessor = p.Nome
            });
            MatriculaMateriaViewModel materias = new MatriculaMateriaViewModel()
            {
                materiasPeriodos = new SelectList(
                    materias3.Select(m => new
                    {
                        Text = $"{m.NomeMateria}  - {m.NomeProfessor}",
                        Value = m.IdMateriaPeriodo
                    }), "Value", "Text")
            };
            return View(materias);

        }
        [Authorize(Roles = "ALUNO")]
        [HttpPost]
        public async Task<IActionResult> CadastroMatriculaMateria(MatriculaMateriaViewModel matriculaMateria)
        {

            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }
            int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (await _materiaService.CadastrarMatriculaMateria(matriculaMateria, idUsuario))
            {
                return RedirectToAction("index", "materia");
            }
            TempData["MensagemErro"] = "Já está matriculado nesta matéria.";
            return RedirectToAction("CadastrarMatriculaMateria", "painel");
        }

    }
}
