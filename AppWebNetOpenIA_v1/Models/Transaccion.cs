namespace AppWebNetOpenIA_v1.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        public int ClienteId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal MontoTotal { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
    }
}
