using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SunridgeHOA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunridgeHOA.Areas.Admin.Models
{
    public class BoardMember
    {
        [Key]
        [Required]
        public int BoardMemberId { get; set; }

        [Required]
        public string BoardPosition { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public int LotId { get; set; }

        [Required]
        public int Priority { get; set; }

        public int test { get; set; }

        [Required]
        public string PhotoId { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Calculated properties
        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return $"{Owner.FirstName} {Owner.LastName}";
            }
        }

        [Display(Name = "Owner Name")]
        [ForeignKey("OwnerId")]
        public virtual SunridgeHOA.Models.Owner Owner { get; set; }

        [ForeignKey("PhotoIdFK")]
        public virtual Photo Photo { get; set; }

        [ForeignKey("LotId")]
        public virtual Lot Lot { get; set; }

    }
}
