namespace PortalEscolar.Views.ViewModel.Admin.Painel
{
    public class DesempenhoCursoViewModel
    {
        public string NomeCurso { get; set; }
        public decimal MediaGeral { get; set; }
        public int TotalAlunos { get; set; }
    }
    public class MateriaRankingReprovacaoViewModel
    {
        public string NomeMateria { get; set; }
        public int QuantidadeReprovados { get; set; }
        public string NomeProfessor { get; set; }
    }
    public class  QuantidadeUsuariosViewModel
    {
        public int Usuariosc { get; set; }
        public int ProfessoresC { get; set; }
        public int Alunosc { get; set; }
    }
    public class ExcluirCursoViewModel
    {
        public int IdCurso { get; set; }
        public string NomeCurso { get; set; }
        public int TotalMatriculas { get; set; }
        public int TotalMaterias { get; set; }
    }

}
