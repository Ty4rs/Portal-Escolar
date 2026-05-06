
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PortalEscolar.Data;
using Microsoft.AspNetCore.Http; // Preciso pra usar os cookies
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using PortalEscolar.Models;
using PortalEscolar.Views.ViewModel;

namespace PortalEscolar.Services
{
    public interface IUsuarioService
    {
        Task<bool> Logar(string Email, string senha);
        Task<bool> Deslogar();
        Task<bool> VerificarEmail(string Email);
        Task<bool> VerificarSenha(string Senha);
        Task<bool> CriarUsuario(string Email, string Senha, string TipoUsuario);
        
        Task<bool> CriarProfessor(string nome, string cpf, string formacao, decimal salario, string telefone, int idUsuario);
        Task<bool> CriarAluno(AlunoViewModel alunoModel, int UserId);
        Task<bool> VerificarCadastro(int IdUsuario);
        Task<bool> MatricularAluno(AlunoViewModel alunoModel, int UserId);
    }
    public class UsuarioService : IUsuarioService
    {
        private readonly PortalescolarContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public UsuarioService(PortalescolarContext Context, IHttpContextAccessor ContextAccessor)
        {
            _context = Context;  //Pegando o contexto do DB
            _contextAccessor = ContextAccessor; //Pegando o contexto do HTTP, para acessar os cookies.
        }

        public async Task<bool> Logar(string Email, string Senha)
        {
            var usuario = _context.Usuarios.Include(u => u.Aluno).Include(u=> u.Professore).Include(u=> u.Administradore).FirstOrDefault(u => u.Email == Email && u.Senha == Senha);

            string nomeUsuario = "user";
            

            if (usuario != null)
            {
                if (usuario.Administradore != null) { nomeUsuario = usuario.Administradore.Nome; }
                else if (usuario.Professore != null) { nomeUsuario = usuario.Professore.Nome; }
                else if (usuario.Aluno != null) { nomeUsuario = usuario.Aluno.Nome; }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, nomeUsuario),
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
                }; //Populando o cookie com as informações do usuario.

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); //Criando a identidade do cookie, dizendo que o tipo de autenticação é o cookie.
                var authProperties = new AuthenticationProperties { IsPersistent = true }; //Definindo que o cookie é persistente, ou seja, ele vai durar mesmo depois de fechar o navegador.

                await _contextAccessor.HttpContext.SignInAsync( //Fazendo o login do usuario, ou seja, criando o cookie.
                    CookieAuthenticationDefaults.AuthenticationScheme, //Dizendo que o tipo de autenticação é o cookie.
                    new ClaimsPrincipal(claimsIdentity), //Criando o principal do cookie, ou seja, a identidade do cookie.
                    authProperties); //Passando as propriedades do cookie, como a persistência.

                return true;
            }
            return false;
        }

        public async Task<bool> Deslogar()
        {
            await _contextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }

        public async Task<bool> VerificarEmail(string Email)
        {
            if (Email == null || Email.Length < 5) return true;
            return await _context.Usuarios.AnyAsync(u => u.Email == Email);
        }
        public async Task<bool> VerificarSenha(string Senha)
        {
            if(Senha == null || Senha.Length < 8) 
            {
                
                return true; 
            }
            
            return false;
        }

        public async Task<bool> CriarUsuario(string Email, string Senha, string TipoUsuario)
        {
            Usuario usuario = new Usuario()
            {
                Email = Email,
                Senha = Senha,
                TipoUsuario = TipoUsuario
            };
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> CriarProfessor(string nome, string cpf, string formacao, decimal salario, string telefone, int idUsuario)
        {
            
            if(await _context.Professores.AnyAsync(p=> p.IdUsuario == idUsuario))
            {
                return false;
            }
            Professore professor = new Professore()
            {
               Nome=nome, Cpf=cpf, Formacao=formacao, Telefone=telefone, IdUsuario=idUsuario, DataCadastro=DateOnly.FromDateTime(DateTime.Today), Salario=salario, Estatus="ATIVO"
            };
            await _context.Professores.AddAsync(professor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CriarAluno(AlunoViewModel alunoModel, int UserId)
        {
            
            if(await _context.Alunos.AnyAsync(p=> p.IdUsuario == UserId))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(alunoModel.Nome) || 
                string.IsNullOrWhiteSpace(alunoModel.Cpf) || 
                string.IsNullOrWhiteSpace(alunoModel.Telefone) || 
                string.IsNullOrWhiteSpace(alunoModel.Endereco) ||
                alunoModel.DataNascimento == default)
            {
                return false;
            }
            Aluno aluno = new Aluno()
            {
               Nome=alunoModel.Nome, Cpf=alunoModel.Cpf, DataNascimento = DateOnly.FromDateTime(alunoModel.DataNascimento), Telefone=alunoModel.Telefone, IdUsuario= UserId, Endereco=alunoModel.Endereco
            };
            
            await _context.Alunos.AddAsync(aluno);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> MatricularAluno(AlunoViewModel alunoModel, int UserId)
        {
            Aluno aluno = await _context.Alunos.FirstOrDefaultAsync(a=> a.IdUsuario == UserId);
            Matricula matricula = new Matricula()
            {
                IdCurso = alunoModel.IdCurso, DataMatricula = DateOnly.FromDateTime(DateTime.Now), IdAluno = aluno.IdAluno
            };
            await _context.Matriculas.AddAsync(matricula);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> VerificarCadastro(int IdUsuario)
        {
            if(await _context.Alunos.AnyAsync(a => a.IdUsuario == IdUsuario) || await _context.Professores.AnyAsync(p => p.IdUsuario == IdUsuario))
                return true;
            ;
            return false;
        }

    }
    
}
