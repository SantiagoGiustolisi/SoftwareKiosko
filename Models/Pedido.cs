using ProyectoFinal_Santiago_Giustolisi.Models;

namespace ProyectoFinal.Models
{
    public class Pedido 
    {
        public int Id { get; set; }

        public string? Descripcion { get; set; }

        public int ProveedorId { get; set; } 

        public int Costo { get; set; }

        public DateTime? FechaIngreso { get; set; } 

        public Proveedor? Proveedor { get; set; } 
    }
}
