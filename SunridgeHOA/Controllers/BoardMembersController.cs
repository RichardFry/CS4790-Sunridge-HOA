using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunridgeHOA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Controllers
{
    public class BoardMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoardMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var boardMemberContext = _context.BoardMember.Include(b => b.Owner);
            var boardMembers = await boardMemberContext.ToListAsync();
            var ordered = boardMembers.OrderBy(x => x.Priority).ToList();
            return View(ordered);
        }
    }
}