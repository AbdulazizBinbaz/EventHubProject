using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using EventHubWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Identity.Client;
using Models;
using System.Diagnostics;
using System.Linq;
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
                        EventList = _unitOfWork.Event.GetAll(SE => SE.EventName.Contains(SearchText.ToLower())
                        && SE.EventStatus ==SD.EventStatus_Approved
                        , includeProperties: "categories"),
                        PostList = _unitOfWork.Post.GetAll(u=>u.PostText.ToLower().Contains(SearchText.ToLower()),includeProperties: "applicationUser,Likes"),

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
                        PostList = _unitOfWork.Post.GetAll(u => u.PostText.ToLower().Contains(SearchText.ToLower()), includeProperties: "applicationUser,Likes"),
                        user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value)
                    };
                    return View(homepageVM);
                }
            }


        }
        [HttpPost]
        public IActionResult Search(HomepageVM homepageVM)
        {
            return RedirectToAction("Index","Home", new { SearchText = homepageVM.SearchText });
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
                    return RedirectToAction("Index");
                }

            }
            else 
            {
                return RedirectToAction("Index");

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
        
        public IActionResult LikePost(int PostId, string? page)
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
            if(page == "Index")
            {
                return RedirectToAction("Index");

            }
            if (page == "Posts")
            {
                return RedirectToAction("Posts");
            }      

            return RedirectToAction("ProfilePage", "Profile", new {id=postFromDb.UserId});
        }
        public IActionResult UnLikePost(int PostId, string? page)
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

            postFromDb.Likes.Remove(postFromDb.Likes.First(x => x.UserId == userFromDb.Id));
            _unitOfWork.Post.Update(postFromDb);
            _unitOfWork.Save();
            if (page == "Index")
            {
                return RedirectToAction("Index");

            }
            if (page == "Posts")
            {
                return RedirectToAction("Posts");
            }

            return RedirectToAction("ProfilePage", "Profile", new { id = postFromDb.UserId });
        }
    
        public IActionResult Events(HomepageVM homepageVM1)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim is not null)
            {
                if (homepageVM1 is null || homepageVM1.SearchText is null)
                {
                    ApplicationUser user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Categories");
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        FollowedCategoryEventList = _unitOfWork.Event.GetAll(u => u.EventStatus == SD.EventStatus_Approved, includeProperties: "categories")
                        .Where(c=>c.categories.Intersect(user.Categories).Any()) ,
                        EventList = _unitOfWork.Event.GetAll(u => u.EventStatus == SD.EventStatus_Approved, includeProperties: "categories"),
                        CategoryListForEvent = _unitOfWork.Category.GetAll()
                    };
                    return View(homepageVM);
                }
                if (homepageVM1.SearchText is not null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        EventList = _unitOfWork.Event.GetAll(u => u.EventStatus == SD.EventStatus_Approved && u.EventLocation.ToLower().
                        Contains(homepageVM1.SearchText.ToLower()) || u.EventName.ToLower().Contains(homepageVM1.SearchText), includeProperties: "categories"),
                        CategoryListForEvent = _unitOfWork.Category.GetAll()
                    };
                    return View(homepageVM);
                }

            }
            else
            {
                if (homepageVM1 is null || homepageVM1.SearchText is null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        EventList = _unitOfWork.Event.GetAll(u => u.EventStatus == SD.EventStatus_Approved, includeProperties: "categories"),
                        CategoryListForEvent = _unitOfWork.Category.GetAll()
                    };
                    return View(homepageVM);
                }
                if (homepageVM1.SearchText is not null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        EventList = _unitOfWork.Event.GetAll(u => u.EventStatus == SD.EventStatus_Approved && u.EventLocation.ToLower().
                        Contains(homepageVM1.SearchText.ToLower()) || u.EventName.ToLower().Contains(homepageVM1.SearchText), includeProperties: "categories"),
                        CategoryListForEvent = _unitOfWork.Category.GetAll()
                    };
                    return View(homepageVM);
                }
            }
       

            return RedirectToAction("Index");       
        } 
        public IActionResult Users(HomepageVM? homepageVM_)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (homepageVM_.SearchText is not null)
            {
                HomepageVM homepageVM = new HomepageVM()
                {
                    UsersList = _unitOfWork.ApplicationUser.GetAll(U => U.Name.ToLower().Contains(homepageVM_.SearchText.ToLower())),
                };
                return View(homepageVM);
            }
            else
            {
                HomepageVM homepageVM = new HomepageVM()
                {
                    UsersList = _unitOfWork.ApplicationUser.GetAll(),
                };
                return View(homepageVM);
            }
        } 
        public IActionResult Posts(HomepageVM? homepageVM1)
        {
            //getting user id 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null)
            {

                if (homepageVM1 is null || homepageVM1.SearchText == null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        PostList = _unitOfWork.Post.GetAll(includeProperties: "applicationUser,Likes").OrderByDescending(u => u.Time)
                    };
                    return View(homepageVM);
                }
                if (homepageVM1.SearchText is not null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        PostList = _unitOfWork.Post.GetAll(u => u.PostText.ToLower().Contains(homepageVM1.SearchText.ToLower()), includeProperties: "applicationUser,Likes").OrderByDescending(u => u.Time)
                    };
                    return View(homepageVM);
                }
            }
            else
            {
                ApplicationUser User = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Followers,Following");
                if (homepageVM1 is null || homepageVM1.SearchText == null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        FollowedPostList = _unitOfWork.Post.GetAll(u=> User.Following.Contains(u.applicationUser) ,includeProperties: "applicationUser,Likes"),
                        PostList = _unitOfWork.Post.GetAll(includeProperties: "applicationUser,Likes").OrderByDescending(u => u.Time),
                        user = User
                    };
                    return View(homepageVM);
                }
                if (homepageVM1.SearchText is not null)
                {
                    HomepageVM homepageVM = new HomepageVM()
                    {
                        FollowedPostList = _unitOfWork.Post.GetAll(u => User.Following.Contains(u.applicationUser) && u.PostText.ToLower().Contains(homepageVM1.SearchText.ToLower()), includeProperties: "applicationUser,Likes"),
                        PostList = _unitOfWork.Post.GetAll(u => u.PostText.ToLower().Contains(homepageVM1.SearchText.ToLower()), includeProperties: "applicationUser,Likes").OrderByDescending(u => u.Time),
                        user = User
                    };
                    return View(homepageVM);
                }
            }

  

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