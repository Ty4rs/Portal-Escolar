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
        public MateriaController(IMateriaService materiaService) { 
            _materiaService = materiaService;
        }

        public async Task<IActionResult> Index()
        {
            if(User.IsInRole("ALUNO"))
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
            
            var relatorio = await _materiaService.ObterRelatorioFrequencia(id);

            if (relatorio == null) return NotFound();

            return View(relatorio);
        }

        // --- ÁREA DO PROFESSOR ---

        [HttpGet]
        public async Task<IActionResult> LancarNotas(int id) // id = IdMateriaPeriodo
        {
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

        // --- ÁREA DO ALUNO ---

        [HttpGet]
        public async Task<IActionResult> MeuBoletim()
        {
            // Pega o ID do aluno logado no cookie
            int idAluno = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var boletim = await _materiaService.GerarBoletimAluno(idAluno);
            return View(boletim);
        }

        // GET: Ver o histórico de avaliações lançadas
        [HttpGet]
        public async Task<IActionResult> VerAvaliacoes()
        {
            int idProfessor = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var historico = await _materiaService.ListarHistoricoAvaliacoes(idProfessor);
            return View(historico);
        }

        // POST: Excluir a avaliação (e todas as notas dela)
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
