﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Models
{
    public class NewsItem : DbItem
    {
        public int NewsItemId { get; set; }
        public int FileId { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime Year { get; set; }

        // Nav properties
        public File File { get; set; }

    }
}