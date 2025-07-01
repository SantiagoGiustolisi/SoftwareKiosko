using ProyectoFinal_Santiago_Giustolisi.Controllers;

namespace ProyectoFinal_Santiago_Giustolisi.Models
{
    public class StockProductoViewModels
    {
        public IEnumerable<StockProducto> productos { get; set; }

        public int PaginaActual { get; set; }

        public int TotalPaginas { get; set; }

        public string? BusquedaNombre { get; set; } 
    }
}
