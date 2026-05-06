namespace PortalEscolar.Views.ViewModel
{

    public class LancarNotaLinhaViewModel
    {
        public int IdMatriculaMateria { get; set; }
        public string NomeAluno { get; set; } = null!;
        public decimal Nota { get; set; }
    }

    public class LancarNotaPaginaViewModel
    {
        public int IdMateriaPeriodo { get; set; }
        public string NomeMateria { get; set; } = null!;
        public DateTime DataAvaliacao { get; set; } = DateTime.Now;
        public List<LancarNotaLinhaViewModel> Alunos { get; set; } = new();
    }
    public class GerenciarAvaliacoesViewModel
    {
        public DateTime Data { get; set; }
        public string NomeMateria { get; set; } = null!;
        public int IdMateriaPeriodo { get; set; }
        public int TotalAlunos { get; set; }
    }
}
