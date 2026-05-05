using PortalEscolar.Models;

namespace PortalEscolar.Views.ViewModel
{
    public class FrequenciaLinhaViewModel
    {
        // ID necessário para a tabela frequencias
        public int IdMatriculaMateria { get; set; }
        public string NomeAluno { get; set; } = null!;
        public bool Presenca { get; set; }
    }

    public class FrequenciaPaginaViewModel
    {
        public int IdMateriaPeriodo { get; set; }
        public string NomeMateria { get; set; } = null!;
        public DateTime Data { get; set; } = DateTime.Now;
        public List<FrequenciaLinhaViewModel> Alunos { get; set; } = new();
    }

}
