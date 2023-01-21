using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using EventHubWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Models;
using System.Diagnostics;
using System.Security.Claims;

namespace EventHubWeb.Areas.Individual.Controllers
{
    [Area("Individual")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string? SearchText)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null) 
            {
                if (SearchText is null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        EventList = _unitOfWork.Event.GetAll(u => u.EventStatus == SD.EventStatus_Approved, includeProperties: "categories"),
                        PostList = _unitOfWork.Post.GetAll(includeProperties: "applicationUser,Likes"),
                    };
                    return View(homepageVM);

                }
                else
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        EventList = _unitOfWork.Event.GetAll(SE => SE.EventName.ToLower()
                        .Contains(SearchText.ToLower()), includeProperties: "categories")
                    };
                    return View(homepageVM);
                }
            }
            else 
            {
                if (SearchText is null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        EventList = _unitOfWork.Event.GetAll(u => u.EventStatus == SD.EventStatus_Approved, includeProperties: "categories"),
                        PostList = _unitOfWork.Post.GetAll(includeProperties: "applicationUser,Likes"),
                        user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u=>u.Id == claim.Value)
                    };
                    return View(homepageVM);

                }
                else
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        EventList = _unitOfWork.Event.GetAll(SE => SE.EventName.ToLower()
                        .Contains(SearchText.ToLower()), includeProperties: "categories"),
                        PostList = _unitOfWork.Post.GetAll(includeProperties: "applicationUser,Likes"),
                        user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value)
                    };
                    return View(homepageVM);
                }
            }


        }
        [HttpPost]
        public IActionResult Search(HomepageVM eventVM)
        {
            return RedirectToAction("Index","Home", new { SearchText = eventVM.SearchText });
        }
        public IActionResult Details(int id)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            HomepageVM homepageVM = new HomepageVM()
            {
                Event = _unitOfWork.Event.GetFirstOrDefault(e => e.EventId == id, includeProperties: "categories,applicationUser,EventComments.User"),
                
            };
            if (claim is not null) 
            {
                homepageVM.user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "EventTickets");
            }

            
            return View(homepageVM);
        }
        public IActionResult Ticket(int id)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is not null)
            {
                HomepageVM homepageVM = new HomepageVM()
                {
                    Event = _unitOfWork.Event.GetFirstOrDefault(e => e.EventId == id, includeProperties: "categories,applicationUser,EventComments.User"),
                    user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "EventTickets")

                };
                if(homepageVM.user.EventTickets.Where(u => u.EventId == id).Any()) 
                {
                    return View(homepageVM);

                }
                else 
                {
                    return RedirectToAction("Details",new { id });
                }

            }
            else 
            {
                return RedirectToAction("Details", new { id });

            }
        }
        [HttpPost]
        public IActionResult AddComment(HomepageVM homepageVM)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) 
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            }
            var eventFromDb = _unitOfWork.Event.GetFirstOrDefault(u => u.EventId == homepageVM.Event.EventId,includeProperties: "EventComments");
            var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "UserComments");
            homepageVM.Comment.User = userFromDb;
            eventFromDb.EventComments.Add(homepageVM.Comment);
            _unitOfWork.Event.Update(eventFromDb);
            _unitOfWork.ApplicationUser.Update(userFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Details", new {id = homepageVM.Event.EventId});
        }
        public IActionResult LikePost(int PostId)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            }
            var postFromDb = _unitOfWork.Post.GetFirstOrDefault(u => u.PostId == PostId, includeProperties: "Likes,Likes.User");
            var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Likes");
            foreach(var like in postFromDb.Likes) 
            {
                if(like.User == userFromDb)
                {
                    return RedirectToAction("Index");

                }
            }
            postFromDb.Likes.Add(new Like { Post = postFromDb, User = userFromDb });
            _unitOfWork.Post.Update(postFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }  
        public IActionResult UnLikePost(int PostId)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            }
            var postFromDb = _unitOfWork.Post.GetFirstOrDefault(u => u.PostId == PostId, includeProperties: "Likes,Likes.User");
            var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Likes");

            postFromDb.Likes.Remove(postFromDb.Likes.First(x=>x.UserId==userFromDb.Id));
            _unitOfWork.Post.Update(postFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}