
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PortalEscolar.Data;
using System.Security.Claims;
using System.Security.Principal;


namespace PortalEscolar.Services
{
    public interface IUsuarioService()
    {
        Task<bool> login(string Email, string senha);
    }
    public class UsuarioService : IUsuarioService
    {
        private readonly PortalescolarContext _context; 


        public UsuarioService(PortalescolarContext Context)
        {
            _context = Context;  //Pegando o contexto do DB
        }

        public async Task<bool> login(string Email, string Senha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == Email && u.Senha == Senha); // Verifica se existe esse usuario com esse email se senha.
            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.IdTipoUsuario.ToString())
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
    }
}
