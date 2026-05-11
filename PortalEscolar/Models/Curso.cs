using System;
using System.Collections.Generic;

namespace PortalEscolar.Models;

public partial class Curso
{
    public int IdCurso { get; set; }

    public string Nome { get; set; } = null!;

    public string Duracao { get; set; } = null!;

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    public virtual ICollection<MateriasPeriodo> MateriasPeriodos { get; set; } = new List<MateriasPeriodo>();
}
