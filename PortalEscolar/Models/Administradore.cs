using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Administradore
{
    public int IdAdmin { get; set; }

    public int IdUsuario { get; set; }

    public string Nome { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
