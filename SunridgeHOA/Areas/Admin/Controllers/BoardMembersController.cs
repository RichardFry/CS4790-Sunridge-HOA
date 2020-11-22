using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SunridgeHOA.Areas.Admin.Models;
using SunridgeHOA.Models;

namespace SunridgeHOA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BoardMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoardMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BoardMembers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BoardMember
                .Include(b => b.Owner)
                .Include(b => b.Photo)
                .OrderBy(b => b.Priority);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/BoardMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardMember = await _context.BoardMember
                .Include(b => b.Owner)
                .Include(b => b.Photo)
                .FirstOrDefaultAsync(m => m.BoardMemberId == id);
            if (boardMember == null)
            {
                return NotFound();
            }

            return View(boardMember);
        }

        // GET: Admin/BoardMembers/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Owner.OrderBy(o => o.FullName), "OwnerId", "FullName");
            ViewData["PhotoId"] = new SelectList(_context.Photo.OrderBy(p => p.Title), "PhotoId", "Title");
            return View();
        }

        // POST: Admin/BoardMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoardMemberId,BoardPosition,OwnerId,Priority,PhotoId,IsActive")] BoardMember boardMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boardMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Owner.OrderBy(o => o.FullName), "OwnerId", "FullName", boardMember.OwnerId);
            ViewData["PhotoId"] = new SelectList(_context.Photo.OrderBy(p => p.Title), "PhotoId", "Title", boardMember.PhotoId);
            return View(boardMember);
        }

        // GET: Admin/BoardMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardMember = await _context.BoardMember.FindAsync(id);
            if (boardMember == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Owner.OrderBy(o => o.FullName), "OwnerId", "FullName", boardMember.OwnerId);
            ViewData["PhotoId"] = new SelectList(_context.Photo.OrderBy(p => p.Title), "PhotoId", "Title", boardMember.PhotoId);
            return View(boardMember);
        }

        // POST: Admin/BoardMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoardMemberId,BoardPosition,OwnerId,Priority,PhotoId,IsActive")] BoardMember boardMember)
        {
            if (id != boardMember.BoardMemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boardMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardMemberExists(boardMember.BoardMemberId))
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
            ViewData["OwnerId"] = new SelectList(_context.Owner.OrderBy(o => o.FullName), "OwnerId", "FullName", boardMember.OwnerId);
            ViewData["PhotoId"] = new SelectList(_context.Photo.OrderBy(p => p.Title), "PhotoId", "Title", boardMember.PhotoId);
            return View(boardMember);
        }

        // GET: Admin/BoardMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardMember = await _context.BoardMember
                .Include(b => b.Owner)
                .Include(b => b.Photo)
                .FirstOrDefaultAsync(m => m.BoardMemberId == id);
            if (boardMember == null)
            {
                return NotFound();
            }

            return View(boardMember);
        }

        // POST: Admin/BoardMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boardMember = await _context.BoardMember.FindAsync(id);
            _context.BoardMember.Remove(boardMember);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardMemberExists(int id)
        {
            return _context.BoardMember.Any(e => e.BoardMemberId == id);
        }
    }
}
