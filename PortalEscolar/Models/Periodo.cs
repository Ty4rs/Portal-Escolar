using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Periodo
{
    public int IdPeriodo { get; set; }

    public string Nome { get; set; } = null!;

    public int Ano { get; set; }

    public int Semestre { get; set; }

    public virtual ICollection<MateriasPeriodo> MateriasPeriodos { get; set; } = new List<MateriasPeriodo>();
}
