#nullable disable
using GenesisBlog.Data;
using GenesisBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GenesisBlog.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class BlogPostCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public BlogPostCommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BlogPostComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlogPostComment.Include(b => b.BlogPost);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPostComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComment
                .Include(b => b.BlogPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPostComment == null)
            {
                return NotFound();
            }

            return View(blogPostComment);
        }

        // GET: BlogPostComments/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["BlogPostId"] = new SelectList(_context.BlogPosts, "Id", "Abstract");
            return View();
        }

        // POST: BlogPostComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogPostId,Comment")] BlogPostComment blogPostComment)
        {
            if (ModelState.IsValid)
            {
                blogPostComment.Created = DateTime.UtcNow;
                blogPostComment.AuthorId = _userManager.GetUserId(User);

                _context.Add(blogPostComment);
                await _context.SaveChangesAsync();

                var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(b => b.Id == blogPostComment.BlogPostId);
                return RedirectToAction("Details", "BlogPosts", new { slug = blogPost.Slug }, "ScrollTo");
            }
            ViewData["BlogPostId"] = new SelectList(_context.BlogPosts, "Id", "Abstract", blogPostComment.BlogPostId);
            return View(blogPostComment);
        }

        //// GET: BlogPostComments/Edit/5
        //[Authorize]
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var blogPostComment = await _context.BlogPostComment.FindAsync(id);
        //    if (blogPostComment == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["BlogPostId"] = new SelectList(_context.BlogPosts, "Id", "Abstract", blogPostComment.BlogPostId);
        //    return View(blogPostComment);
        //}

        // POST: BlogPostComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogPostId,Comment")] BlogPostComment blogPostComment)
        {
            if (id != blogPostComment.Id)
            {
                return NotFound();
            }

            try
            {
                var existingComment = await _context.BlogPostComment.FindAsync(blogPostComment.Id);
                existingComment.Comment = blogPostComment.Comment;
                existingComment.Updated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(b => b.Id == existingComment.BlogPostId);

                return RedirectToAction("Details", "BlogPosts", new { slug = blogPost!.Slug }, "ScrollTo");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostCommentExists(blogPostComment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Moderate(int id, [Bind("Id,ModReason,ModeratedComment")] BlogPostComment blogPostComment)
        {
            if (id != blogPostComment.Id)
            {
                return NotFound();
            }

            try
            {
                var existingComment = await _context.BlogPostComment.FindAsync(blogPostComment.Id);
                existingComment.ModeratedComment = blogPostComment.ModeratedComment;
                existingComment.ModReason = blogPostComment.ModReason;
                existingComment.Moderated = DateTime.UtcNow;
                existingComment.ModeratorId = _userManager.GetUserId(User);

                await _context.SaveChangesAsync();

                var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(b => b.Id == existingComment.BlogPostId);
                return RedirectToAction("Details", "BlogPosts", new { slug = blogPost.Slug }, "ScrollTo");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostCommentExists(blogPostComment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
        }

        // GET: BlogPostComments/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPostComment = await _context.BlogPostComment
                .Include(b => b.BlogPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPostComment == null)
            {
                return NotFound();
            }

            return View(blogPostComment);
        }


        // POST: BlogPostComments/Delete/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.BlogPostComment.FindAsync(id);
            _context.BlogPostComment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id, string slug)
        {
            var comment = await _context.BlogPostComment.FindAsync(id);
            comment.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "BlogPosts", new { slug }, "ScrollTo");
        }

        private bool BlogPostCommentExists(int id)
        {
            return _context.BlogPostComment.Any(e => e.Id == id);
        }
    }
}
