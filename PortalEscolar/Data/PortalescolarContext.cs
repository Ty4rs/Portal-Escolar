using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PortalEscolar.Models;

namespace PortalEscolar.Data;

public partial class PortalescolarContext : DbContext
{
    public PortalescolarContext()
    {
    }

    public PortalescolarContext(DbContextOptions<PortalescolarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administradore> Administradores { get; set; }

    public virtual DbSet<Aluno> Alunos { get; set; }

    public virtual DbSet<Avaliaco> Avaliacoes { get; set; }

    public virtual DbSet<Curso> Cursos { get; set; }

    public virtual DbSet<Frequencia> Frequencias { get; set; }

    public virtual DbSet<Materia> Materias { get; set; }

    public virtual DbSet<MateriasPeriodo> MateriasPeriodos { get; set; }

    public virtual DbSet<Matricula> Matriculas { get; set; }

    public virtual DbSet<MatriculasMateria> MatriculasMaterias { get; set; }

    public virtual DbSet<Periodo> Periodos { get; set; }

    public virtual DbSet<Professore> Professores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;;Database=portalescolar;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administradore>(entity =>
        {
            entity.HasKey(e => e.IdAdmin).HasName("PK__administ__89472E953AD2E90B");

            entity.ToTable("administradores");

            entity.HasIndex(e => e.IdUsuario, "UQ__administ__4E3E04ACE6EB96F5").IsUnique();

            entity.Property(e => e.IdAdmin).HasColumnName("id_admin");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Administradore)
                .HasForeignKey<Administradore>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_admin_usuario");
        });

        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.HasKey(e => e.IdAluno).HasName("PK__alunos__8D231D0919FD7BCB");

            entity.ToTable("alunos");

            entity.HasIndex(e => e.IdUsuario, "UQ__alunos__4E3E04ACF0584C18").IsUnique();

            entity.HasIndex(e => e.Cpf, "UQ__alunos__D836E71F2EBE4629").IsUnique();

            entity.Property(e => e.IdAluno).HasColumnName("id_aluno");
            entity.Property(e => e.Cpf)
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnName("cpf");
            entity.Property(e => e.DataNascimento).HasColumnName("data_nascimento");
            entity.Property(e => e.Endereco)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("endereco");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefone");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Aluno)
                .HasForeignKey<Aluno>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_aluno_usuario");
        });

        modelBuilder.Entity<Avaliaco>(entity =>
        {
            entity.HasKey(e => e.IdAvaliacao).HasName("PK__avaliaco__272BC05D32B27A68");

            entity.ToTable("avaliacoes");

            entity.Property(e => e.IdAvaliacao).HasColumnName("id_avaliacao");
            entity.Property(e => e.DataAvaliacao).HasColumnName("data_avaliacao");
            entity.Property(e => e.Descricao)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descricao");
            entity.Property(e => e.IdMatriculaMateria).HasColumnName("id_matricula_materia");
            entity.Property(e => e.NotaAvaliacao)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("nota_avaliacao");

            entity.HasOne(d => d.IdMatriculaMateriaNavigation).WithMany(p => p.Avaliacos)
                .HasForeignKey(d => d.IdMatriculaMateria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_av_vinculo");
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.IdCurso).HasName("PK__cursos__5D3F7502768C3BF0");

            entity.ToTable("cursos");

            entity.Property(e => e.IdCurso).HasColumnName("id_curso");
            entity.Property(e => e.Duracao)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("duracao");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Frequencia>(entity =>
        {
            entity.HasKey(e => e.IdFrequencia).HasName("PK__frequenc__E0D6C3C29238B58A");

            entity.ToTable("frequencias");

            entity.Property(e => e.IdFrequencia).HasColumnName("id_frequencia");
            entity.Property(e => e.DataFrequencia).HasColumnName("data_frequencia");
            entity.Property(e => e.HoraFrequencia)
                .HasPrecision(0)
                .HasColumnName("hora_frequencia");
            entity.Property(e => e.IdMatriculaMateria).HasColumnName("id_matricula_materia");
            entity.Property(e => e.Presenca).HasColumnName("presenca");

            entity.HasOne(d => d.IdMatriculaMateriaNavigation).WithMany(p => p.Frequencia)
                .HasForeignKey(d => d.IdMatriculaMateria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_freq_vinculo");
        });

        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(e => e.IdMateria).HasName("PK__materias__7E03FD39FF709A54");

            entity.ToTable("materias");

            entity.Property(e => e.IdMateria).HasColumnName("id_materia");
            entity.Property(e => e.Cargahoraria).HasColumnName("cargahoraria");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<MateriasPeriodo>(entity =>
        {
            entity.HasKey(e => e.IdMateriaPeriodo).HasName("PK__materias__30ADAEB763EEB806");

            entity.ToTable("materias_periodos");

            entity.Property(e => e.IdMateriaPeriodo).HasColumnName("id_materia_periodo");
            entity.Property(e => e.IdMateria).HasColumnName("id_materia");
            entity.Property(e => e.IdPeriodo).HasColumnName("id_periodo");
            entity.Property(e => e.IdProfessor).HasColumnName("id_professor");
            entity.Property(e => e.Sala)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sala");

            entity.HasOne(d => d.IdMateriaNavigation).WithMany(p => p.MateriasPeriodos)
                .HasForeignKey(d => d.IdMateria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_matper_materia");

            entity.HasOne(d => d.IdPeriodoNavigation).WithMany(p => p.MateriasPeriodos)
                .HasForeignKey(d => d.IdPeriodo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_matper_periodo");

            entity.HasOne(d => d.IdProfessorNavigation).WithMany(p => p.MateriasPeriodos)
                .HasForeignKey(d => d.IdProfessor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_matper_professor");
        });

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasKey(e => e.IdMatricula).HasName("PK__matricul__1D7CF00B967AEAC7");

            entity.ToTable("matriculas");

            entity.Property(e => e.IdMatricula).HasColumnName("id_matricula");
            entity.Property(e => e.DataMatricula)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("data_matricula");
            entity.Property(e => e.IdAluno).HasColumnName("id_aluno");
            entity.Property(e => e.IdCurso).HasColumnName("id_curso");

            entity.HasOne(d => d.IdAlunoNavigation).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.IdAluno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_matricula_aluno");

            entity.HasOne(d => d.IdCursoNavigation).WithMany(p => p.Matriculas)
                .HasForeignKey(d => d.IdCurso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_matricula_curso");
        });

        modelBuilder.Entity<MatriculasMateria>(entity =>
        {
            entity.HasKey(e => e.IdMatriculaMateria).HasName("PK__matricul__94E3949CB5124B1E");

            entity.ToTable("matriculas_materias");

            entity.Property(e => e.IdMatriculaMateria).HasColumnName("id_matricula_materia");
            entity.Property(e => e.DataVinculo)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("data_vinculo");
            entity.Property(e => e.IdMateriaPeriodo).HasColumnName("id_materia_periodo");
            entity.Property(e => e.IdMatricula).HasColumnName("id_matricula");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.IdMateriaPeriodoNavigation).WithMany(p => p.MatriculasMateria)
                .HasForeignKey(d => d.IdMateriaPeriodo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_matmat_periodo");

            entity.HasOne(d => d.IdMatriculaNavigation).WithMany(p => p.MatriculasMateria)
                .HasForeignKey(d => d.IdMatricula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_matmat_matricula");
        });

        modelBuilder.Entity<Periodo>(entity =>
        {
            entity.HasKey(e => e.IdPeriodo).HasName("PK__periodos__801188B7AE3104F0");

            entity.ToTable("periodos");

            entity.Property(e => e.IdPeriodo).HasColumnName("id_periodo");
            entity.Property(e => e.Ano).HasColumnName("ano");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Semestre).HasColumnName("semestre");
        });

        modelBuilder.Entity<Professore>(entity =>
        {
            entity.HasKey(e => e.IdProfessor).HasName("PK__professo__BB758483CDD850C7");

            entity.ToTable("professores");

            entity.HasIndex(e => e.IdUsuario, "UQ__professo__4E3E04ACB0F40AB4").IsUnique();

            entity.HasIndex(e => e.Cpf, "UQ__professo__D836E71F478763AA").IsUnique();

            entity.Property(e => e.IdProfessor).HasColumnName("id_professor");
            entity.Property(e => e.Cpf)
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnName("cpf");
            entity.Property(e => e.DataCadastro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("data_cadastro");
            entity.Property(e => e.Estatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.Formacao)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("formacao");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nome");
            entity.Property(e => e.Salario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salario");
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefone");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Professore)
                .HasForeignKey<Professore>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_professor_usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__4E3E04ADCEACA550");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Email, "UQ__usuarios__AB6E61647B855D3A").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Senha)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("senha");
            entity.Property(e => e.TipoUsuario)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo_usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
