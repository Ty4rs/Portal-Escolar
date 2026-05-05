namespace PortalEscolar.Views.ViewModel
{
    public class AlunoFrequenciaResumo
    {
        public string NomeAluno { get; set; } = null!;
        public int TotalPresencas { get; set; }
        public int TotalFaltas { get; set; }
        public double PorcentagemPresenca { get; set; }
    }

    public class RelatorioFrequenciaViewModel
    {
        public string NomeMateria { get; set; } = null!;
        public List<AlunoFrequenciaResumo> ResumoAlunos { get; set; } = new();
    }
}
