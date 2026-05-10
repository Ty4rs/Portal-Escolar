using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class MatriculasMateria
{
    public int IdMatriculaMateria { get; set; }

    public int IdMatricula { get; set; }

    public int IdMateriaPeriodo { get; set; }

    public DateOnly DataVinculo { get; set; }

    public string Status { get; set; } = null!;
    public decimal? NotaFinal { get; set; }

    public virtual ICollection<Avaliaco> Avaliacos { get; set; } = new List<Avaliaco>();

    public virtual ICollection<Frequencia> Frequencia { get; set; } = new List<Frequencia>();

    public virtual MateriasPeriodo IdMateriaPeriodoNavigation { get; set; } = null!;

    public virtual Matricula IdMatriculaNavigation { get; set; } = null!;
}
