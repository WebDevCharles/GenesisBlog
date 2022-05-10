using System.ComponentModel.DataAnnotations;

namespace GenesisBlog.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;
        public string? Description { get; set; }
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

    }
}
