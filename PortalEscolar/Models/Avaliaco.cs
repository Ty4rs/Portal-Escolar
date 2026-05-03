using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Avaliaco
{
    public int IdAvaliacao { get; set; }

    public int IdMatriculaMateria { get; set; }

    public decimal NotaAvaliacao { get; set; }

    public DateOnly DataAvaliacao { get; set; }

    public string? Descricao { get; set; }

    public virtual MatriculasMateria IdMatriculaMateriaNavigation { get; set; } = null!;
}
