﻿using SunridgeHOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Areas.Owner.Models
{
    public class LotEditInventoryVM
    {
        public Lot Lot { get; set; }
        public Address Address { get; set; }
        public List<int> SelectedItems { get; set; }
    }
}
