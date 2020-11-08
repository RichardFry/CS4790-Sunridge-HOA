using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunridgeHOA.Models;
using SunridgeHOA.Models.ViewModels;

namespace SunridgeHOA.Controllers
{
    public class BoardMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoardMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Lots()
        {
            var lotCategory = _context.ClassifiedCategory
                .Single(u => u.Description == "Lots")
                .ClassifiedCategoryId;
            var classifieds = await GetClassifiedsByCategory(lotCategory);
            return View(classifieds);
        }

        public async Task<IActionResult> Cabins()
        {
            var cabinCategory = _context.ClassifiedCategory
                .Single(u => u.Description == "Cabins")
                .ClassifiedCategoryId;
            var classifieds = await GetClassifiedsByCategory(cabinCategory);
            return View(classifieds);
        }

        public async Task<IActionResult> Other()
        {
            var otherCategory = await _context.ClassifiedCategory
                .SingleAsync(u => u.Description == "Other");
            var classifieds = await _context.ClassifiedListing
                .Include(u => u.Images)
                .Where(u => u.ClassifiedCategoryId == otherCategory.ClassifiedCategoryId)
                .Where(u => u.Images.Any())
                .Where(u => !u.IsArchive)
                .Select(u => u.Images[0].ImageURL)
                .ToListAsync();
            return View(classifieds);
        }

        public async Task<IActionResult> ATVs()
        {
            var atvCategory = _context.ClassifiedCategory
                .Single(u => u.Description == "ATVs")
                .ClassifiedCategoryId;
            var classifieds = await GetClassifiedsByCategory(atvCategory);
            return View(classifieds);
        }

        public async Task<IActionResult> Trailers()
        {
            var trailerCategory = _context.ClassifiedCategory
                .Single(u => u.Description == "Trailers")
                .ClassifiedCategoryId;
            var classifieds = await GetClassifiedsByCategory(trailerCategory);
            return View(classifieds);
        }

        private async Task<List<ClassifiedListing>> GetClassifiedsByCategory(int category)
        {
            var classifieds = await _context.ClassifiedListing
                .Include(u => u.Images)
                .Where(u => u.ClassifiedCategoryId == category)
                .Where(u => !u.IsArchive)
                .ToListAsync();
            return classifieds;
        }
    }
}