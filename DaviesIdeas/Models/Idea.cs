using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaviesIdeas.Models
{
    public class Idea
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        //User written post
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
