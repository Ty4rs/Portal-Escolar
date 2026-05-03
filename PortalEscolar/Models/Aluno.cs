using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Aluno
{
    public int IdAluno { get; set; }

    public int IdUsuario { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateOnly DataNascimento { get; set; }

    public string? Endereco { get; set; }

    public string? Telefone { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
