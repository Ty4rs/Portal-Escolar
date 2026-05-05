


using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalEscolar.Views.ViewModel;

public class AlunoViewModel
{
    //public Aluno _aluno { get; set; }
    //public Matricula _matricula { get; set; }
    public SelectList cursos { get; set; }
    public int idUsuario { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Endereco { get; set; }
    public string Telefone { get; set; }
    public int IdCurso { get; set; }


}