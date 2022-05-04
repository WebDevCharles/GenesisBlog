using System.ComponentModel.DataAnnotations;

namespace GenesisBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; } // This turns into an auto-incrementing integer

        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Title { get; set; } = "";

        [Required]
        public string Abstract { get; set; } = "";

        [Required]
        public string Content { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }


        // Naigational Properties
        public virtual ICollection<BlogPostComment> BlogPostComments { get; set; } = new HashSet<BlogPostComment>();

    }
}
