using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class MateriasPeriodo
{
    public int IdMateriaPeriodo { get; set; }

    public int IdMateria { get; set; }

    public int IdPeriodo { get; set; }

    public int IdProfessor { get; set; }

    public string Sala { get; set; } = null!;
    public bool Concluida { get; set; }

    public virtual Materia IdMateriaNavigation { get; set; } = null!;

    public virtual Periodo IdPeriodoNavigation { get; set; } = null!;

    public virtual Professore IdProfessorNavigation { get; set; } = null!;

    public virtual ICollection<MatriculasMateria> MatriculasMateria { get; set; } = new List<MatriculasMateria>();
}
