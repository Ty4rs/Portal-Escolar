namespace PortalEscolar.Views.ViewModel;

public class MateriaProfessorViewModel
{
    public int IdMateriaPeriodo { get; set; }
    public int IdMateria { get; set; }
    public string NomeMateria { get; set; } = null!;
    public int CargaHoraria { get; set; }
    public string Sala { get; set; } = null!;
    public List<MateriaProfessorViewModel> Materias { get; set; } = new();
    public string NomeCurso { get; set; } = null!;
    
}