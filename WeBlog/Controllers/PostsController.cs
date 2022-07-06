﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeBlog.Data;
using WeBlog.Models;
using WeBlog.Services.Interfaces;

namespace WeBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;

        public PostsController(ApplicationDbContext context, ISlugService slugService, IImageService imageService, UserManager<BlogUser> userManager)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog).Include(p => p.BlogUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // BlogPostIndex
        public async Task<IActionResult> BlogPostIndex(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var posts = _context.Posts.Where(p => p.BlogId == id).ToList();

            return View("Index", posts);
        }

        // Post Details
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                if (post.Image != null)
                {
                    post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                    post.ContentType = _imageService.ContentType(post.Image);
                }

                post.Created = DateTime.UtcNow;
                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;
                var slug = _slugService.UrlFriendly(post.Title);
                bool validationError = false;

                // Detect null or empty slug
                if (string.IsNullOrEmpty(slug))
                {
                    ModelState.AddModelError("", "The title you provided cannot be used as it results in an empty slug");
                    validationError = true;
                }

                // Detect duplicate slug
                if (!_slugService.IsUnique(slug))
                {
                    ModelState.AddModelError("Title", "The title you provided cannot be used as it results in a duplicate slug");
                    validationError = true;
                }

                // if validation errors exist, return necessary data to the view
                if (validationError)
                {
                    ViewData["TagValues"] = string.Join(",", tagValues);
                    return View(post);
                }

                post.Slug = slug;
                _context.Add(post);
                await _context.SaveChangesAsync();

                foreach (string tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    var newPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
                    newPost.Updated = DateTime.UtcNow;
                    newPost.Title = post.Title;
                    newPost.Abstract = post.Abstract;
                    newPost.Content = post.Content;
                    newPost.ReadyStatus = post.ReadyStatus;

                    var newSlug = _slugService.UrlFriendly(post.Title);
                    bool validationError = false;

                    // Check for slug change
                    if (newSlug != newPost.Slug)
                    {
                        // Detect null or empty slug
                        if (string.IsNullOrEmpty(newSlug))
                        {
                            ModelState.AddModelError("", "The title you provided cannot be used as it results in an empty slug");
                            validationError = true;
                        }

                        // Detect duplicate slug
                        if (!_slugService.IsUnique(newSlug))
                        {
                            ModelState.AddModelError("Title", "The title you provided cannot be used as it results in a duplicate slug");
                            validationError = true;
                        }

                        // if validation errors exist, return necessary data to the view
                        if (validationError)
                        {
                            ViewData["TagValues"] = string.Join(",", tagValues);
                            return View(post);
                        }

                        newPost.Title = post.Title;
                        newPost.Slug = newSlug;
                    }

                    if (post.Image is not null)
                    {
                        newPost.ImageData = await _imageService.EncodeImageAsync(post.Image);
                        newPost.ContentType = _imageService.ContentType(post.Image);
                    }

                    // Remove all tags previously associated with this post
                    _context.Tags.RemoveRange(newPost.Tags);

                    // Add the new tags from the edit form
                    foreach(var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = newPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);

            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
