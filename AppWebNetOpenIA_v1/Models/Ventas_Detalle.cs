using System.ComponentModel.DataAnnotations;

namespace AppWebNetOpenIA_v1.Models
{
    public class Ventas_Detalle
    {
        [Key]
        public int Id_venta { get; set; }
        public int Id_item { get; set; }
        public decimal precio { get; set; }
        public int cantidad { get; set; }

    }
}
