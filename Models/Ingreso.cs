namespace ProyectoFinal_Santiago_Giustolisi.Models
{
    public class Ingreso
    {
        public int Id { get; set; }

        public int productoId { get; set; }

        public int Cantidad { get; set; }

        public DateTime fechaIngreso { get; set; }

        public StockProducto? stockProductos { get; set; }
    }
}
