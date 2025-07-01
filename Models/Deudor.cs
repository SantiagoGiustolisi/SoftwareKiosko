namespace ProyectoFinal_Santiago_Giustolisi.Models
{
    public class Deudor
    {
        public int Id { get; set; }

        public string? Nombre { get; set; }

        public DateTime fechaVenta { get; set; }

        public string? Descripcion { get; set; }

        public int dineroEntregado { get; set; }

        public int dineroTotal { get; set; }

        public int dineroResto { get; set; }
    }
}
