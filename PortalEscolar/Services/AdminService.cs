using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalEscolar.Data;
using PortalEscolar.Models;
using PortalEscolar.Views.ViewModel;
using PortalEscolar.Views.ViewModel.Admin;
using PortalEscolar.Views.ViewModel.Admin.Painel;
using System.Security.Claims;

namespace PortalEscolar.Services
    
{
    public interface IAdminService
    {
        Task<GerenciarAlunosViewModel> ListarAlunosG();
        Task<int> IdAlunoToUser(int idUsuario);
        Task<GerenciarProfessoresViewModel> ListarProfessoresG();
        Task<List<DesempenhoCursoViewModel>> ObterDesempenhoPorCurso();
        Task<bool> ExcluirAluno(int idAluno);
        Task<List<MateriaRankingReprovacaoViewModel>> ObterMateriaComMaisReprovacoes();
        Task<QuantidadeUsuariosViewModel> ObterUsuarios();
        Task<bool> CriarCurso(string nome, string duracao);
        Task<bool> ExcluirCursoCompleto(int idCurso);
        Task<List<ExcluirCursoViewModel>> ListarCursos();
    }
    public class AdminService : IAdminService
    {
        private readonly PortalescolarContext _context;
        private readonly IUsuarioService _usuarioService;
        public AdminService(PortalescolarContext Context)
        {
            _context = Context;
        }

        public async Task<int> IdAlunoToUser(int idAluno)
        {
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.IdAluno == idAluno);

            if (aluno != null)
            {
                return aluno.IdUsuario;
            }
            return -1;
        }

        public async Task<GerenciarAlunosViewModel> ListarAlunosG()
        {
            var alunos = await _context.Alunos.ToListAsync();

            var alunosViewModel = new GerenciarAlunosViewModel
            {
                Alunos = alunos
            };
            return alunosViewModel;
        }
        public async Task<GerenciarProfessoresViewModel> ListarProfessoresG()
        {
            var lista = await _context.Professores.ToListAsync();

            var professoresViewModel = new GerenciarProfessoresViewModel
            {
                Professores = lista ?? new List<Professore>() 
            };

            return professoresViewModel;
        }
        public async Task<bool> ExcluirAluno(int idAluno)
        {
            var aluno = await _context.Alunos
                .Include(a => a.Matriculas)
                .FirstOrDefaultAsync(a => a.IdAluno == idAluno);

            if (aluno == null) return false;

            foreach (var mat in aluno.Matriculas)
            {
                var vinculosMaterias = await _context.MatriculasMaterias
                    .Where(mm => mm.IdMatricula == mat.IdMatricula)
                    .ToListAsync();

                foreach (var vinculo in vinculosMaterias)
                {
                    var frequencias = await _context.Frequencias
                        .Where(f => f.IdMatriculaMateria == vinculo.IdMatriculaMateria).ToListAsync();
                    _context.Frequencias.RemoveRange(frequencias);

                    var notas = await _context.Avaliacoes
                        .Where(av => av.IdMatriculaMateria == vinculo.IdMatriculaMateria).ToListAsync();
                    _context.Avaliacoes.RemoveRange(notas);
                }

                _context.MatriculasMaterias.RemoveRange(vinculosMaterias);

                _context.Matriculas.Remove(mat);
            }

            _context.Alunos.Remove(aluno);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == aluno.IdUsuario);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<DesempenhoCursoViewModel>> ObterDesempenhoPorCurso()
        {
            var estatisticas = await _context.MatriculasMaterias
                .Where(mm => mm.IdMateriaPeriodoNavigation.Concluida == true && mm.NotaFinal != null)

                .GroupBy(mm => mm.IdMatriculaNavigation.IdCursoNavigation.Nome)

                .Select(g => new DesempenhoCursoViewModel
                {
                    NomeCurso = g.Key,
                    MediaGeral = g.Average(mm => (decimal)mm.NotaFinal),

                    TotalAlunos = g.Select(mm => mm.IdMatriculaNavigation.IdAluno).Distinct().Count()
                })
                .OrderByDescending(x => x.MediaGeral)
                .ToListAsync();

            return estatisticas;
        }
        public async Task<List<MateriaRankingReprovacaoViewModel>> ObterMateriaComMaisReprovacoes()
        {
            var resultado = await _context.MatriculasMaterias
                .Where(mm => mm.Status == "REPROVADO")
                .GroupBy(mm => mm.IdMateriaPeriodoNavigation.IdMateriaNavigation.Nome)
                .Select(g => new MateriaRankingReprovacaoViewModel
                {
                    NomeMateria = g.Key,
                    QuantidadeReprovados = g.Count(),
                    NomeProfessor = g.Select(mm => mm.IdMateriaPeriodoNavigation.IdProfessorNavigation.Nome).FirstOrDefault()
                })
                .OrderByDescending(x => x.QuantidadeReprovados)
                .Take(3)
                .ToListAsync();

            return resultado;
        }
        public async Task<QuantidadeUsuariosViewModel> ObterUsuarios()
        {
            var quantidadeUsuarios = new QuantidadeUsuariosViewModel
            {
                Usuariosc = await _context.Usuarios.CountAsync(),
                ProfessoresC = await _context.Professores.CountAsync(),
                Alunosc = await _context.Alunos.CountAsync()
            };
            return quantidadeUsuarios;
        }
        public async Task<bool> CriarCurso(string nome, string duracao)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(duracao))
            {
                return false;
            }
           
            bool cursoExistente = await _context.Cursos.AnyAsync(c => c.Nome == nome);
            if (cursoExistente)
            {
                return false; 
            }
            var novoCurso = new Curso
            {
                Nome = nome,
                Duracao = duracao
            };
            _context.Cursos.Add(novoCurso);
            await _context.SaveChangesAsync();
            return true;

        }
        public async Task<bool> ExcluirCursoCompleto(int idCurso)
        {
            var curso = await _context.Cursos
                .Include(c => c.MateriasPeriodos)
                .Include(c => c.Matriculas)
                .FirstOrDefaultAsync(c => c.IdCurso == idCurso);

            if (curso == null) return false;
            foreach (var mp in curso.MateriasPeriodos)
            {
                var vinculos = await _context.MatriculasMaterias
                    .Where(mm => mm.IdMateriaPeriodo == mp.IdMateriaPeriodo).ToListAsync();

                foreach (var v in vinculos)
                {
                    var freqs = _context.Frequencias.Where(f => f.IdMatriculaMateria == v.IdMatriculaMateria);
                    var notas = _context.Avaliacoes.Where(a => a.IdMatriculaMateria == v.IdMatriculaMateria);
                    _context.Frequencias.RemoveRange(freqs);
                    _context.Avaliacoes.RemoveRange(notas);
                }
                _context.MatriculasMaterias.RemoveRange(vinculos);
            }
            _context.MateriasPeriodos.RemoveRange(curso.MateriasPeriodos);

            _context.Matriculas.RemoveRange(curso.Matriculas);

            _context.Cursos.Remove(curso);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ExcluirCursoViewModel>> ListarCursos()
        {
            var cursos = await _context.Cursos
                .Include(c => c.Matriculas)
                .Include(c => c.MateriasPeriodos)
                .Select(c => new ExcluirCursoViewModel
                {
                    IdCurso = c.IdCurso,
                    NomeCurso = c.Nome,
                    TotalMatriculas = c.Matriculas.Count(),
                    TotalMaterias = c.MateriasPeriodos.Count()
                })
                .ToListAsync();
            return cursos;
        }
    }
    }

