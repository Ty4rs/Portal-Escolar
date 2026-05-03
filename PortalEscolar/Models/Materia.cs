using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Materia
{
    public int IdMateria { get; set; }

    public string Nome { get; set; } = null!;

    public int Cargahoraria { get; set; }

    public virtual ICollection<MateriasPeriodo> MateriasPeriodos { get; set; } = new List<MateriasPeriodo>();
}
