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
            //Include all tables that are needed by the view. 
            var boardMemberContext = _context.BoardMember.Include(b => b.Owner).Include(b => b.Photo).Include(b => b.Owner.OwnerLots).ThenInclude(x => x.Lot);
            var boardMembers = await boardMemberContext.ToListAsync(); //Get all available board members
            var ordered = boardMembers.OrderBy(x => x.Priority).ToList();//Order them by priority. 
            return View(ordered);//Return the index view. 
        }
    }
}