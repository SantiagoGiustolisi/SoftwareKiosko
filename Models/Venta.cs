namespace ProyectoFinal_Santiago_Giustolisi.Models
{
    public class Venta
    {
        public int Id { get; set; }

        public int productoId { get; set; }

        public int Cantidad { get; set; }

        public int precioIngreso { get; set; }

        public int precioEgreso { get; set; }

        public int Margen { get; set; }

        public string? Cliente { get; set; }

        public DateTime fechaVenta { get; set; }

        public StockProducto? stockProductos { get; set; }
    }
}
