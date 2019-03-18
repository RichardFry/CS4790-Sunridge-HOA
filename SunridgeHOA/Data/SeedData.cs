﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SunridgeHOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Data
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            

            // Add default users and roles
            if (!context.Owner.Any())
            {
                // Add temporary addresses
                context.Address.AddRange(
                    new Address
                    {
                        StreetAddress = "123 Fake Street",
                        City = "Somewhere",
                        State = "UT",
                        IsArchive = false,
                        LastModifiedBy = "Seed",
                        LastModifiedDate = DateTime.Now
                    },
                    new Address
                    {
                        StreetAddress = "567 Circle Lane",
                        City = "City",
                        State = "NY",
                        IsArchive = false,
                        LastModifiedBy = "Seed",
                        LastModifiedDate = DateTime.Now
                    });

                // Add temporary owners
                context.Owner.AddRange(
                    new Owner
                    {
                        AddressId = 1,
                        IsPrimary = true,
                        FirstName = "Test",
                        LastName = "Guy",
                        Occupation = "Some sort of admin",
                        Birthday = DateTime.Now,
                        EmergencyContactName = "Someone Person",
                        EmergencyContactPhone = "8015551234",
                        IsArchive = false,
                        LastModifiedBy = "Seed",
                        LastModifiedDate = DateTime.Now
                    },
                    new Owner
                    {
                        AddressId = 2,
                        IsPrimary = true,
                        FirstName = "Richard",
                        LastName = "Guy",
                        Occupation = "Data tester",
                        Birthday = DateTime.Now,
                        EmergencyContactName = "Someone Person",
                        EmergencyContactPhone = "8015551234",
                        IsArchive = false,
                        LastModifiedBy = "Seed",
                        LastModifiedDate = DateTime.Now
                    });

                context.SaveChanges();
            }
        }
    }
}
