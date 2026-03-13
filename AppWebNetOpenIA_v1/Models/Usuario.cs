using System.ComponentModel.DataAnnotations;

namespace AppWebNetOpenIA_v1.Models
{
    public class Usuario
    {
        [Key]   // Marca que esta propiedad es la Primary Key
        public int id_usu { get; set; }
        public string usu_nombre { get; set; }
        public string estado { get; set; }
    }
}
