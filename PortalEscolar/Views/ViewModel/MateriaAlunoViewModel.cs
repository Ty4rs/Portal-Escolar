namespace PortalEscolar.Views.ViewModel;

public class MateriaAlunoViewModel
{

    public int IdMatriculaMateria { get; set; }
    public int IdMateriaPeriodo { get; set; }
    public int IdMateria { get; set; }

    public string NomeMateria { get; set; } = null!;
    public int CargaHoraria { get; set; }
    public string Sala { get; set; } = null!;
    public string NomeProfessor { get; set; } = null;
    public List<MateriaAlunoViewModel> Materias { get; set; } = new();
}