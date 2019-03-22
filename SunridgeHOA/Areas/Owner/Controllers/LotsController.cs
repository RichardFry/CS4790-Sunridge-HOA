﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunridgeHOA.Areas.Owner.Models;
using SunridgeHOA.Models;

namespace SunridgeHOA.Areas.Admin.Controllers
{
    [Area("Owner")]
    public class LotsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LotsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Lots
        public async Task<IActionResult> Index()
        {
            var lots = await _context.Lot
                .Include(l => l.Address)
                .OrderBy(u => u.LotNumber)
                .ToListAsync();

            var vm = new List<LotIndexVM>();

            foreach (var lot in lots)
            {
                var owners = _context.OwnerLot
                    .Include(u => u.Owner)
                    .Where(u => u.LotId == lot.LotId)
                    .Where(u => u.EndDate == DateTime.MinValue);

                if (!owners.Any())
                {
                    vm.Add(new LotIndexVM
                    {
                        Lot = lot,
                        Address = lot.Address,
                        PrimaryOwner = null,
                        Owners = null
                    });
                }
                else
                {
                    vm.Add(new LotIndexVM
                    {
                        Lot = lot,
                        Address = lot.Address,
                        PrimaryOwner = owners.Where(u => u.IsPrimary).First().Owner,
                        Owners = owners.Where(u => !u.IsPrimary).Select(u => u.Owner).ToList()
                    });
                }

                
            }
            return View(vm);
        }

        // GET: Admin/Lots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lot = await _context.Lot
                .Include(l => l.Address)
                .FirstOrDefaultAsync(m => m.LotId == id);
            if (lot == null)
            {
                return NotFound();
            }

            var vm = new LotIndexVM
            {
                Lot = lot,
                Address = lot.Address,
                PrimaryOwner = await GetPrimaryOwnerAsync(lot.LotId),
                Owners = await GetNonPrimaryOwnerAsync(lot.LotId)
            };

            return View(vm);
        }

        // GET: Admin/Lots/Create
        public IActionResult Create()
        {
            //ViewData["AddressId"] = new SelectList(_context.Address, "Id", "Id");
            return View();
        }

        // POST: Admin/Lots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LotEditVM vm)
        {
            if (_context.Lot.Where(u => u.LotNumber == vm.Lot.LotNumber).Any())
            {
                ModelState.AddModelError("LotNumber", "A lot already exists with that lot number");
            }
            if (_context.Lot.Where(u => u.TaxId == vm.Lot.TaxId).Any())
            {
                ModelState.AddModelError("TaxId", "A lot already exists with that tax id");
            }

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(HttpContext.User);
                var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

                // We can auto fill these details because all lots will have same city/state/zip
                var address = new Address
                {
                    City = "Some City",
                    State = "Utah",
                    Zip = "12345",
                    LastModifiedBy = loggedInUser.FullName,
                    LastModifiedDate = DateTime.Now
                };
                _context.Add(address);

                var lot = new Lot
                {
                    Address = address,
                    Status = "Something",
                    LastModifiedBy = loggedInUser.FullName,
                    LastModifiedDate = DateTime.Now
                };
                _context.Add(lot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["AddressId"] = new SelectList(_context.Address, "Id", "Id", lot.AddressId);
            return View(vm);
        }

        // GET: Admin/Lots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lot = await _context.Lot.Include(u => u.Address).SingleOrDefaultAsync(u => u.LotId == id);
            if (lot == null)
            {
                return NotFound();
            }

            var currOwner = await GetPrimaryOwnerAsync(id.Value);

            //ViewData["AddressId"] = new SelectList(_context.Address, "Id", "Id", lot.AddressId);
            var list = new SelectList(_context.Owner, "OwnerId", "FullName", currOwner.OwnerId);
            ViewData["Owner"] = new SelectList(_context.Owner, "OwnerId", "FullName", currOwner.OwnerId);
            return View(new LotEditVM { Lot = lot, Address = lot.Address });
        }

        // POST: Admin/Lots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LotEditVM vm)
        {
            if (id != vm.Lot.LotId)
            {
                return NotFound();
            }

            if (vm.OwnerId == -1)
            {
                // Assuming that a lot must always have an owner
                ModelState.AddModelError("OwnerId", "You must select an owner");
            }

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.GetUserAsync(HttpContext.User);
                var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

                try
                {
                    // Update Address entry
                    var addr = _context.Address.Find(vm.Address.Id);
                    addr.StreetAddress = vm.Address.StreetAddress;
                    addr.LastModifiedBy = loggedInUser.FullName;
                    addr.LastModifiedDate = DateTime.Now;
                    _context.Update(addr);

                    // Update Lot entry
                    var lot = _context.Lot.Find(vm.Lot.LotId);
                    lot.LotNumber = vm.Lot.LotNumber;
                    lot.Status = vm.Lot.Status;
                    lot.TaxId = vm.Lot.TaxId;
                    lot.LastModifiedBy = loggedInUser.FullName;
                    lot.LastModifiedDate = DateTime.Now;
                    _context.Update(lot);

                    // Check OwnerLot entry for this owner/lot combination, create if necessary
                    var currOwnerLot = await _context.OwnerLot
                        .Where(u => u.LotId == id)
                        .Where(u => u.EndDate == DateTime.MinValue)
                        .Where(u => u.IsPrimary)
                        .FirstOrDefaultAsync();

                    // Nobody owns this lot right now
                    if (currOwnerLot == null)
                    {
                        // Create a new entry
                        var newOwnerLot = new OwnerLot
                        {
                            OwnerId = vm.OwnerId,
                            LotId = id,
                            IsPrimary = true,
                            StartDate = DateTime.Now
                        };

                        _context.Add(newOwnerLot);
                    }
                    // Someone else is set as the owner
                    else if (vm.OwnerId != currOwnerLot.OwnerId)
                    {
                        // End the existing relationship
                        currOwnerLot.EndDate = DateTime.Now;

                        // Create a new entry
                        var newOwnerLot = new OwnerLot
                        {
                            OwnerId = vm.OwnerId,
                            LotId = id,
                            IsPrimary = true,
                            StartDate = DateTime.Now
                        };

                        _context.Add(newOwnerLot);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LotExists(vm.Lot.LotId))
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
            //ViewData["AddressId"] = new SelectList(_context.Address, "Id", "Id", lot.AddressId);
            ViewData["Owner"] = new SelectList(_context.Owner, "OwnerId", "FullName");
            return View(vm);
        }

        // GET: Admin/Lots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lot = await _context.Lot
                .Include(l => l.Address)
                .FirstOrDefaultAsync(m => m.LotId == id);
            if (lot == null)
            {
                return NotFound();
            }

            return View(lot);
        }

        // POST: Admin/Lots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var identityUser = await _userManager.GetUserAsync(HttpContext.User);
            var loggedInUser = _context.Owner.Find(identityUser.OwnerId);

            var lot = await _context.Lot.Include(u => u.Address).FirstOrDefaultAsync(u => u.LotId == id);
            lot.LastModifiedBy = loggedInUser.FullName;
            lot.LastModifiedDate = DateTime.Now;
            lot.IsArchive = true;
            lot.Address.LastModifiedBy = loggedInUser.FullName;
            lot.Address.LastModifiedDate = DateTime.Now;
            lot.Address.IsArchive = true;

            //_context.Lot.Remove(lot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LotExists(int id)
        {
            return _context.Lot.Any(e => e.LotId == id);
        }

        private async Task<List<OwnerLot>> GetLotOwners(int id)
        {
            return await _context.OwnerLot
                .Include(u => u.Owner)
                .Where(u => u.LotId == id)
                .Where(u => u.EndDate == DateTime.MinValue)
                .ToListAsync();
        }

        private async Task<SunridgeHOA.Models.Owner> GetPrimaryOwnerAsync(int id)
        {
            return await _context.OwnerLot
                .Include(u => u.Owner)
                .Where(u => u.LotId == id)
                .Where(u => u.EndDate == DateTime.MinValue)
                .Where(u => u.IsPrimary)
                .Select(u => u.Owner)
                .FirstOrDefaultAsync();
        }

        private async Task<List<SunridgeHOA.Models.Owner>> GetNonPrimaryOwnerAsync(int id)
        {
            return await _context.OwnerLot
                .Include(u => u.Owner)
                .Where(u => u.LotId == id)
                .Where(u => u.EndDate == DateTime.MinValue)
                .Where(u => !u.IsPrimary)
                .Select(u => u.Owner)
                .ToListAsync();
        }
    }
}