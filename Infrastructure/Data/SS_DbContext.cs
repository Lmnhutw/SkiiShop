using Microsoft.EntityFrameworkCore;
using Core.Entities; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Config;

namespace Infrastructure.Data
{
    public class SS_DbContext(DbContextOptions<SS_DbContext> options) : DbContext(options)
    {   
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
        }

    }
}