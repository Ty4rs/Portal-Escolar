using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Frequencia
{
    public int IdFrequencia { get; set; }

    public int IdMatriculaMateria { get; set; }

    public DateOnly DataFrequencia { get; set; }

    public TimeOnly HoraFrequencia { get; set; }

    public bool Presenca { get; set; }

    public virtual MatriculasMateria IdMatriculaMateriaNavigation { get; set; } = null!;
}
