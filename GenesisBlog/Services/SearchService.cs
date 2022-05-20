using GenesisBlog.Data;
using GenesisBlog.Models;
using Microsoft.EntityFrameworkCore;
using GenesisBlog.Enums;

namespace GenesisBlog.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<BlogPost> Search(string searchString)
        {
            var blogPostRepo = new List<BlogPost>().AsQueryable();

            //Finish search method
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return blogPostRepo;
            }

            searchString = searchString.ToLower();
            blogPostRepo = _context.BlogPosts
                                            .Include(c => c.Tags)
                                            .Include(c => c.BlogPostComments)
                                            .ThenInclude(c => c.Author)
                                            .Where(b => b.BlogPostState == BlogPostState.ProductionReady && !b.IsDeleted)
                                            .AsQueryable();

            blogPostRepo = blogPostRepo.Where(b => b.Title.ToLower().Contains(searchString)
                || b.Abstract.ToLower().Contains(searchString)
                || b.Content.ToLower().Contains(searchString)
                || b.BlogPostComments.Any(
                    c => c.Comment.ToLower().Contains(searchString) ||
                    c.Author!.FirstName.ToLower().Contains(searchString) ||
                    c.Author!.LastName.ToLower().Contains(searchString))
                || b.Tags.Any(t => t.Text.ToLower().Contains(searchString)));

            return blogPostRepo.OrderByDescending(b => b.Created);
        }
    }
}