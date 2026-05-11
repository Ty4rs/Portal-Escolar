using PortalEscolar.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalEscolar.Views.ViewModel
{
    public class MatriculaMateriaViewModel
    {
        public SelectList materiasPeriodos { get; set; }
        
        public int _idMateriaPeriodo { get; set; }

    }
}
