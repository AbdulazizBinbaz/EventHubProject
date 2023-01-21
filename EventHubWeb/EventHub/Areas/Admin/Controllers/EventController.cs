using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using EventHubWeb.ViewModels;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Models;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EventHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_EventManager + "," + SD.Role_Admin)]
    public class EventController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public EventController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: EventController

        public ActionResult Index()
        {
            if (User.IsInRole(SD.Role_User_EventManager))
            {
                //getting user id 
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                IEnumerable<Event> EventList = _unitOfWork.Event.GetAll(u => u.EventManagerId == claim.Value, includeProperties: "applicationUser");
                return View(EventList);

            }
            else
            {

                IEnumerable<Event> EventList = _unitOfWork.Event.GetAll(includeProperties: "applicationUser");
                return View(EventList);
            }
        }

        // GET: EventController/Details/5
        public ActionResult Details(int id)
        {
            Event EventFromDb = _unitOfWork.Event.GetFirstOrDefault(u => u.EventId == id, includeProperties: "categories");
            return View(EventFromDb);
        }

        public ActionResult Create()
        {
            HomepageVM eventVM = new HomepageVM()
            {
                Event = new Event()
                {
                    categories = new List<Category>(),
                },
                CategoryList = _unitOfWork.Category.GetAll().Select(
                    i => new SelectListItem
                    {
                        Text = i.CategoryName,
                        Value = i.CategoryId.ToString()
                    }


                    )
            };
            return View(eventVM);
        }
        // POST: EventController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_User_EventManager + "," + SD.Role_Admin)]
        public ActionResult Create(HomepageVM eventVM, IFormFile file)
        {
            Event obj = eventVM.Event;
            var FirstSelectedCategory = _unitOfWork.Category.GetFirstOrDefault(
                u => u.CategoryId == eventVM.FirstSelectedCategoryId, includeProperties: "Events");
            var SecondSelectedCategory = _unitOfWork.Category.GetFirstOrDefault(
                u => u.CategoryId == eventVM.SecondSelectedCategoryId, includeProperties: "Events");
            if (FirstSelectedCategory != null)
            {
                obj.categories = new List<Category>();
                obj.categories.Add(FirstSelectedCategory);
                obj.categories.Add(SecondSelectedCategory);
            }

            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            obj.EventStatus = SD.EventStatus_WaitingForApproval;
            obj.EventManagerId = claim.Value;
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, "uploads", "events", "images");
                var extension = Path.GetExtension(file.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                obj.EventPictureUrl = @"uploads\events\images\" + fileName + extension;
                _unitOfWork.Event.Add(obj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(obj);

        }
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult ChangeStatus(Event obj)
        {
            if (ModelState.IsValid)
            {

                Event eventFromDb = _unitOfWork.Event.GetFirstOrDefault(u => u.EventId == obj.EventId);
                eventFromDb.EventStatus = obj.EventStatus;
                _unitOfWork.Event.Update(eventFromDb);
                _unitOfWork.Save();
            }

            return RedirectToAction("Details", new { id = obj.EventId });

        }

        [Authorize(Roles = SD.Role_User_EventManager + "," + SD.Role_Admin)]

        // GET: EventController/Edit/5
        public ActionResult Edit(int id)
        {
            HomepageVM eventVM = new HomepageVM()
            {
                Event = _unitOfWork.Event.GetFirstOrDefault(u => u.EventId == id, includeProperties: "categories"),
                CategoryList = _unitOfWork.Category.GetAll().Select
                (
                    i => new SelectListItem
                    {
                        Text = i.CategoryName,
                        Value = i.CategoryId.ToString(),
                    }
                ),

            };
            if (eventVM.Event.categories.Count() == 1)
            {
                eventVM.FirstSelectedCategoryId = eventVM.Event.categories.First().CategoryId;

            }
            else
            {
                eventVM.FirstSelectedCategoryId = eventVM.Event.categories.First().CategoryId;
                eventVM.SecondSelectedCategoryId = eventVM.Event.categories.Skip(1).First().CategoryId;
            }


            return View(eventVM);
        }

        // POST: EventController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HomepageVM eventVM, IFormFile? file)
        {
            Event obj = _unitOfWork.Event.GetFirstOrDefault(u => u.EventId == eventVM.Event.EventId, includeProperties: "categories");
            //------------------------Update Categoryies--------------------------
            var FirstSelectedCategory = _unitOfWork.Category.GetFirstOrDefault(
                u => u.CategoryId == eventVM.FirstSelectedCategoryId, includeProperties: "Events");
            var SecondSelectedCategory = _unitOfWork.Category.GetFirstOrDefault(
               u => u.CategoryId == eventVM.SecondSelectedCategoryId, includeProperties: "Events");
            obj.categories.Clear();
            obj.categories.Add(FirstSelectedCategory);
            obj.categories.Add(SecondSelectedCategory);
            //--------------------------------------------------------------------
            //------------------------Update Picture------------------------------
            if (file is not null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, "uploads", "events", "images");
                var extension = Path.GetExtension(file.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                var oldImagePath = Path.Combine(wwwRootPath, obj.EventPictureUrl.Trim('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                obj.EventPictureUrl = @"uploads\events\images\" + fileName + extension;
                //--------------------------------------------------------------------
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Event.Update(obj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);

        }

        // GET: EventController/Delete/5
        public ActionResult Delete(int id)
        { 
            Event EventFromDb = _unitOfWork.Event.GetFirstOrDefault(u => u.EventId == id, includeProperties: "applicationUser");
            return View(EventFromDb);
        }

        // POST: EventController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Event obj)
        {
            if (User.IsInRole(SD.Role_Admin))
            {
                if (obj.EventPictureUrl is not null)
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.EventPictureUrl.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.Event.Remove(obj);
                _unitOfWork.Save();
            }
            else
            {
                obj.EventStatus = SD.EventStatus_DeleteRequest;
                _unitOfWork.Event.Update(obj);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index");
        }

        public IActionResult ListAttendees(int id, int page = 1, int? SearchedTicketID = null)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // Get the event tickets for the event
            var tickets = _unitOfWork.Event.GetFirstOrDefault(e => e.EventId == id, includeProperties: "EventTickets.User").EventTickets
                .Where(t => t.EventId == id);
            if(SearchedTicketID != null)
            {
               tickets =  tickets.Where(t => t.TicketId == SearchedTicketID);
            }

            tickets = tickets
                .OrderBy(t => t.TicketId)
                .Skip((page - 1) * 10)
                .Take(10);

            // Set the page number and page count in the view bag
            ViewBag.PageNumber = page;
            ViewBag.PageCount = (int)Math.Ceiling(tickets.Count() / 10.0);

            ListOfAttendeesVM listOfAttendeesVM = new ListOfAttendeesVM();
            listOfAttendeesVM.tickets = tickets;
            listOfAttendeesVM._event = _unitOfWork.Event.GetFirstOrDefault(e => e.EventId == id);
            if(claim is not null)
            if(listOfAttendeesVM._event.EventManagerId != claim.Value)
            {
                return RedirectToAction("Index");
            }
            return View(listOfAttendeesVM);
        }
        
        

    }
}
