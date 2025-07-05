using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SSContext(DbContextOptions options) : DbContext(options)

    {
        public DbSet<Product> Products { get; set; }
    }
}
