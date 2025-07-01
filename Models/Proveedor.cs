namespace ProyectoFinal_Santiago_Giustolisi.Models
{
    public class Proveedor 
    {
        public int Id { get; set; }
        public string? NombreProveedor { get; set; }
        public string? Telefono { get; set; }
        public ICollection<StockProducto> Productos { get; set; } = new List<StockProducto>();
    }
}

