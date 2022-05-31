using GenesisBlog.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenesisBlog.Models
{

    /// <summary>
    ///     This Class processes the requests for Blog Posts.
    /// </summary>
    [Display(Name = "Blog Post")]
    public class BlogPost
    {
        /// <summary>
        ///     This is the PK of a Blog Post record.
        /// </summary>
        public int Id { get; set; } // This turns into an auto-incrementing integer

        /// <summary>
        ///     This serves as the Blog Post Title.
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Title { get; set; } = "";

        /// <summary>
        ///     This is a synopsis of the Blog Post to entice the reader.
        /// </summary>
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Abstract { get; set; } = "";

        /// <summary>
        ///     This is the the actual Blog Post.
        /// </summary>
        [Required]
        public string Content { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }

        // This property is derived from the Title.
        // This will eventually be used in some cases INSTEAD of
        // the Primary Key (Id)
        public string Slug { get; set; } = "";

        public bool IsDeleted { get; set; }


        [Display(Name = "Blog Post State")]
        public BlogPostState BlogPostState { get; set; }


        // What if I wanted to record an image with a blog post
        [Display(Name = "Image")]
        public byte[] ImageData { get; set; } = Array.Empty<byte>();


        [Display(Name = "Image Type")]
        public string ImageType { get; set; } = String.Empty;


        [NotMapped]
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }


        // Naigational Properties
        public virtual ICollection<BlogPostComment> BlogPostComments { get; set; } = new HashSet<BlogPostComment>();
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

    }
}
