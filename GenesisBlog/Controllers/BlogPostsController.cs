#nullable disable
using GenesisBlog.Data;
using GenesisBlog.Enums;
using GenesisBlog.Models;
using GenesisBlog.Services.Interfaces;
using GenesisBlog.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace GenesisBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public BlogPostsController(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            //var posts = await _context.BlogPost.ToListAsync();
            var posts = await _context.BlogPosts
                                                  .Include(b => b.Tags)
                                                  .ToListAsync();

            return View(posts);
        }

        public async Task<IActionResult> InDevIndex()
        {
            var posts = await _context.BlogPosts
                                .Include(b => b.Tags)
                                .Where(b => b.BlogPostState == BlogPostState.InDevelopment)
                                .ToListAsync();

            return View("Index", posts);
        }

        public async Task<IActionResult> InPreviewIndex()
        {
            var posts = await _context.BlogPosts
                                .Include(b => b.Tags)
                                .Where(b => b.BlogPostState == BlogPostState.InPreview)
                                .ToListAsync();

            return View("Index", posts);
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Tags)
                .Include(b => b.BlogPostComments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            BlogPost blogPost = new BlogPost();
            ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text");

            return View(blogPost);
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Abstract,Content,ImageFile,ImageType,ImageData,BlogPostState")] BlogPost blogPost, List<int> tagIds)
        {
            if (ModelState.IsValid)
            {
                //Create a variable named slug and use the new SlugService....
                SlugService slugSvc = new();
                var slug = slugSvc.URLFriendly(blogPost.Title);

                if (_context.BlogPosts.Any(b => b.Slug == slug))
                {
                    ModelState.AddModelError("Title", "The Title must be changed, because it has already been used.");
                    ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagIds);
                    return View(blogPost);
                }
                else
                {
                    blogPost.Slug = slug;
                }

                //Before I try interacting with the IFormFile
                //I should make sure it's present
                if (blogPost.ImageFile is not null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }
                ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagIds);

                //Associate any/all selected tags with the BlogPost
                foreach (var tagId in tagIds)
                {
                    blogPost.Tags.Add(await _context.Tag.FindAsync(tagId));
                }

                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                                                      .Include(b => b.Tags)
                                                      .FirstOrDefaultAsync(b => b.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            //4th parameter in a multiSelect list is a List<int> representing the current selection
            var tagPks = blogPost.Tags.Select(b => b.Id).ToList();
            ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagPks);

            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,BlogPostState,ImageFile")] BlogPost blogPost, List<int> tagIds)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    //This code gets whats known as a "Tracked entity". By default, most LINQ
                    //statement that pull data from a DB are tracked. In fact, if you don't it tracked
                    //you have to use extra code to tell it so.. AsNoTracking()
                    var existingPost = await _context.BlogPosts
                                                            .Include(b => b.Tags)
                                                            .FirstOrDefaultAsync(b => b.Id == blogPost.Id);

                    SlugService slugSvc = new();
                    var newSlug = slugSvc.URLFriendly(blogPost.Title);
                    if (newSlug != existingPost.Slug)
                    {
                        if (_context.BlogPosts.Any(b => b.Slug == newSlug))
                        {
                            ModelState.AddModelError("Title", "The Title must be changed, because it has already been used.");
                            ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagIds);
                            return View(blogPost);
                        }
                    }

                    if (blogPost.ImageFile is not null)
                    {
                        existingPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        existingPost.ImageType = blogPost.ImageFile.ContentType;
                    }


                    existingPost.Tags.Clear();
                    await _context.SaveChangesAsync();

                    //Continue on making the requested user changes
                    //Since I already have a tracked entity named existingPost I will
                    //copy over the incoming form values
                    existingPost.Slug = newSlug;
                    existingPost.Updated = DateTime.UtcNow;
                    existingPost.Title = blogPost.Title;
                    existingPost.Abstract = blogPost.Abstract;
                    existingPost.Content = blogPost.Content;
                    existingPost.BlogPostState = blogPost.BlogPostState;

                    //Add all the selected tags back
                    foreach (var tagId in tagIds)
                    {
                        existingPost.Tags.Add(await _context.Tag.FindAsync(tagId));
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["TagIds"] = new MultiSelectList(_context.Tag, "Id", "Text", tagIds);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
