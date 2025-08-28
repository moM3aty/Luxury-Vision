using LuxuryVision.Data;
using LuxuryVision.Models;
using LuxuryVision.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LuxuryVision.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                ProductCount = await _context.Products.CountAsync(),
                CategoryCount = await _context.Categories.CountAsync(),
                ShippingZoneCount = await _context.ShippingZones.CountAsync(),
                OrderCount = await _context.Orders.CountAsync(),
                RecentOrders = await _context.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesData()
        {
            var salesData = await _context.Orders
                .Where(o => o.OrderDate.Year == DateTime.Now.Year && o.OrderStatus != "ملغي" && o.OrderStatus != "بانتظار الدفع")
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .OrderBy(x => x.Month)
                .ToListAsync();

            var labels = Enumerable.Range(1, 12).Select(m => new DateTime(DateTime.Now.Year, m, 1).ToString("MMMM", new CultureInfo("ar-SA"))).ToArray();
            var data = new decimal[12];

            foreach (var item in salesData)
            {
                data[item.Month - 1] = item.Total;
            }

            return Json(new { labels, data });
        }
        [HttpGet]
        public async Task<IActionResult> GetCategoryDistributionData()
        {
            var categoryData = await _context.Categories
                .Include(c => c.Products)
                .Select(c => new { Name = c.Name, ProductCount = c.Products.Count() })
                .ToListAsync();

            var labels = categoryData.Select(c => c.Name).ToArray();
            var data = categoryData.Select(c => c.ProductCount).ToArray();

            return Json(new { labels, data });
        }
        #region Product Management
        public async Task<IActionResult> Products() => View(await _context.Products.Include(p => p.Category).ToListAsync());

        public async Task<IActionResult> ProductForm(int? id)
        {
            var viewModel = new ProductFormViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                Product = id.HasValue ? await _context.Products.FindAsync(id) : new Product()
            };
            if (id.HasValue && viewModel.Product == null) return NotFound();
            ViewBag.Title = id.HasValue ? "تعديل المنتج" : "إضافة منتج جديد";
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductForm(ProductFormViewModel viewModel)
        {
            ModelState.Remove("Product.ImageUrl");
            ModelState.Remove("Product.Category");
            ModelState.Remove("Categories");
            if (viewModel.Product.Id != 0)
            {
                ModelState.Remove("ImageFile");
            }
    
            if (!ModelState.IsValid)
            {
                viewModel.Categories = await _context.Categories.ToListAsync();
                return View(viewModel);
            }

            if (viewModel.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ImageFile.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(viewModel.Product.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, viewModel.Product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                viewModel.Product.ImageUrl = "/images/products/" + uniqueFileName;
            }

            if (viewModel.Product.Id == 0) 
            {
                _context.Products.Add(viewModel.Product);
            }
            else 
            {
                _context.Products.Update(viewModel.Product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Products));
        }
        #endregion

        #region Category Management
        public async Task<IActionResult> Categories() => View(await _context.Categories.Include(c => c.Products).ToListAsync());

        public async Task<IActionResult> CategoryForm(int? id)
        {
            var viewModel = new CategoryFormViewModel
            {
                Category = id.HasValue ? await _context.Categories.FindAsync(id) : new Category()
            };

            if (id.HasValue && viewModel.Category == null) return NotFound();
            ViewBag.Title = id.HasValue ? "تعديل القسم" : "إضافة قسم جديد";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryForm(CategoryFormViewModel viewModel)
        {
            ModelState.Remove("Category.ImageUrl");
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string uniqueFileName = null;
            if (viewModel.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/categories");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ImageFile.CopyToAsync(fileStream);
                }
            }

            if (viewModel.Category.Id == 0) 
            {
                if (uniqueFileName != null)
                {
                    viewModel.Category.ImageUrl = "/images/categories/" + uniqueFileName;
                }
                _context.Categories.Add(viewModel.Category);
            }
            else 
            {
                var categoryInDb = await _context.Categories.FindAsync(viewModel.Category.Id);
                if (categoryInDb == null) return NotFound();

                categoryInDb.Name = viewModel.Category.Name;

                if (uniqueFileName != null)
                {
                    if (!string.IsNullOrEmpty(categoryInDb.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, categoryInDb.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    categoryInDb.ImageUrl = "/images/categories/" + uniqueFileName;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category != null && !category.Products.Any())
            {
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, category.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Categories));
        }
        #endregion

        #region Shipping Zone Management
        public async Task<IActionResult> ShippingZones() => View(await _context.ShippingZones.ToListAsync());

        public async Task<IActionResult> ShippingZoneForm(int? id)
        {
            var zone = id.HasValue ? await _context.ShippingZones.FindAsync(id) : new ShippingZone();
            if (id.HasValue && zone == null) return NotFound();
            ViewBag.Title = id.HasValue ? "تعديل منطقة الشحن" : "إضافة منطقة شحن";
            return View(zone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShippingZoneForm(ShippingZone zone)
        {
            if (ModelState.IsValid)
            {
                if (zone.Id == 0) _context.ShippingZones.Add(zone);
                else _context.ShippingZones.Update(zone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ShippingZones));
            }
            return View(zone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShippingZone(int id)
        {
            var zone = await _context.ShippingZones.FindAsync(id);
            if (zone != null)
            {
                _context.ShippingZones.Remove(zone);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ShippingZones));
        }
        #endregion

        #region Order Management
        public async Task<IActionResult> Orders() => View(await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync());

        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null) return NotFound();
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null && !string.IsNullOrEmpty(status))
            {
                order.OrderStatus = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(OrderDetails), new { id = orderId });
        }
        #endregion
    }
}
