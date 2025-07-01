namespace ProyectoFinal_Santiago_Giustolisi.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        public string? NombreCategoria { get; set; }

        public ICollection<StockProducto> productos { get; set; } = new List<StockProducto>();
    }
}
