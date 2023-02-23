using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using EventHubWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventHubWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public UsersController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }



        // GET: UsersController/Edit/5
        public ActionResult Edit(string id)
        {
            AppUserVM appUserVM = new AppUserVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id)
            };

            return View(appUserVM);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppUserVM appUserVM, IFormFile? file)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //------------------------Update Picture------------------------------
            if (file is not null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                string uploads = Path.Combine(wwwRootPath, "uploads", "profiles", "images");
                var extension = Path.GetExtension(file.FileName);
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                if (appUserVM.ApplicationUser.ProfilePictureUrl is not null) 
                { 
                    var oldImagePath = Path.Combine(wwwRootPath, appUserVM.ApplicationUser.ProfilePictureUrl.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                    System.IO.File.Delete(oldImagePath);
                    }
                }

                appUserVM.ApplicationUser.ProfilePictureUrl = @"uploads\profiles\images\" + fileName + extension;
                //--------------------------------------------------------------------
            }

            var userfromdb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u=> u.Id == appUserVM.ApplicationUser.Id);
            userfromdb.Name = appUserVM.ApplicationUser.Name;
            userfromdb.Email = appUserVM.ApplicationUser.Email;
            userfromdb.ProfilePictureUrl = appUserVM.ApplicationUser.ProfilePictureUrl;
            userfromdb.PhoneNumber = appUserVM.ApplicationUser.PhoneNumber;

            
            _unitOfWork.Save();
            if(claim?.Value == userfromdb.Id)
            {
                return RedirectToAction("Index", "Profile", new { area = "Individual" });

            }

            return RedirectToAction("ProfilePage", "Profile", new { area="Individual" , id = appUserVM.ApplicationUser.Id });
        }

            // GET: UsersController/Delete/5
            public ActionResult Delete(string id)
        {
            AppUserVM appUserVM = new AppUserVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id)
            };

            return View(appUserVM);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AppUserVM appUserVM)
        {
            if (User.IsInRole(SD.Role_Admin))
            {
                var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == appUserVM.ApplicationUser.Id,
                    includeProperties: "Followers,Following,Categories,Posts,Posts.Likes,UserComments,Likes,EventTickets");
                if (userFromDb.ProfilePictureUrl is not null)
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, userFromDb.ProfilePictureUrl.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                if(userFromDb.Posts is not null)
                {
                    foreach (var post in userFromDb.Posts)
                    {
                        if (post.Likes.Any()) ;
                        post.Likes.Clear(); 
                    }
                }
                userFromDb.Posts?.Clear();
                userFromDb.Likes?.Clear();
                userFromDb.Following?.Clear();
                userFromDb.Followers?.Clear();
                userFromDb.UserComments?.Clear();
                userFromDb.Categories?.Clear();
                userFromDb.EventTickets?.Clear();


                _unitOfWork.ApplicationUser.Remove(userFromDb);
                _unitOfWork.Save();
            }

            return RedirectToAction("Users", "Home", new { area = "Individual" });
        }
    }
}
