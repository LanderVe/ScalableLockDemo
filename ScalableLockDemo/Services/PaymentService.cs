using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScalableLockDemo.Data;

namespace ScalableLockDemo.Services
{
  public class PaymentService
  {
    public async Task<PaymentDelta> GetPaymentDeltaAsync(Payment payment)
    {
      await Task.Delay(TimeSpan.FromSeconds(1));

      return new PaymentDelta
      {
        PaymentId = payment.PaymentId,
        Amount = payment.Amount,
        //Description = payment.Description, //Description not sent, do in merge
        Status = payment.Status,
      };
    }
  }

  public class PaymentDelta
  {
    public int PaymentId { get; set; }
    public decimal? Amount { get; set; }
    public string Description { get; set; }
    public PaymentStatus? Status { get; set; }
  }

}
