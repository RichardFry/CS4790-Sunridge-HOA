using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunridgeHOA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Controllers
{
    /// <summary>
    /// This class controls the access of boardmembers by non admins
    /// </summary>
    public class BoardMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Board members controller for non admin applications
        /// </summary>
        /// <param name="context">The entier database context</param>
        public BoardMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create a list of board members.
        /// Remove inactive members, sort them by priority.
        /// Return the view. 
        /// </summary>
        /// <returns>The index view</returns>
        public async Task<IActionResult> Index()
        {
            //Include all tables that are needed by the view. 
            var boardMemberContext = _context.BoardMember.Include(b => b.Owner).Include(b => b.Photo).Include(b => b.Owner.OwnerLots).ThenInclude(x => x.Lot);
            var boardMembers = await boardMemberContext.ToListAsync(); //Get all available board members
            var ordered = boardMembers.Where(x=>x.IsActive).OrderBy(x => x.Priority).ToList();//Order them by priority. 
            return View(ordered);//Return the index view. 
        }
    }
}