using Microsoft.EntityFrameworkCore;
using Core.Entities; // Ensure this using directive is present to reference the Product class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SS_DbContext : DbContext
    {
        public SS_DbContext(DbContextOptions<SS_DbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}