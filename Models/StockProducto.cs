namespace ProyectoFinal_Santiago_Giustolisi.Models
{
    public class StockProducto
    {
        public int Id { get; set; }

        public int categoriaId { get; set; }

        public string? Nombre { get; set; }

        public int Cantidad { get; set; }

        public int Precio { get; set; }

        public string? Foto { get; set; }

        public Categoria? categoria { get; set; }

        public ICollection<Venta> ventas { get; set; } = new List<Venta>();

        public ICollection<Ingreso> ingresos { get; set; } = new List<Ingreso>();
    }
}
