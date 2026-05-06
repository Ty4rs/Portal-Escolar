using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalEscolar.Models;
using PortalEscolar.Services;
using PortalEscolar.Views.ViewModel;
using System.Security.Claims;


namespace PortalEscolar.Controllers
{
    public class MateriaController : Controller
    {
        readonly IMateriaService _materiaService;
        readonly IUsuarioService _usuarioService;
        public MateriaController(IMateriaService materiaService, IUsuarioService usuarioService) { 
            _materiaService = materiaService;
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            if(!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }
            if (User.IsInRole("ALUNO"))
            {
                var materias = _materiaService.ListarMateriasAluno(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))).Result;
                return View("AlunoIndex", materias);
            }
            if(User.IsInRole("PROFESSOR"))
            {
                var materias = _materiaService.ListarMateriasProfessor(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))).Result;
                return View("ProfessorIndex", materias);
            }
            else
            {
                return RedirectToAction("aluno", "usuario");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CadastrarFrequencia(int id)
        {
            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }
            var viewModel = await _materiaService.ListarAlunosParaChamada(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> SalvarFrequencia(FrequenciaPaginaViewModel model)
        {
            
            if (model.Alunos == null || !model.Alunos.Any())
            {
                ModelState.AddModelError("", "Não há alunos para registrar frequência.");
                return View("Frequencia", model);
            }

            
            if (await _materiaService.SalvarFrequencia(model))
            {
                
                return RedirectToAction("Index", "Painel");
            }

            
            ModelState.AddModelError("", "Ocorreu um erro ao salvar a frequência no banco de dados.");
            return View("Frequencia", model);
        }
        [HttpGet]
        public async Task<IActionResult> VerFrequencia(int id)
        {
            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }
            var relatorio = await _materiaService.ObterRelatorioFrequencia(id);

            if (relatorio == null) return NotFound();

            return View(relatorio);
        }

       

        [HttpGet]
        public async Task<IActionResult> LancarNotas(int id)
        {
            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }
            var model = await _materiaService.ListarAlunosParaNotas(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarNotas(LancarNotaPaginaViewModel model)
        {
            if (await _materiaService.SalvarNotas(model))
                return RedirectToAction("Index");

            return View("LancarNotas", model);
        }

        [HttpGet]
        public async Task<IActionResult> MeuBoletim()
        {
            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }
            
            int idAluno = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var boletim = await _materiaService.GerarBoletimAluno(idAluno);
            return View(boletim);
        }

        
        [HttpGet]
        public async Task<IActionResult> VerAvaliacoes()
        {
            if (!await _usuarioService.VerificarCadastro(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))))
            {
                return RedirectToAction(User.FindFirstValue(ClaimTypes.Role), "usuario");
            }
            int idProfessor = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var historico = await _materiaService.ListarHistoricoAvaliacoes(idProfessor);
            return View(historico);
        }

        
        [HttpPost]
        public async Task<IActionResult> DeletarAvaliacao(int idMateriaPeriodo, DateTime data)
        {
            if (await _materiaService.ExcluirAvaliacaoCompleta(idMateriaPeriodo, data))
            {
                return RedirectToAction("VerAvaliacoes");
            }
            return BadRequest("Erro ao excluir avaliação.");
        }


    }
}
