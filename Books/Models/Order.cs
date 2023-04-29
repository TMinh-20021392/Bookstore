using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        [ForeignKey("BookId")]
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime Bought { get; set; } = DateTime.Now;
    }
}
