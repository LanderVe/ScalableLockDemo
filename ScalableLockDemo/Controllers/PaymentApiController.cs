using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScalableLockDemo.Data;
using ScalableLockDemo.Services;
using Weakly;

namespace ScalableLockDemo.Controllers
{
  [Route("api/payment")]
  [ApiController]
  public class PaymentApiController : ControllerBase
  {
    private readonly PaymentContext db;
    private readonly PaymentService service;

    public PaymentApiController(PaymentContext db, PaymentService service)
    {
      this.db = db;
      this.service = service;
    }

    #region NO LOCKING 
    [HttpGet("notifynolock")]
    public async Task<IActionResult> NotifyAsync(int paymentid)
    {
      //get from db
      var payment = await db.Payments.FindAsync(paymentid);

      //talk to service
      var delta = await service.GetPaymentDeltaAsync(payment);
      MergePayment(payment, delta);

      //store in db
      await db.SaveChangesAsync();

      return Ok(paymentid);
    }
    #endregion

    #region Lock All
    private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    [HttpGet("notifylockall")]
    public async Task<IActionResult> NotifyLockAll(int paymentid)
    {
      await semaphore.WaitAsync();
      try
      {
        //get from db
        var payment = await db.Payments.FindAsync(paymentid);

        //talk to service
        var delta = await service.GetPaymentDeltaAsync(payment);
        MergePayment(payment, delta);

        //store in db
        await db.SaveChangesAsync();
      }
      finally
      {
        semaphore.Release();
      }

      return Ok(paymentid);
    }
    #endregion

    #region Lock per id
    [HttpGet("notifylockperid")]
    public async Task<IActionResult> NotifyLockPerId(int paymentid)
    {
      var semaphore = GetSemaphoreForPayment(paymentid);
      await semaphore.WaitAsync();
      try
      {
        //get from db
        var payment = await db.Payments.FindAsync(paymentid);

        //talk to service
        var delta = await service.GetPaymentDeltaAsync(payment);
        MergePayment(payment, delta);

        //store in db
        await db.SaveChangesAsync();
      }
      finally
      {
        semaphore.Release();
      }

      return Ok(paymentid);
    }

    //Weakreferences are ok even though SemaphoreSlime implements IDisposable, https://stackoverflow.com/questions/32033416/do-i-need-to-dispose-a-semaphoreslim
    private static WeakValueDictionary<int, SemaphoreSlim> semaphores = new WeakValueDictionary<int, SemaphoreSlim>();

    private SemaphoreSlim GetSemaphoreForPayment(int paymentId)
    {
      lock (semaphores)
      {
        var semaphore = semaphores.GetValueOrDefault(paymentId);
        if (semaphore == null)
        {
          semaphore = new SemaphoreSlim(1, 1);
          semaphores.Add(paymentId, semaphore);
        }
        return semaphore;
      }
    }
    #endregion

    private void MergePayment(Payment payment, PaymentDelta delta)
    {
      payment.Amount = delta.Amount ?? payment.Amount;
      payment.Description = delta.Description ?? payment.Description;
      payment.Status = delta.Status ?? payment.Status;

      ++payment.Counter; //increase counter, just for testing
    }

  }
}