using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SunridgeHOA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunridgeHOA.Areas.Admin.Models
{
    public class BoardMember
    {
        
        public int BoardMemberId { get; set; }

        public string BoardPosition { get; set; }

        public int OwnerId { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; }

         //Calculated properties
        //[Display(Name = "Name")]
        //public string FullName
        //{
            //get
            //{
                //return $"{Owner.FirstName} {Owner.LastName}";
            //}
        //}

        [Display(Name = "Owner Name")]
        public virtual SunridgeHOA.Models.Owner Owner { get; set; }
    }
}
