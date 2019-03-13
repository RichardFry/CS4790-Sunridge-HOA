﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Models
{
    public class Maintenance : DbItem
    {
        public int MaintenanceId { get; set; }
        public int CommonAreaAssetId { get; set; }

        public float Cost { get; set; }
        public DateTime DateCompleted { get; set; }
        public string Description { get; set; }

        //Nav properties
        public CommonAreaAsset CommonAreaAsset { get; set; }

    }
}
