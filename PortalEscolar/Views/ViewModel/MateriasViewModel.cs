using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalEscolar.Views.ViewModel
{
    public class MateriasViewModel
    {
        public SelectList materias { get; set; }
        public SelectList periodos { get; set; }
        public SelectList cursos { get; set; }
        public int idMateria { get; set; }
        public string Sala { get; set; }
        public int IdPeriodo { get; set; }
        public int _idCurso { get; set; }


    }
}
