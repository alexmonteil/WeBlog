using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WeBlog.Data;
using WeBlog.Models;
using WeBlog.Services.Interfaces;
using WeBlog.ViewModels;
using X.PagedList;

namespace WeBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IBlogEmailSender emailSender, ApplicationDbContext context, IImageService imageService, IConfiguration configuration)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _imageService = imageService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var defaultImage = await _imageService.EncodeImageAsync(_configuration["DefaultHomePageImage"]);
            var defaultContentType = _configuration["DefaultHomePageImage"].Split(".")[1];

            ViewData["HeaderImage"] = _imageService.DecodeImage(defaultImage, defaultContentType);
            ViewData["HeaderText"] = "(We)b + log = We Blog";
            ViewData["SubText"] = "Sharing with others one line at a time";

            var pageNumber = page ?? 1;
            var pageSize = 5;
            var blogs = _context.Blogs.Include(b => b.BlogUser).Where(b => b.Posts
            .Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady))
            .OrderByDescending(b => b.Created)
            .ToPagedListAsync(pageNumber, pageSize);

            return View(await blogs);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}