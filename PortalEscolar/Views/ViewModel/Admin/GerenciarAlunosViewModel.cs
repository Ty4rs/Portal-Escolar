using PortalEscolar.Models;

namespace PortalEscolar.Views.ViewModel.Admin
{
    public class GerenciarAlunosViewModel
    {
        public List<Aluno> Alunos { get; set; } = new();
        public List<Models.Usuario> Usuarios { get; set; } = new();
    }
    public class GerenciarProfessoresViewModel
    {
        public List<Professore> Professores { get; set; } = new();
        public List<Models.Usuario> Usuarios { get; set; } = new();
    }
}
