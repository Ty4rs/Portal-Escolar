namespace PortalEscolar.Views.ViewModel
{
    public class BoletimAlunoViewModel
    {
        public string NomeMateria { get; set; } = null!;
        public List<decimal> Notas { get; set; } = new();
        public decimal Media { get; set; }
        public int AulasMinistradas { get; set; }
        public int TotalFaltas { get; set; }
        public double PorcentagemFrequencia { get; set; }
        public string Situacao { get; set; } = null!;

        public List<BoletimAlunoViewModel> ListaBoletim { get; set; } = new();

    }
}
