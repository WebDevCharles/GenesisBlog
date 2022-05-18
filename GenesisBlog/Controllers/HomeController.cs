using GenesisBlog.Data;
using GenesisBlog.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace GenesisBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailService;
        private readonly IConfiguration _appSettings;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IEmailSender emailService, IConfiguration appSettings)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            var blogPosts = _context.BlogPosts.ToList();
            return View(blogPosts);
        }

        public IActionResult ContactMe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(string name, string email, string phone, string subject, string message)
        {
            var myEmail = _appSettings["SmtpSettings:UserName"];

            //Incorporate as much information as possible into the body of the email
            StringBuilder _builder = new(message);
            _builder.AppendLine("<br><br>");
            _builder.AppendLine($"Sender's Information <br>");
            _builder.AppendLine($"Name: {name} <br>");
            _builder.AppendLine($"Email: {email} <br>");
            _builder.AppendLine($"Phone: {phone}");

            await _emailService.SendEmailAsync(myEmail, subject, _builder.ToString());
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}