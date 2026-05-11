using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore;
using PortalEscolar.Data;
using PortalEscolar.Models;
using PortalEscolar.Views.ViewModel;
using System.Security.Claims;
using System.Security.Principal;

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
        Task<List<MateriasPeriodo>> ListarMateriasPeriodos(int idUser);
        Task<bool> CadastrarMatriculaMateria(MatriculaMateriaViewModel materiaPeriodo, int IdAuluno);
        Task<MateriaAlunoViewModel> ListarMateriasAluno(int idAluno);
        Task<MateriaProfessorViewModel> ListarMateriasProfessor(int idProfessor);
        Task<FrequenciaPaginaViewModel> ListarAlunosParaChamada(int idMateriaPeriodo);
        Task<bool> SalvarFrequencia(FrequenciaPaginaViewModel model);
        Task<RelatorioFrequenciaViewModel> ObterRelatorioFrequencia(int idMateriaPeriodo);
        Task<LancarNotaPaginaViewModel> ListarAlunosParaNotas(int idMateriaPeriodo);
        Task<bool> SalvarNotas(LancarNotaPaginaViewModel model);
        Task<BoletimAlunoViewModel> GerarBoletimAluno(int idAluno);
        Task<List<GerenciarAvaliacoesViewModel>> ListarHistoricoAvaliacoes(int idProfessor);
        Task<bool> ExcluirAvaliacaoCompleta(int idMateriaPeriodo, DateTime data);
        Task<List<Professore>> ListarProfessores();
        Task<bool> ConcluirMateria(int idMateriaPeriodo);
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
            if (await _context.Materias.AnyAsync(m => m.Nome == nome))
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
            if (materiaPeriodo.Sala == null)
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
                IdCurso = materiaPeriodo._idCurso,
                Sala = materiaPeriodo.Sala

            };
            await _context.MateriasPeriodos.AddAsync(_materiaPeriodo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MateriasPeriodo>> ListarMateriasPeriodos(int idUser)
        {
            
            var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.IdAlunoNavigation.IdUsuario == idUser);
            if (matricula == null)
            {
                return new List<MateriasPeriodo>();
            }


            return await _context.MateriasPeriodos.Where(mp => mp.Concluida == false && mp.IdCurso == matricula.IdCurso).ToListAsync();
        }
        public async Task<bool> CadastrarMatriculaMateria(MatriculaMateriaViewModel materiaPeriodo, int IdAuluno)
        {
            var idAluno = await _context.Alunos.FirstOrDefaultAsync(p => p.IdUsuario == IdAuluno);
            var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.IdAluno == idAluno.IdAluno);
            if (await _context.MatriculasMaterias.AnyAsync(mm => mm.IdMatricula == matricula.IdMatricula && mm.IdMateriaPeriodo == materiaPeriodo._idMateriaPeriodo))
            {
                return false;
            }
            MatriculasMateria matriculaMateria = new MatriculasMateria()
            {
                IdMatricula = matricula.IdMatricula,
                IdMateriaPeriodo = materiaPeriodo._idMateriaPeriodo,
                DataVinculo = DateOnly.FromDateTime(DateTime.Now),
                Status = "MATRICULADO"
            };
            await _context.MatriculasMaterias.AddAsync(matriculaMateria);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Professore>> ListarProfessores()
        {
            return await _context.Professores.ToListAsync();
        }

        public async Task<MateriaAlunoViewModel> ListarMateriasAluno(int idUsuario)
        {
            var idAlunoC = await _context.Alunos.FirstAsync(p => p.IdUsuario == idUsuario);
            var idAluno = idAlunoC.IdAluno;
            var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.IdAluno == idAluno);
            if (matricula == null) return new MateriaAlunoViewModel();

            var materiasMatriculadas = await _context.MatriculasMaterias
                .Where(mm => mm.IdMatricula == matricula.IdMatricula).ToListAsync();
            var materiasPeriodo = await _context.MateriasPeriodos.Where(mp=> mp.Concluida == false).ToListAsync();
            var materias = await _context.Materias.ToListAsync();

            var listaResultado = materiasMatriculadas.Join(materiasPeriodo,
                mm => mm.IdMateriaPeriodo,
                mp => mp.IdMateriaPeriodo,
                (mm, mp) => new { mm, mp })
            .Join(materias,
                temp => temp.mp.IdMateria,
                m => m.IdMateria,
                (temp, m) => new MateriaAlunoViewModel
                {
                    IdMatriculaMateria = temp.mm.IdMatriculaMateria,
                    IdMateriaPeriodo = temp.mp.IdMateriaPeriodo,
                    IdMateria = m.IdMateria,
                    NomeMateria = m.Nome,
                    CargaHoraria = m.Cargahoraria,
                    Sala = temp.mp.Sala,
                    NomeProfessor = _context.Professores.FirstOrDefault(p => p.IdProfessor == temp.mp.IdProfessor).Nome
                }).ToList();

            return new MateriaAlunoViewModel
            {
                Materias = listaResultado
            };
        }

        public async Task<MateriaProfessorViewModel> ListarMateriasProfessor(int idUsuario)
        {
            Professore idProfessorC = await _context.Professores.FirstAsync(p=> p.IdUsuario == idUsuario);
            int idProfessor = idProfessorC.IdProfessor;
            var materiasPeriodo = await _context.MateriasPeriodos
                .Where(mp => mp.IdProfessor == idProfessor)
                .Where(mp=> mp.Concluida == false)
                .ToListAsync();

            var materias = await _context.Materias.ToListAsync();


            var listaResultado = materiasPeriodo.Join(materias,
                mp => mp.IdMateria,
                m => m.IdMateria,
                (mp, m) => new MateriaProfessorViewModel
                {
                    IdMateriaPeriodo = mp.IdMateriaPeriodo,
                    IdMateria = m.IdMateria,
                    NomeMateria = m.Nome,
                    CargaHoraria = m.Cargahoraria,
                    Sala = mp.Sala,
                    NomeCurso = _context.Cursos.FirstOrDefault(c => c.IdCurso == mp.IdCurso).Nome
                }).ToList();

            
            return new MateriaProfessorViewModel
            {
                Materias = listaResultado
            };
        }

        public async Task<FrequenciaPaginaViewModel> ListarAlunosParaChamada(int idMateriaPeriodo)
        {
            
            var alunosDaTurma = await (from mm in _context.MatriculasMaterias
                                       join m in _context.Matriculas on mm.IdMatricula equals m.IdMatricula
                                       join a in _context.Alunos on m.IdAluno equals a.IdAluno
                                       where mm.IdMateriaPeriodo == idMateriaPeriodo
                                       select new FrequenciaLinhaViewModel
                                       {
                                           IdMatriculaMateria = mm.IdMatriculaMateria,
                                           NomeAluno = a.Nome,
                                           Presenca = true 
                                       }).ToListAsync();

            return new FrequenciaPaginaViewModel
            {
                IdMateriaPeriodo = idMateriaPeriodo,
                Alunos = alunosDaTurma
            };
        }
        public async Task<bool> SalvarFrequencia(FrequenciaPaginaViewModel model)
        {
            foreach (var item in model.Alunos)
            {
                var frequencia = new Frequencia
                {
                    IdMatriculaMateria = item.IdMatriculaMateria,
                    DataFrequencia = DateOnly.FromDateTime(model.Data),
                    HoraFrequencia = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay),
                    Presenca = item.Presenca
                };
                _context.Frequencias.Add(frequencia);
            }
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<RelatorioFrequenciaViewModel> ObterRelatorioFrequencia(int idMateriaPeriodo)
        {
            
            var materia = await _context.MateriasPeriodos.Where(mp=> mp.Concluida==true)
                .Include(mp => mp.IdMateriaNavigation)
                .FirstOrDefaultAsync(mp => mp.IdMateriaPeriodo == idMateriaPeriodo);

            
            var dadosBrutos = await _context.Frequencias
                .Include(f => f.IdMatriculaMateriaNavigation)
                    .ThenInclude(mm => mm.IdMatriculaNavigation)
                        .ThenInclude(m => m.IdAlunoNavigation)
                .Where(f => f.IdMatriculaMateriaNavigation.IdMateriaPeriodo == idMateriaPeriodo)
                .ToListAsync();

            
            var resumo = dadosBrutos
                .GroupBy(f => f.IdMatriculaMateriaNavigation.IdMatriculaNavigation.IdAlunoNavigation.Nome)
                .Select(g => new AlunoFrequenciaResumo
                {
                    NomeAluno = g.Key,
                    TotalPresencas = g.Count(f => f.Presenca),
                    TotalFaltas = g.Count(f => !f.Presenca),
                    PorcentagemPresenca = g.Any()
                        ? (double)g.Count(f => f.Presenca) / g.Count() * 100
                        : 0
                }).ToList();

            return new RelatorioFrequenciaViewModel
            {
                NomeMateria = materia?.IdMateriaNavigation?.Nome ?? "Matéria não encontrada",
                ResumoAlunos = resumo
            };
        }
        public async Task<LancarNotaPaginaViewModel> ListarAlunosParaNotas(int idMateriaPeriodo)
        {
            var alunos = await (from mm in _context.MatriculasMaterias
                                join m in _context.Matriculas on mm.IdMatricula equals m.IdMatricula
                                join a in _context.Alunos on m.IdAluno equals a.IdAluno
                                where mm.IdMateriaPeriodo == idMateriaPeriodo
                                select new LancarNotaLinhaViewModel
                                {
                                    IdMatriculaMateria = mm.IdMatriculaMateria,
                                    NomeAluno = a.Nome
                                }).ToListAsync();

            return new LancarNotaPaginaViewModel { IdMateriaPeriodo = idMateriaPeriodo, Alunos = alunos };
        }
        public async Task<bool> SalvarNotas(LancarNotaPaginaViewModel model)
        {
            foreach (var item in model.Alunos)
            {
                var avaliacao = new Avaliaco
                {
                    IdMatriculaMateria = item.IdMatriculaMateria,
                    NotaAvaliacao = item.Nota,
                    DataAvaliacao = DateOnly.FromDateTime(model.DataAvaliacao)
                };
                _context.Avaliacoes.Add(avaliacao);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<BoletimAlunoViewModel> GerarBoletimAluno(int idUsuario)
        {
            
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.IdUsuario == idUsuario);

            
            if (aluno == null) return new BoletimAlunoViewModel { ListaBoletim = new List<BoletimAlunoViewModel>() };


            var dados = await _context.MatriculasMaterias
                .Where(mm => mm.IdMatriculaNavigation.IdAluno == aluno.IdAluno)
                .Select(mm => new BoletimAlunoViewModel
                {
                    NomeMateria = mm.IdMateriaPeriodoNavigation.IdMateriaNavigation.Nome,
                    Notas = mm.Avaliacos.Select(a => a.NotaAvaliacao).ToList(),
                    Media = mm.Avaliacos.Any() ? mm.Avaliacos.Average(a => a.NotaAvaliacao) : 0,
                    AulasMinistradas = mm.Frequencia.Count(),
                    TotalFaltas = mm.Frequencia.Count(f => !f.Presenca),
                    PorcentagemFrequencia = mm.Frequencia.Any()
                        ? (double)mm.Frequencia.Count(f => f.Presenca) / mm.Frequencia.Count() * 100
                        : 100,
                    Situacao = mm.Status.ToString(),
                }).ToListAsync();

            return new BoletimAlunoViewModel { ListaBoletim = dados };
        }


        public async Task<List<GerenciarAvaliacoesViewModel>> ListarHistoricoAvaliacoes(int idUsuario)
        {
            Professore idProfessorC = await _context.Professores.FirstAsync(p => p.IdUsuario == idUsuario);
            int idProfessor = idProfessorC.IdProfessor;

            var avaliacoesDoProfessor = await _context.Avaliacoes
                .Include(a => a.IdMatriculaMateriaNavigation)
                    .ThenInclude(mm => mm.IdMateriaPeriodoNavigation)
                        .ThenInclude(mp => mp.IdMateriaNavigation)
                .Where(a => a.IdMatriculaMateriaNavigation.IdMateriaPeriodoNavigation.IdProfessor == idProfessor)
                .ToListAsync();

            var historico = avaliacoesDoProfessor
                .GroupBy(a => new
                {
                    a.DataAvaliacao,
                    NomeMateria = a.IdMatriculaMateriaNavigation.IdMateriaPeriodoNavigation.IdMateriaNavigation.Nome,
                    IdMateriaPeriodo = a.IdMatriculaMateriaNavigation.IdMateriaPeriodo
                })
                .Select(g => new GerenciarAvaliacoesViewModel
                {
                    Data = g.Key.DataAvaliacao.ToDateTime(TimeOnly.MinValue),
                    NomeMateria = g.Key.NomeMateria,
                    IdMateriaPeriodo = g.Key.IdMateriaPeriodo,
                    TotalAlunos = g.Count()
                })
                .OrderByDescending(x => x.Data)
                .ToList();

            return historico;
        }
        public async Task<bool> ExcluirAvaliacaoCompleta(int idMateriaPeriodo, DateTime data)
            {
                var notasParaRemover = await _context.Avaliacoes
                    .Where(a => a.IdMatriculaMateriaNavigation.IdMateriaPeriodo == idMateriaPeriodo && a.DataAvaliacao == DateOnly.FromDateTime(data))
                    .ToListAsync();

                if (!notasParaRemover.Any()) return false;

                _context.Avaliacoes.RemoveRange(notasParaRemover);
                return await _context.SaveChangesAsync() > 0;
            }


        public async Task<bool> ConcluirMateria(int idMateriaPeriodo)
        {
            
            var materiaPeriodo = await _context.MateriasPeriodos
                .Include(mp => mp.MatriculasMateria)
                    .ThenInclude(mm => mm.Avaliacos) 
                .Include(mp => mp.MatriculasMateria)
                    .ThenInclude(mm => mm.Frequencia) 
                .FirstOrDefaultAsync(mp => mp.IdMateriaPeriodo == idMateriaPeriodo);

            if (materiaPeriodo == null) return false;

            foreach (var matricula in materiaPeriodo.MatriculasMateria)
            {
                decimal somaNotas = matricula.Avaliacos.Sum(a => a.NotaAvaliacao);

                int totalAulas = matricula.Frequencia.Count;
                int presencas = matricula.Frequencia.Count(f => f.Presenca);
                double freq = totalAulas > 0 ? (double)presencas / totalAulas * 100 : 100;

                if (somaNotas >= (decimal)6.0 && freq >= 75)
                    matricula.Status = "APROVADO";
                else
                    matricula.Status = "REPROVADO";
                matricula.NotaFinal = somaNotas; 
            }
            materiaPeriodo.Concluida = true;
            await _context.SaveChangesAsync();
            return true;
        }
        

    }
    }
