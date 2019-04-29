﻿using Microsoft.AspNetCore.Identity;
using SunridgeHOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunridgeHOA.Areas.Owner.Data
{
    public static class OwnerUtility
    {
        public static string GenerateDefaultPassword(string username)
        {
            return $"Sunridge{username}123$";
        }

        public static async Task<string> GenerateUsername(UserManager<ApplicationUser> userManager, SunridgeHOA.Models.Owner owner)
        {
            // Find a default username - adds a number to the end if there is a duplicate
            var username = $"{owner.FirstName}{owner.LastName}";
            int count = 0;
            while (await userManager.FindByNameAsync(username) != null)
            {
                count++;
                username = $"{username}{count}";
            }

            return username;
        }
    }
}