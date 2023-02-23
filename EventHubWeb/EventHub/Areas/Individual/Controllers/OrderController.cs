using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using EventHubWeb.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Runtime.InteropServices.ObjectiveC;
using System.Security.Claims;

namespace EventHubWeb.Areas.Individual.Controllers;

[Area(SD.Role_User_Individual)]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Checkout(int id)
    {
        //getting user id 
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        }
        OrderVM orderVM = new OrderVM
        {
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value),
            Event = _unitOfWork.Event.GetFirstOrDefault(u => u.EventId == id),
        }
        ;
        return View(orderVM);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Payment(OrderVM orderVM)
    {
        string payId;
        orderVM.Event = _unitOfWork.Event.GetFirstOrDefault(e => e.EventId == orderVM.Event.EventId);
        var domain = "http://abdulazizbinbaz-001-site1.atempurl.com/";
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>
    {
      new SessionLineItemOptions
      {
        PriceData = new SessionLineItemPriceDataOptions
        {
          UnitAmount = orderVM.Event.EventPrice*100,
          Currency = "usd",
          ProductData = new SessionLineItemPriceDataProductDataOptions
          {
            Name = orderVM.Event.EventName,
          },
        },
        Quantity = 1,
      },
    },
            Mode = "payment", 
            SuccessUrl = domain+$"Individual/Order/OrderConformation?id={orderVM.Event.EventId}",
            CancelUrl = domain + $"Individual/Order/Checkout?id={orderVM.Event.EventId}",
        };

        var service = new SessionService();
        
        Session session = service.Create(options);
        payId = session.PaymentIntentId;

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }
    public IActionResult OrderConformation(int id)
    {
        OrderVM orderVM = new OrderVM();
        //getting user id 
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        }
        var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "EventTickets");
        var eventFromDb = _unitOfWork.Event.GetFirstOrDefault(e => e.EventId == id, includeProperties: "EventTickets");

        if (userFromDb.EventTickets.Where(u => u.EventId == id).Any()) 
        {
            orderVM.Event = eventFromDb;
            orderVM.applicationUser = userFromDb;
            return View(orderVM);
        }

        userFromDb.EventTickets.Add(
                        new EventTicket()
                        {
                            User = userFromDb,
                            _event = eventFromDb,

                        });
        _unitOfWork.Save();

        orderVM.Event = eventFromDb;
        orderVM.applicationUser = userFromDb;


        return View(orderVM);
    }


}
