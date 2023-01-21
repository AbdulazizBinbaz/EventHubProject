using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepositiory;
using EventHub.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.DataAccess.Repository;

public class CatgeoryRepository : Repository<Category> , ICategoryRepository
{
    private ApplicationDbContext _db;
    public CatgeoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Category obj)
    {
        _db.categories.Update(obj);
    }
}
