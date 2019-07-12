using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScalableLockDemo.Data
{
  public class Payment
  {
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public PaymentStatus Status { get; set; }
    public int Counter { get; set; }
  }

  public enum PaymentStatus
  {
    Unknown,
    Initialized,
    Completed,
    Cancelled,
  }
}
