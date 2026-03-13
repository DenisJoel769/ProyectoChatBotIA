using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebNetOpenIA_v1.Models.Views
{
    public class VentasDetalleView
    {
            [Column("id_ven")]
            public int IdVen { get; set; }
            [Column("id_item")]
            public int IdItem { get; set; }
            [Column("precio")]
            public decimal Precio { get; set; }
            [Column("cantidad")]
            public int Cantidad { get; set; }
            [Column("item_descrip")]
            public string? ItemDescrip { get; set; }
            [Column("id_mar")]
            public int IdMar { get; set; }
            [Column("mar_descrip")]
            public string? MarDescrip { get; set; }
            [Column("item_color")]
            public string? ItemColor { get; set; }
            [Column("estado")]
            public string? Estado { get; set; }
            [Column("ven_fecha")]
            public DateTime FechaVenta { get; set; }

    }
}
