using LuxuryVision.Data;
using LuxuryVision.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LuxuryVision.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogPosts.OrderByDescending(b => b.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> BlogForm(int? id)
        {
            var post = id.HasValue ? await _context.BlogPosts.FindAsync(id) : new BlogPost();
            if (id.HasValue && post == null) return NotFound();
            ViewBag.Title = id.HasValue ? "تعديل المقال" : "إضافة مقال جديد";
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogForm(BlogPost post, Microsoft.AspNetCore.Http.IFormFile ImageFile)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImageFile");
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/blog");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    post.ImageUrl = "/images/blog/" + uniqueFileName;
                }

                if (post.Id == 0)
                {
                    post.CreatedAt = DateTime.Now;

                    if (string.IsNullOrEmpty(post.Author))
                    {
                        post.Author = User.Identity.Name ?? "Admin";
                    }

                    _context.BlogPosts.Add(post);
                }
                else
                {
                    if (string.IsNullOrEmpty(post.Author))
                    {
                       
                        post.Author = User.Identity.Name ?? "Admin";
                    }
                    _context.BlogPosts.Update(post);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}