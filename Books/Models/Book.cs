using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Models
{
    public class Book
    {
        [Key]
		[ScaffoldColumn(false)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

        // This means Name cannot be null
        [Required]
        public string Name { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }
        public string? Genre { get; set; }
        public byte[]? Image { get; set; }
    }
}
