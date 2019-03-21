using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel productsVM { get; set; }
        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            productsVM = new ProductsViewModel()
            {
                productTypes = _db.ProductTypes.ToList(),
                specialTags = _db.SpecialTags.ToList(),
                products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags);
            return View(await products.ToListAsync());
        }



        //Get: Product create
        public IActionResult Create()
        {
            return View(productsVM);
        }


        //Post: Product create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(productsVM);
            }
            _db.Products.Add(productsVM.products);
            await _db.SaveChangesAsync();

            //Image being saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productsFromDb = _db.Products.Find(productsVM.products.Id);

            if(files.Count!=0)
            {
                //Image has been uploded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extention = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads, productsVM.products.Id + extention), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsVM.products.Id + extention;
            }
            else
            {
                // When user not upload image
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultImageProduct);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + productsVM.products.Id + ".png");

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + productsVM.products.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        //Get : Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            productsVM.products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if(productsVM.products == null)
            {
                return NotFound();
            }

            return View(productsVM);
        }

        //Post: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _db.Products.Where(m => m.Id == productsVM.products.Id).FirstOrDefault();

                if(files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extention_new = Path.GetExtension(files[0].FileName);
                    var extention_old = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, productsVM.products.Id + extention_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, productsVM.products.Id + extention_old));
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploads, productsVM.products.Id + extention_new), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productsVM.products.Image = @"\" + SD.ImageFolder + @"\" + productsVM.products.Id + extention_new;
                }

                if(productsVM.products.Image != null)
                {
                    productFromDb.Image = productsVM.products.Image;
                }

                productFromDb.Name = productsVM.products.Name;
                productFromDb.Price = productsVM.products.Price;
                productFromDb.Availability = productsVM.products.Availability;
                productFromDb.ProductTypeId = productsVM.products.ProductTypeId;
                productFromDb.SpecialTagsId = productsVM.products.SpecialTagsId;
                productFromDb.shadeColor = productsVM.products.shadeColor;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productsVM);
        }



        //Get Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            productsVM.products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.products == null)
            {
                return NotFound();
            }

            return View(productsVM);
        }


        //Get Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            productsVM.products = await _db.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (productsVM.products == null)
            {
                return NotFound();
            }

            return View(productsVM);
        }


        //Post Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath; 
            var product = await _db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(product.Image);

                if(System.IO.File.Exists(Path.Combine(uploads,product.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, product.Id + extension));
                }
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}