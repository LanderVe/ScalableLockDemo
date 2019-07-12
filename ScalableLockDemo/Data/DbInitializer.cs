using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScalableLockDemo.Data
{
  public class DbInitializer
  {
    public static void Initialize(PaymentContext context)
    {
      context.Database.EnsureCreated();

      // Look for any payments.
      if (context.Payments.Any()) return; // DB has been seeded

      var payments = new List<Payment> {
        new Payment { Description="p1", Amount=100M, Counter=0,Status=PaymentStatus.Initialized },
        new Payment { Description="p2", Amount=200M, Counter=0,Status=PaymentStatus.Initialized },
        new Payment { Description="p3", Amount=300M, Counter=0,Status=PaymentStatus.Initialized },
      };

      context.Payments.AddRange(payments);
      context.SaveChanges();
    }
  }
}
