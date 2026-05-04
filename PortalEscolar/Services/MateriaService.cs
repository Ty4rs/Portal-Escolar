using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PortalEscolar.Data;
using Microsoft.AspNetCore.Http; // Preciso pra usar os cookies
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using PortalEscolar.Models;

namespace PortalEscolar.Services
{
    public interface IMateriaService
    {
        Task<bool> VerificarMateria(string nome, int cargaHoraria);
        Task<bool> CadastrarMateria(string nome, int cargaHoraria);
        Task<List<Materia>> ListarMaterias();
    }
    public class MateriaService : IMateriaService
    {
        private readonly PortalescolarContext _context;
        public MateriaService(PortalescolarContext Context)
        {
            _context = Context;
        }
        public async Task<bool> VerificarMateria(string nome, int cargaHoraria)
        {
            if(await _context.Materias.AnyAsync(m=>m.Nome == nome) || await _context.Materias.AnyAsync(m=> m.Cargahoraria == cargaHoraria))
            {
                return true;
            }
            return false;
            
        }
        public async Task<bool> CadastrarMateria(string nome, int cargaHoraria)
        {
            Materia materia = new Materia()
            {
                Nome = nome,
                Cargahoraria = cargaHoraria
            };
            await _context.Materias.AddAsync(materia);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Materia>> ListarMaterias()
        {
            return await _context.Materias.ToListAsync();
        }


    }
}
