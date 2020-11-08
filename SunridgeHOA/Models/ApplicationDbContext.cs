using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunridgeHOA.Models;
using SunridgeHOA.Areas.Admin.Models;

namespace SunridgeHOA.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Lot> Lot { get; set; }
        public DbSet<LotInventory> LotInventory { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<LotHistory> LotHistory { get; set; }
        public DbSet<Owner> Owner { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<ClassifiedListing> ClassifiedListing { get; set; }
        public DbSet<ClassifiedCategory> ClassifiedCategory { get; set; }
        public DbSet<KeyHistory> KeyHistory { get; set; }
        public DbSet<Key> Key { get; set; }
        public DbSet<CommonAreaAsset> CommonAreaAsset { get; set; }
        public DbSet<Maintenance> Maintenance { get; set; }
        public DbSet<NewsItem> NewsItem { get; set; }
        public DbSet<OwnerLot> OwnerLot { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<SunridgeHOA.Models.Banner> Banner { get; set; }
        public DbSet<ScheduledEvent> ScheduledEvents { get; set; }
        public DbSet<SunridgeHOA.Models.ClassifiedImage> ClassifiedImage { get; set; }
        public DbSet<FormResponse> FormResponse { get; set; }
        public DbSet<InKindWorkHours> InKindWorkHours { get; set; }

        //gk
        public DbSet<Photo> Photo { get; set; }
        public DbSet<BoardMember> BoardMember { get; internal set; }
    }
}
