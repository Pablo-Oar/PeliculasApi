using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    //Entidad categoria model --> a partir de esto se pueden crear muchos DTOs para no exponer el model directamente.
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
    }
}
