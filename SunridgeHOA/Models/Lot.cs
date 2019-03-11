﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Models
{
    public class Lot
    {
        public int LotId { get; set; }
        public int AddressId { get; set; }
        public int OwnerId { get; set; }

        public string LotNumber { get; set; }
        public string Status { get; set; }
        public bool IsArchive { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        //NavigationalProperties
        public virtual Address Address { get; set; }
        public virtual Owner Owner { get; set; }
        public virtual ICollection<LotInventory> LotInventories { get; set; }
        public virtual ICollection<OwnerHistory> OwnerHistories { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }

    }
}