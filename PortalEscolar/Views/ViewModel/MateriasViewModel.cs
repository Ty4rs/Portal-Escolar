using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalEscolar.Views.ViewModel
{
    public class MateriasViewModel
    {
        public SelectList materias { get; set; }
        public int idMateria { get; set; }
        public int Sala { get; set; }



    }
}
