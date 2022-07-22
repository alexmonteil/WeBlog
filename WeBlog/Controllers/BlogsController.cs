using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeBlog.Data;
using WeBlog.Models;
using WeBlog.Services.Interfaces;

namespace WeBlog.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;

        public BlogsController(ApplicationDbContext context, IImageService imageService, UserManager<BlogUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: Blogs
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
            var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];

            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["MainText"] = "Blog Index";
            ViewData["SubText"] = "A List of all blogs";

            var blogs = _context.Blogs.Include(b => b.BlogUser);
            return View(await blogs.ToListAsync());
        }

        // GET: Blogs/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            if (blog.ImageData is not null)
            {
                ViewData["HeaderImage"] = _imageService.DecodeImage(blog.ImageData, blog.ContentType);
            }
            else
            {
                var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
                var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];
                ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            }

            ViewData["MainText"] = "Blog Details";
            ViewData["SubText"] = "The in-depth content";

            return View(blog);
        }

        // GET: Blogs/Create
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
            var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];

            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["MainText"] = "Create Blog";
            ViewData["SubText"] = "Add content to people's lives";

            return View();
        }


        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Name,Description,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Created = DateTime.UtcNow;
                blog.BlogUserId = _userManager.GetUserId(User);
                blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                blog.ContentType = _imageService.ContentType(blog.Image);
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
            var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];
            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["MainText"] = "Create Blog";
            ViewData["SubText"] = "Add content to people's lives";

            return View(blog);
        }


        // GET: Blogs/CreateFromIndex
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateFromIndex()
        {
            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
            var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];

            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["MainText"] = "Create Blog";
            ViewData["SubText"] = "Add content to people's lives";

            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateFromIndex([Bind("Name,Description,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Created = DateTime.UtcNow;
                blog.BlogUserId = _userManager.GetUserId(User);
                blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                blog.ContentType = _imageService.ContentType(blog.Image);
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
            var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];
            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["MainText"] = "Create Blog";
            ViewData["SubText"] = "Add content to people's lives";

            return View(blog);
        }

        // GET: Blogs/Edit/
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            if (blog.ImageData is not null)
            {
                ViewData["HeaderImage"] = _imageService.DecodeImage(blog.ImageData, blog.ContentType);
            }
            else
            {
                var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
                var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];
                ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            }

            ViewData["MainText"] = "Edit Blog";
            ViewData["SubText"] = "Change the content you are sharing";

            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogUserId,Name,Description,Created,Image,ImageData,ContentType")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (blog.Image != null)
                {
                    blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                    blog.ContentType = _imageService.ContentType(blog.Image);
                }

                try
                {
                    blog.Created = blog.Created.ToUniversalTime();
                    blog.Updated = DateTime.UtcNow;
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }

            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
            var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];
            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["MainText"] = "Edit Blog";
            ViewData["SubText"] = "Change the content you are sharing";

            return View(blog);
        }


        // GET: Blogs/EditFromIndex/
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditFromIndex(int? id)
        {
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            if (blog.ImageData is not null)
            {
                ViewData["HeaderImage"] = _imageService.DecodeImage(blog.ImageData, blog.ContentType);
            }
            else
            {
                var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
                var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];
                ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            }

            ViewData["MainText"] = "Edit Blog";
            ViewData["SubText"] = "Change the content you are sharing";

            return View(blog);
        }


        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditFromIndex(int id, [Bind("Id,BlogUserId,Name,Description,Created,Image,ImageData,ContentType")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (blog.Image != null)
                {
                    blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                    blog.ContentType = _imageService.ContentType(blog.Image);
                }

                try
                {
                    blog.Created = blog.Created.ToUniversalTime();
                    blog.Updated = DateTime.UtcNow;
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
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

            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultBlogImage"]);
            var defaultContentType = _configuration["DefaultBlogImage"].Split(".")[1];
            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["MainText"] = "Edit Blog";
            ViewData["SubText"] = "Change the content you are sharing";

            return View(blog);
        }


        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Blogs'  is null.");
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // POST: Blogs/DeleteFromIndex/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteFromIndex(int id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Blogs'  is null.");
            }
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
          return (_context.Blogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
