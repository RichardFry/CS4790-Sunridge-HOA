﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunridgeHOA.Models;
using SunridgeHOA.Models.ViewModels;

namespace SunridgeHOA.Areas.Owner.Controllers
{
    [Area("Owner")]
    [Authorize(Roles = "Owner")]
    public class ClassifiedsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public ClassifiedListingViewModel classifiedListingViewModel { get; set; }

        public ClassifiedsController(ApplicationDbContext context, HostingEnvironment hostingEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            classifiedListingViewModel = new ClassifiedListingViewModel()
            {
                ClassifiedListing = new SunridgeHOA.Models.ClassifiedListing(),
                ClassifiedCategory = _context.ClassifiedCategory.ToList(),
                ClassifiedImages = _context.ClassifiedImage.Where(x => x.ClassifiedListingId == classifiedListingViewModel.ClassifiedListing.ClassifiedListingId),
                Owner = _context.Owner.ToList()
            };
        }

        // GET: Classifieds
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> Index()
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

            // Need to redirect if user is not in an admin role
            var roles = await _userManager.GetRolesAsync(identityUser);
            if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin"))
            {
                return RedirectToAction(nameof(MyClassifieds));
            }

            var vmList = new List<ClassifiedListingViewModel>();

            var adminItems = _context.ClassifiedListing.ToList();
            foreach (var listing in adminItems)
            {
                var vm = new ClassifiedListingViewModel()
                {
                    ClassifiedListing = listing,
                    ClassifiedCategory = _context.ClassifiedCategory.ToList(),
                    ClassifiedImages = _context.ClassifiedImage.ToList(),
                    Owner = _context.Owner.ToList()
                };
                vmList.Add(vm);
            }

            return View(vmList);
        }

        public async Task<ActionResult> MyClassifieds()
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

            var vmList = new List<ClassifiedListingViewModel>();

            var ownerItems = _context.ClassifiedListing.Where(x => x.OwnerId == loggedInUser.OwnerId).ToList();
            foreach (var listing in ownerItems)
            {
                var vm = new ClassifiedListingViewModel()
                {
                    ClassifiedListing = listing,
                    ClassifiedCategory = _context.ClassifiedCategory.ToList(),
                    ClassifiedImages = _context.ClassifiedImage.ToList(),
                    Owner = _context.Owner.ToList()
                };
                vmList.Add(vm);
            }
            return View(vmList);
        }

        // GET: Classifieds/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var item = await _context.ClassifiedListing.Include(u => u.ClassifiedCategory).SingleOrDefaultAsync(m => m.ClassifiedListingId == id);
            if (item == null)
            {
                return NotFound();
            }

            // Redirect to specific action for service details
            if (item.ClassifiedCategoryId == 3)
            {
                return RedirectToAction(nameof(ServiceDetails), new { id });
            }

            item.Images = await _context.ClassifiedImage.Where(x => x.ClassifiedListingId == item.ClassifiedListingId).ToListAsync();

            return View(item);
        }

        // GET: Classifieds/Create
        public ActionResult Create()
        {
            ViewData["Category"] = new SelectList(_context.ClassifiedCategory.Where(u => u.Description != "Other"), "ClassifiedCategoryId", "Description");
            return View(new ClassifiedListingViewModel()
            {
                ClassifiedListing = new SunridgeHOA.Models.ClassifiedListing(),
                //ClassifiedCategory = _context.ClassifiedCategory.ToList(),
                ClassifiedImages = _context.ClassifiedImage.Where(x => x.ClassifiedListingId == classifiedListingViewModel.ClassifiedListing.ClassifiedListingId),
            });
        }

        // POST: Classifieds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClassifiedListingViewModel listing)
        {

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(HttpContext.User);
                var loggedInUser = await _context.Owner.FindAsync(identityUser.OwnerId);
                
                classifiedListingViewModel.ClassifiedListing.LastModifiedBy = loggedInUser.FullName;
                classifiedListingViewModel.ClassifiedListing.LastModifiedDate = DateTime.Now;
                classifiedListingViewModel.ClassifiedListing.ListingDate = DateTime.Now;
                classifiedListingViewModel.ClassifiedListing.Owner = loggedInUser;
                classifiedListingViewModel.ClassifiedListing.OwnerId = loggedInUser.OwnerId;

                _context.ClassifiedListing.Add(classifiedListingViewModel.ClassifiedListing);
                await _context.SaveChangesAsync();

                //image uploading
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                //var classifiedListingFromDb = _context.ClassifiedListing.Find(classifiedListingViewModel.ClassifiedListing.ClassifiedListingId);

                var uploads = Path.Combine(webRootPath, @"img\ClassifiedsImages");

                int i = 0;
                foreach (var file in files)
                {
                    i++;
                    var extension = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(uploads, classifiedListingViewModel.ClassifiedListing.ClassifiedListingId + @"_" + i + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream); // moves to server and renames
                    }
                    var image = new ClassifiedImage()
                    {
                        ClassifiedListingId = classifiedListingViewModel.ClassifiedListing.ClassifiedListingId,
                        IsMainImage = (file == files.First()),
                        ImageExtension = extension,
                        ImageURL = @"\" + @"img\ClassifiedsImages" + @"\" + classifiedListingViewModel.ClassifiedListing.ClassifiedListingId + @"_" + i + extension
                    };

                    _context.ClassifiedImage.Add(image);
                }

                
                //classifiedListingFromDb.Images.Add(image);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["Category"] = new SelectList(_context.ClassifiedCategory, "ClassifiedCategoryId", "Description");
            return View(new ClassifiedListingViewModel()
            {
                ClassifiedListing = listing.ClassifiedListing,
                ClassifiedCategory = _context.ClassifiedCategory.ToList(),
                ClassifiedImages = _context.ClassifiedImage.Where(x => x.ClassifiedListingId == classifiedListingViewModel.ClassifiedListing.ClassifiedListingId),
            });
        }

        // GET: Classifieds/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ClassifiedListing.SingleOrDefaultAsync(u => u.ClassifiedListingId == id);

            if (item == null)
            {
                return NotFound();
            }

            // Item is a service
            if (item.ClassifiedCategoryId == 3)
            {
                return RedirectToAction(nameof(EditService), new { id });
            }

            item.Images = await _context.ClassifiedImage.Where(x => x.ClassifiedListingId == item.ClassifiedListingId).ToListAsync();
            classifiedListingViewModel.ClassifiedListing = item;

            ViewData["Category"] = new SelectList(_context.ClassifiedCategory.Where(u => u.Description != "Other"), "ClassifiedCategoryId", "Description", item.ClassifiedCategoryId);

            return View(classifiedListingViewModel);
        }

        // POST: Classifieds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, ClassifiedListing listing)
        {
            if (id != classifiedListingViewModel.ClassifiedListing.ClassifiedListingId)
            {
                return NotFound();
            }

            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedInUser = await _context.Owner.FindAsync(identityUser.OwnerId);

            classifiedListingViewModel.ClassifiedListing.LastModifiedBy = loggedInUser.FullName;
            classifiedListingViewModel.ClassifiedListing.LastModifiedDate = DateTime.Now;
            classifiedListingViewModel.ClassifiedListing.OwnerId = loggedInUser.OwnerId;

            _context.Update(classifiedListingViewModel.ClassifiedListing);
            await _context.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var uploads = Path.Combine(webRootPath, @"img\ClassifiedsImages");
            ClassifiedListing item = await _context.ClassifiedListing.FindAsync(id);
            if (files.Count != 0)
            {
                var oldImages = await _context.ClassifiedImage.Where(x => x.ClassifiedListingId == item.ClassifiedListingId).ToListAsync();
                foreach(var oldImage in oldImages)
                {
                    if (System.IO.File.Exists(Path.Combine(webRootPath, oldImage.ImageURL.Substring(1))))
                    {
                        System.IO.File.Delete(Path.Combine(webRootPath, oldImage.ImageURL.Substring(1)));
                    }
                    _context.ClassifiedImage.Remove(oldImage);
                 }

                int i = 0;
                foreach (var file in files)
                {
                    i++;
                    var extension = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(uploads, classifiedListingViewModel.ClassifiedListing.ClassifiedListingId + @"_" + i + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream); // moves to server and renames
                    }
                    var image = new ClassifiedImage()
                    {
                        ClassifiedListingId = classifiedListingViewModel.ClassifiedListing.ClassifiedListingId,
                        IsMainImage = (file == files.First()),
                        ImageExtension = extension,
                        ImageURL = @"\" + @"img\ClassifiedsImages" + @"\" + classifiedListingViewModel.ClassifiedListing.ClassifiedListingId + @"_" + i + extension
                    };

                    _context.ClassifiedImage.Add(image);
                }
                await _context.SaveChangesAsync();
            }

            ViewData["Category"] = new SelectList(_context.ClassifiedCategory, "ClassifiedCategoryId", "Description", item.ClassifiedCategoryId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ServiceDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _context.ClassifiedListing
                .Include(u => u.Images)
                .Include(u => u.Owner)
                .SingleOrDefaultAsync(u => u.ClassifiedListingId == id);

            return View(listing);
        }

        public IActionResult AddService()
        {
            return View();
        }

        [HttpPost, ActionName("AddService")]
        public async Task<IActionResult> AddServicePOST(string description)
        {
            if (String.IsNullOrEmpty(description))
            {
                ModelState.AddModelError("Description", "Please enter a description");
            }

            var files = HttpContext.Request.Form.Files;
            if (files == null || files.Count == 0)
            {
                ModelState.AddModelError("Files", "Please upload one file");
            }

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(HttpContext.User);
                var loggedInUser = await _context.Owner.FindAsync(identityUser.OwnerId);

                var service = new ClassifiedListing
                {
                    ClassifiedCategoryId = 3, // Manually set to "Other"
                    ItemName = "Service",
                    Description = description,
                    Price = 0,
                    LastModifiedBy = loggedInUser.FullName,
                    LastModifiedDate = DateTime.Now,
                    ListingDate = DateTime.Now,
                    Owner = loggedInUser,
                    OwnerId = loggedInUser.OwnerId
                };

                _context.ClassifiedListing.Add(service);
                await _context.SaveChangesAsync();

                //image uploading
                string webRootPath = _hostingEnvironment.WebRootPath;

                var uploads = Path.Combine(webRootPath, @"img\ClassifiedsImages");

                var file = files[0];
                var extension = Path.GetExtension(file.FileName);
                using (var filestream = new FileStream(Path.Combine(uploads, service.ClassifiedListingId + extension), FileMode.Create))
                {
                    file.CopyTo(filestream); // moves to server and renames
                }
                var image = new ClassifiedImage()
                {
                    ClassifiedListingId = service.ClassifiedListingId,
                    IsMainImage = (file == files.First()),
                    ImageExtension = extension,
                    ImageURL = @"\" + @"img\ClassifiedsImages" + @"\" + service.ClassifiedListingId + extension
                };

                _context.Add(image);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> EditService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ClassifiedListing
                .Include(u => u.Images)
                .Include(u => u.Owner)
                .SingleOrDefaultAsync(u => u.ClassifiedListingId == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(int id, string description)
        {
            if (String.IsNullOrEmpty(description))
            {
                ModelState.AddModelError("Description", "Please enter a description");
            }

            var files = HttpContext.Request.Form.Files;

            var service = await _context.ClassifiedListing
                    .Include(u => u.Images)
                    .SingleOrDefaultAsync(u => u.ClassifiedListingId == id);

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(HttpContext.User);
                var loggedInUser = await _context.Owner.FindAsync(identityUser.OwnerId);
                
                service.Description = description;

                if (files != null && files.Count > 0)
                {
                    var oldImage = service.Images[0];

                    string webRootPath = _hostingEnvironment.WebRootPath;

                    var uploads = Path.Combine(webRootPath, @"img\ClassifiedsImages");

                    var file = files[0];
                    var extension = Path.GetExtension(file.FileName);

                    if (System.IO.File.Exists(Path.Combine(webRootPath, oldImage.ImageURL.Substring(1))))
                    {
                        System.IO.File.Delete(Path.Combine(webRootPath, oldImage.ImageURL.Substring(1)));
                    }

                    using (var filestream = new FileStream(Path.Combine(uploads, service.ClassifiedListingId + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream); // moves to server and renames
                    }

                    oldImage.ImageExtension = extension;
                    oldImage.ImageURL = @"\" + @"img\ClassifiedsImages" + @"\" + service.ClassifiedListingId + extension;
                }
                
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

        // GET: Classifieds/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _context.ClassifiedListing.FindAsync(id);
            return View(item);
        }

        // POST: Classifieds/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, ClassifiedListing listing)
        {
            ClassifiedListing item = await _context.ClassifiedListing.FindAsync(id);

            string webRootPath = _hostingEnvironment.WebRootPath;
            var uploads = Path.Combine(webRootPath, @"img\ClassifiedsImages");
            var oldImages = await _context.ClassifiedImage.Where(x => x.ClassifiedListingId == item.ClassifiedListingId).ToListAsync();
            foreach (var oldImage in oldImages)
            {
                if (System.IO.File.Exists(Path.Combine(webRootPath, oldImage.ImageURL.Substring(1))))
                {
                    System.IO.File.Delete(Path.Combine(webRootPath, oldImage.ImageURL.Substring(1)));
                }
                _context.ClassifiedImage.Remove(oldImage);
            }
            
            item.IsArchive = true;
            _context.ClassifiedListing.Update(item);
            //_context.ClassifiedListing.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}