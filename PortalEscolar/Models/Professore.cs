using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Professore
{
    public int IdProfessor { get; set; }

    public int IdUsuario { get; set; }

    public string Nome { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public string Formacao { get; set; } = null!;

    public string Estatus { get; set; } = null!;

    public DateOnly DataCadastro { get; set; }

    public string? Telefone { get; set; }

    public decimal? Salario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<MateriasPeriodo> MateriasPeriodos { get; set; } = new List<MateriasPeriodo>();
}
