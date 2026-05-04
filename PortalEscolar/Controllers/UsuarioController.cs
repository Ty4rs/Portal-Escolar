using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PortalEscolar.Services;
namespace PortalEscolar.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService UsuarioService)
        {
            _usuarioService = UsuarioService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Cadastro()
        {
            if (TempData["ErrorEmail"]!= null) 
            {
                ModelState.AddModelError("Email", TempData["ErrorEmail"].ToString());
            }

             
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Cadastrar(string Email, string Senha, string TipoUsuario)
        {
            TempData["ErrorEmail"] = null;
            TempData["ErrorSenha"] = null;

            if (await _usuarioService.VerificarEmail(Email))
            {
                TempData["ErrorEmail"] = "Email já Cadastrado ou inválido";
            }
            
            if (await _usuarioService.VerificarSenha(Senha))
            {
                TempData["ErrorSenha"] = "Senha inválida! (Ela precisa conter 8 ou mais caracteres).";
            }
            if (TempData["ErrorEmail"] != null || TempData["ErrorSenha"] != null)
            {
                return RedirectToAction("cadastro", "usuario");
            }

            await _usuarioService.CriarUsuario(Email, Senha, TipoUsuario);
            bool logou = await _usuarioService.Logar(Email, Senha);
            if (logou)
            {
                return RedirectToAction(TipoUsuario.ToLower(), "usuario");
            }
            
            return RedirectToAction("index", "usuario");

        }

        [Authorize(Roles ="PROFESSOR")]
        public async Task<IActionResult> Professor()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CadastrarProfessor(string Nome, string Cpf, string Formacao, string Telefone, decimal Salario)
        {
            
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(!await _usuarioService.CriarProfessor( Nome, Cpf, Formacao, Salario, Telefone, UserId))
            {
                return RedirectToAction("professor", "usuario");
            }
            return RedirectToAction("index", "home");
        }
        [Authorize(Roles ="ALUNO")]
        public async Task<IActionResult> Aluno()
        {
            if (TempData["ErrorAluno"] != null)
            {
                ModelState.AddModelError("IdUsuario", TempData["ErrorAluno"].ToString());
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> cadastrarAluno(string Nome, string Cpf, DateTime DataNascimento, string Endereco, string teledone)
        {
            TempData["ErrorAluno"] = null;
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(!await _usuarioService.CriarAluno(Nome, Cpf, DataNascimento, Endereco, teledone, UserId))
            {
                TempData["ErrorAluno"] = "Aluno já cadastrado!";
                return RedirectToAction("aluno", "usuario");
            }
            return RedirectToAction("aluno", "usuario");
        }

        [HttpPost]
        public async Task<IActionResult> Logar(string Email,string Senha)
        {
            bool logou = await _usuarioService.Logar(Email, Senha);
            if (logou)
            {
                return RedirectToAction("index", "home");
            }
            else
            {
                return RedirectToAction("index", "usuario");
            }
        }

        
    }
}
