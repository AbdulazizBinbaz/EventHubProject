using BulkyBook.DataAccess.Repository.IRepository;
using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepositiory;
using EventHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _db;
    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Event = new EventRepository(_db);
        Category = new CatgeoryRepository(_db);
        ApplicationUser = new ApplicationUserRepository(_db);
        Post = new PostRepository(_db);
    }

    public ICategoryRepository Category { get; private set; }
    public IEventRepository Event { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }
    public IPostRepository Post { get; private set; }



    public void Save()
    {
        _db.SaveChanges();
    }
}
