using GenesisBlog.Enums;
using System.ComponentModel.DataAnnotations;

namespace GenesisBlog.Models
{
    public class BlogPostComment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; } = Guid.Empty.ToString();
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }


        [Required]
        public string Comment { get; set; } = string.Empty;


        public DateTime? Moderated { get; set; }
        public string? ModeratedId { get; set; }
        public string? ModeratedComment { get; set; }
        public ModReason? ModReason { get; set; }
        public bool IsDeleted { get; set; }


        // Navigational Properties
        public virtual BlogPost? BlogPost { get; set; }
        public virtual BlogUser? Author { get; set; }

    }
}
