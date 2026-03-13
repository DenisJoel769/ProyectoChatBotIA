using System.ComponentModel.DataAnnotations.Schema;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace AppWebNetOpenIA_v1.Models.Views
{
    public class ComprasDetalleView
    {
        [Column("id_item")]
        public int Id_Item { get; set; }
        [Column("cantidad")]
        public int Cantidad { get; set; }
        [Column("precio")]
        public int Precio { get; set; }
        [Column("estado")]
        public string? Estado { get; set; }
        [Column("item_descrip")]
        public string? Item_Descripcion { get; set; }
        [Column("precio_compra")]
        public int Precio_Compra { get; set; }
        [Column("precio_venta")]
        public int Precio_Venta { get; set; }
        [Column("mar_descrip")]
        public string? Marca_Descripcion { get; set; }
        [Column("ped_fecha")]
        public DateTime Fecha { get; set; }
    }
}
