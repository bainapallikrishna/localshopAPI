using LocalShop.DBContext;
using LocalShop.Entities;
using LocalShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShop.Repository
{
    public class AppDataContext:DbContext
    {
        public AppDataContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
