using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebNetOpenIA_v1.Models.Views
{
    public class StockView
    {
        [Column("cantidad")]
        public int Cantidad { get; set; }
        [Column("item_descrip")]
        public string? NombreProducto { get; set; }
        [Column("precio_compra")]
        public double PrecioCompra { get; set; }
        [Column("precio_venta")]
        public double PrecioVenta { get; set; }
        [Column("mar_descrip")]
        public string? Marca { get; set; }
    }
}
