using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using EventHubWeb.ViewModels;
using FluentNHibernate.Conventions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Claims;
using System.Web;
using static System.Net.WebRequestMethods;

namespace EventHubWeb.Areas.Individual.Controllers;

[Area(SD.Role_User_Individual)]
public class ProfileController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private IWebHostEnvironment _webHostEnvironment;

    public ProfileController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        //getting user id 
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is not null) 
        {
            var userId = claim.Value;
            AppUserVM AppUserVM = new AppUserVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == userId.ToString(), includeProperties: "Followers,Following,Categories,Posts,Posts.Likes,EventTickets,EventTickets._event"),
                categories = _unitOfWork.Category.GetAll(includeProperties: "ApplicationUsers"),
            };
            return View(AppUserVM);

        }
        return RedirectToPage("/Account/Login", new { area = "Identity" });
    } 
    public IActionResult ProfilePage(string id)
    {
        AppUserVM AppUserVM;
        var userId = id;
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null)
        {
            AppUserVM = new AppUserVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == userId.ToString(), includeProperties: "Followers,Following,Categories,Posts,Posts.Likes,EventTickets,EventTickets._event"),
                categories = _unitOfWork.Category.GetAll(includeProperties: "ApplicationUsers"),
            };
            return View(AppUserVM);
        }
        AppUserVM = new AppUserVM()
        {
              ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(i => i.Id == userId.ToString(), includeProperties: "Followers,Following,Categories,Posts,Posts.Likes,EventTickets,EventTickets._event"),
              categories = _unitOfWork.Category.GetAll(includeProperties: "ApplicationUsers"),
              UserOwner = _unitOfWork.ApplicationUser.GetFirstOrDefault(u=>u.Id==claim.Value,includeProperties: "Following,Likes")
        };
            return View(AppUserVM);
    }
    public IActionResult DeletePost(int PostId)
    {
        //getting user id 
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });

        }
        var postFromDb = _unitOfWork.Post.GetFirstOrDefault(u => u.PostId == PostId, includeProperties: "Likes,Likes.User");
        if (postFromDb.PostImageUrl is not null)
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, postFromDb.PostImageUrl.Trim('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        if(claim.Value == postFromDb.UserId || User.IsInRole(SD.Role_Admin))
        {
            var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Likes");
            postFromDb.Likes.Clear();
            _unitOfWork.Post.Update(postFromDb);
            userFromDb.Posts.Remove(postFromDb);
            _unitOfWork.Post.Remove(postFromDb);
            _unitOfWork.Save();
        }
        return RedirectToAction("Index");
    }

    public IActionResult UpdateDes(AppUserVM appUserVM)
    {
        var AppUserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == appUserVM.ApplicationUser.Id);
        AppUserFromDb.Description = appUserVM.ApplicationUser.Description;
        _unitOfWork.ApplicationUser.Update(AppUserFromDb);
        _unitOfWork.Save();
        return RedirectToAction("Index");
    }
    public IActionResult FollowingList(string id)
    {
        var AppUserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id,includeProperties: "Following");
        return View(AppUserFromDb);
    }
    public IActionResult FollowersList(string id)
    {
        var AppUserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id,includeProperties: "Followers,Following");
        return View(AppUserFromDb);
    }
    [HttpGet]
    public IActionResult ListCategories(string UserId)
    {
        AppUserVM appUserVM = new AppUserVM()
        {
            ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == UserId, includeProperties: "Categories"),
            categories = _unitOfWork.Category.GetAll(includeProperties: "ApplicationUsers"),
        };
        return View(appUserVM);
    }
    public IActionResult FollowCat(string UserId,int CatId)
    {
        AppUserVM appUserVM = new AppUserVM()
        {
            ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == UserId, includeProperties: "Categories"),
            categories = _unitOfWork.Category.GetAll(includeProperties: "ApplicationUsers"),
        };
        var selectedCategory = _unitOfWork.Category.GetFirstOrDefault(c => c.CategoryId == CatId, includeProperties: "ApplicationUsers");
        appUserVM.ApplicationUser.Categories.Add(selectedCategory);
        _unitOfWork.ApplicationUser.Update(appUserVM.ApplicationUser);
        _unitOfWork.Save();

        return RedirectToAction("Index");
    }
    public IActionResult UnFollowCat(string UserId, int CatId)
    {
        AppUserVM appUserVM = new AppUserVM()
        {
            ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == UserId, includeProperties: "Categories"),
            categories = _unitOfWork.Category.GetAll(includeProperties: "ApplicationUsers"),
        };
        var selectedCategory = _unitOfWork.Category.GetFirstOrDefault(c => c.CategoryId == CatId, includeProperties: "ApplicationUsers");
        appUserVM.ApplicationUser.Categories.Remove(selectedCategory);
        _unitOfWork.ApplicationUser.Update(appUserVM.ApplicationUser);
        _unitOfWork.Save();

        return RedirectToAction("Index");
    }
    public IActionResult CreatePost(AppUserVM appUserVM,IFormFile? file)
    {
        appUserVM.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == appUserVM.ApplicationUser.Id);

        if (appUserVM.Post.PostText is null) 
        {
            return RedirectToAction("Index");
        }
        if (ModelState.IsValid)
        {
            if (file is not null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, "uploads", "posts", "images");
                var extension = Path.GetExtension(file.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                appUserVM.Post.PostImageUrl = @"uploads\posts\images\" + fileName + extension;
            }
            appUserVM.Post.Time = DateTime.Now;
            _unitOfWork.Post.Add(appUserVM.Post);
            
            _unitOfWork.Save();
            appUserVM.ApplicationUser.Posts.Add(appUserVM.Post);
            _unitOfWork.ApplicationUser.Update(appUserVM.ApplicationUser);
            _unitOfWork.Save();
        }

        return RedirectToAction("Index");
    }
    public IActionResult Follow(string UserId) 
    {
        //getting user id 
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if(claim is null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        else
        {
            var UserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Following");
            var UserToFollowFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == UserId, includeProperties: "Followers");
            UserFromDb.Following.Add(UserToFollowFromDb);
            UserToFollowFromDb.Followers.Add(UserFromDb);
            _unitOfWork.Save();
            return RedirectToAction("ProfilePage", new { id = UserId });
        }
    }
    public IActionResult UnFollow(string UserId) 
    {
        //getting user id 
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if(claim is null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        else
        {
            var UserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Following");
            var UserToFollowFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == UserId, includeProperties: "Followers");
            UserFromDb.Following.Remove(UserToFollowFromDb);
            UserToFollowFromDb.Followers.Remove(UserFromDb);
            _unitOfWork.Save();
            return RedirectToAction("ProfilePage", new { id = UserId });
        }
    }

    }        
