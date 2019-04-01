﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Models
{
    public class Lot
    {
        public int LotId { get; set; }
        public int AddressId { get; set; }
        //public int OwnerId { get; set; }

        [Display(Name = "Lot Number")]
        [Required]
        public string LotNumber { get; set; }

        [Display(Name = "Tax ID")]
        public string TaxId { get; set; }

        public bool IsArchive { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        //NavigationalProperties
        public virtual Address Address { get; set; }
        //public virtual Owner Owner { get; set; }
        public virtual ICollection<OwnerLot> OwnerLots { get; set; }
        public virtual ICollection<LotInventory> LotInventories { get; set; }
        public virtual ICollection<LotHistory> LotHistories { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }

    }
}
