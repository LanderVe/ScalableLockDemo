using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScalableLockDemo.Data
{
  public class PaymentContext : DbContext
  {
    public PaymentContext(DbContextOptions<PaymentContext> options) : base(options) { }
    public DbSet<Payment> Payments { get; set; }
  }
}
