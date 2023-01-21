using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.Models;
using EventHub.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Data;
using System.Security.Claims;

namespace EventHubWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
 
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: EventController

        public ActionResult Index()
        {
            IEnumerable<Category> CategoryList = _unitOfWork.Category.GetAll();
            return View(CategoryList);
            
        }

        // GET: EventController/Details/5
        public ActionResult Details(int id)
        {
            Category CategoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryId == id);
            return View(CategoryFromDb);
        }

        public ActionResult Create()
        {
            return View();
        }
        // POST: EventController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(obj);

        }
        // GET: EventController/Edit/5
        public ActionResult Edit(int id)
        {
            Category CategoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryId == id);
            return View(CategoryFromDb);
        }

        // POST: EventController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);

        }

        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            Category CategoryFromDb = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryId == id);
            return View(CategoryFromDb);
        }

        // POST: EventController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Category obj)
        {
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}

