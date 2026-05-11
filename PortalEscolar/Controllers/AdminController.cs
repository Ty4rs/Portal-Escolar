using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalEscolar.Services;
using PortalEscolar.Views.ViewModel;
using PortalEscolar.Views.ViewModel.Admin;
using System.Security.Claims;

namespace PortalEscolar.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IUsuarioService _usuarioService;
        private readonly IMateriaService _materiaService;
        public AdminController(IAdminService adminService, IUsuarioService usuarioService, IMateriaService materiaService)
        {
            _adminService = adminService;
            _usuarioService = usuarioService;
            _materiaService = materiaService;
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult GerenciarUsuarios()
        {
            return View();
        }
        [Authorize(Roles = "ADMIN")]
        public IActionResult GerenciarMaterias()
        {
            return View();
        }
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GerenciarAlunos()
        {
            var alunos = await _adminService.ListarAlunosG();
            return View(alunos);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> VerBoletim(int id)
        {
            int teste = id;
            int idAlunoL = await _adminService.IdAlunoToUser(id);
            var boletim = await _materiaService.GerarBoletimAluno(idAlunoL);
            return View(boletim);
        }
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ExcluirAluno(int id)
        {
            var result = await _adminService.ExcluirAluno(id);

            return RedirectToAction("GerenciarAlunos");

        }
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GerenciarProfessores()
        {
            var professores = await _adminService.ListarProfessoresG();
            if (professores.Professores == null)
            {
                return Content("A lista de professores está nula vindo do Service");
            }
            return View(professores);
        }
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Estatistica(int id)
        {
            var dados = await _adminService.ObterDesempenhoPorCurso();
            var viewModel = new EstatisticaViewModel
            {
                DesempenhoCurso = dados
            };
            ViewBag.Cursos = dados.Select(x => x.NomeCurso).ToList();
            ViewBag.Medias = dados.Select(x => x.MediaGeral).ToList();

            var materiasCriticas = await _adminService.ObterMateriaComMaisReprovacoes();
            ViewBag.NomesMaterias = materiasCriticas.Select(x => x.NomeMateria + " - " + x.NomeProfessor).ToList();
            ViewBag.QtdReprovados = materiasCriticas.Select(x => x.QuantidadeReprovados).ToList();

            var quantidadeUsuarios = await _adminService.ObterUsuarios();
            ViewBag.Usuarios = quantidadeUsuarios.Usuariosc;
            ViewBag.Professores = quantidadeUsuarios.ProfessoresC;
            ViewBag.Alunos = quantidadeUsuarios.Alunosc;

            return View(viewModel);
        }
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CriarCurso()
        {
            var cursos = await _adminService.ListarCursos();
            return View(cursos);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CadastrarCurso(string nome, string duracao )
        {

            if (!await _adminService.CriarCurso(nome, duracao))
            {
                return RedirectToAction("index", "admin");
            }
            return RedirectToAction("criarCurso", "admin");
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ExcluirCurso(int idCurso)
        {
            var result = await _adminService.ExcluirCursoCompleto(idCurso);
            return RedirectToAction("criarCurso", "admin");
        }
    }
}
