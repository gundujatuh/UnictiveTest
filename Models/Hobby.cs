using System.ComponentModel.DataAnnotations;

namespace UnictiveTest.Models
{
    public class Hobby
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
