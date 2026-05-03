using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Email { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public string TipoUsuario { get; set; } = null!;

    public virtual Administradore? Administradore { get; set; }

    public virtual Aluno? Aluno { get; set; }

    public virtual Professore? Professore { get; set; }
}
