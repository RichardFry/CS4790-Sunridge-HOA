using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunridgeHOA.Areas.Admin.Data;
using SunridgeHOA.Areas.Admin.Models;
using SunridgeHOA.Models;
using SunridgeHOA.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class BoardMembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HostingEnvironment _hostingEnv;

        public BoardMembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, HostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnv = env;
        }

        // GET: Admin/Owners
        public async Task<IActionResult> Index(string query, int? pageNumber)
        {
            ViewData["CurrentQuery"] = query ?? string.Empty;

            IQueryable<BoardMember> boardMembers = null;

            // No search - include all board members
           
            boardMembers = _context.BoardMember
                .Include(u => u.Photo)
                .Include(o => o.Owner)
                .Where(u => u.FullName != "Super Admin")
                .OrderBy(u => u.Priority);

            int pageSize = 25;
            return View(await PaginatedList<BoardMember>.Create(boardMembers, pageNumber ?? 1, pageSize));
        }

        // GET: Admin/Owners/Create
        public IActionResult Create()
        {
            ViewData["LotsSelect"] = new SelectList(_context.Lot.OrderBy(u => u.LotNumber).ToList(), "LotId", "LotNumber");
            return View();
        }

        // POST: Admin/Owners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardMember model)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(HttpContext.User);
                var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

                // Create board memebr
                var newOwner = new BoardMember
                {
                    BoardPosition = model.BoardPosition,
                    IsActive = model.IsActive,
                    Priority = model.Priority,
                    OwnerId = model.OwnerId,
                    
                };

                return RedirectToAction(nameof(Index));
            }

            ViewData["LotsSelect"] = new SelectList(_context.Lot.OrderBy(u => u.LotNumber).ToList(), "LotId", "LotNumber", model.LotId);
            return View(model);
        }

        // GET: Admin/Owners/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var owner = await _context.Owner.Include(u => u.Address).SingleOrDefaultAsync(u => u.OwnerId == id);
        //    if (owner == null)
        //    {
        //        return NotFound();
        //    }

        //    var appUser = await _userManager.FindByIdAsync(owner.ApplicationUserId);
        //    var roles = await _userManager.GetRolesAsync(appUser);

        //    var vm = new OwnerVM
        //    {
        //        Owner = owner,
        //        Address = owner.Address,
        //        IsAdmin = roles.Contains("Admin")
        //    };

        //    var identityUser = await _userManager.GetUserAsync(HttpContext.User);
        //    ViewData["CurrUserId"] = identityUser.Id;

        //    return View(vm);
        //}

        // POST: Admin/Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BoardMember model)
        {
            if (id != model.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var identityUser = await _userManager.GetUserAsync(HttpContext.User);
                    var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

                    Photo photo = await _context.Photo.SingleOrDefaultAsync(u => u.PhotoId == model.Photo.PhotoId);
                    model.Photo = photo;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardMemberExists(model.BoardMemberId))
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

            return View(model);
        }

        // GET: Admin/Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardMember = await _context.BoardMember
                .Include(p => p.Photo)
                .Include(o => o.Owner)
                .FirstOrDefaultAsync(m => m.BoardMemberId == id);
            if (boardMember == null)
            {
                return NotFound();
            }

            return View(boardMember);
        }

        // POST: Admin/Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

            // Hide owner from index pages
            var owner = await _context.Owner.FindAsync(id);
            owner.IsArchive = true;
            owner.LastModifiedBy = loggedInUser.FullName;
            owner.LastModifiedDate = DateTime.Now;

            // Disable user login
            var ownerLogin = await _userManager.FindByIdAsync(owner.ApplicationUserId);
            ownerLogin.LockoutEnabled = true;
            ownerLogin.LockoutEnd = DateTime.MaxValue;

            // Remove owner from existing lots
            var ownerLots = await _context.OwnerLot
                .Where(u => u.OwnerId == owner.OwnerId)
                .Where(u => !u.IsArchive)
                .ToListAsync();
            foreach (var rel in ownerLots)
            {
                rel.IsArchive = true;
                rel.LastModifiedBy = loggedInUser.FullName;
                rel.LastModifiedDate = DateTime.Now;
            }

            // Need to make sure that if the lot has other owners, they are listed as primary
            var lotList = ownerLots.Select(u => u.LotId).ToHashSet();
            var lots = await _context.Lot.Include(u => u.OwnerLots).Where(u => lotList.Contains(u.LotId)).ToListAsync();
            foreach (var lot in lots)
            {
                var primary = lot.OwnerLots.Where(u => u.IsPrimary);

                // If the lot has owners, and none of them are listed as a primary
                if (lot.OwnerLots.Any() && !primary.Any())
                {
                    // Only one owner on the lot, therefore they are primary
                    if (lot.OwnerLots.Count == 1)
                    {
                        lot.OwnerLots.ToList()[0].IsPrimary = true;
                    }
                    // More than one owner on the lot, just assume the first entry is primary
                    else
                    {
                        lot.OwnerLots.ToList()[0].IsPrimary = true;
                    }
                }
            }

            await _userManager.UpdateAsync(ownerLogin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardMemberExists(int id)
        {
            return _context.BoardMember.Any(e => e.OwnerId == id);
        }
    }
}
