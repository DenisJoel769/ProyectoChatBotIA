using System.ComponentModel.DataAnnotations;

namespace AppWebNetOpenIA_v1.Models
{
    public class Items
    {
        [Key]
        public int Id_item { get; set; }
        public int Id_mar { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio_compra { get; set; }
        public decimal Precio_venta { get; set; }

    }
}
