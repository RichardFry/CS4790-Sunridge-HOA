using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SunridgeHOA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunridgeHOA.Areas.Admin.Models
{
    public class BoardMember
    {
        
        public int BoardMemberId { get; set; }

        [Display(Name = "Position")]
        public string BoardPosition { get; set; }

        [Display(Name = "Owner")]
        public int OwnerId { get; set; }

        public int Priority { get; set; }

        [Display(Name = "Photo")]
        public int PhotoId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Owner Name")]
        public virtual SunridgeHOA.Models.Owner Owner { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
