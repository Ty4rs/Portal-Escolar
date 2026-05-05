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
    public interface IMateriaService
    {
        Task<bool> VerificarMateria(string nome, int cargaHoraria);
        Task<bool> CadastrarMateria(string nome, int cargaHoraria);
        Task<List<Materia>> ListarMaterias();
        Task<List<Periodo>> ListarPeriodos();
        Task<List<Curso>> ListarCursos();

        Task<bool> CadastrarMateriaPeriodo(MateriasViewModel materiaPeriodo, int IdProfessor);
        Task<bool> ListarMateriasPeriodos();

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
            if(await _context.Materias.AnyAsync(m=>m.Nome == nome))
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
        public async Task<List<Curso>> ListarCursos()
        {
            return await _context.Cursos.ToListAsync();
        }
        public async Task<List<Periodo>> ListarPeriodos()
        {
            return await _context.Periodos.ToListAsync();
        }

        public async Task<bool> CadastrarMateriaPeriodo(MateriasViewModel materiaPeriodo, int IdProfessor)
        {
            if(materiaPeriodo.Sala == null )
            {
                return false;
            }
            var idProff = await _context.Professores.FirstAsync(p => p.IdUsuario == IdProfessor);
            IdProfessor = idProff.IdProfessor;
            MateriasPeriodo _materiaPeriodo = new MateriasPeriodo()
            {
                IdMateria = materiaPeriodo.idMateria,
                IdPeriodo = materiaPeriodo.IdPeriodo,
                IdProfessor = IdProfessor,
                Sala = materiaPeriodo.Sala

            };
            await _context.MateriasPeriodos.AddAsync(_materiaPeriodo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MateriasPeriodo>> ListarMateriasPeriodos()
        {
            return await _context.MateriasPeriodos.ToListAsync();
        }
    }
        
}
