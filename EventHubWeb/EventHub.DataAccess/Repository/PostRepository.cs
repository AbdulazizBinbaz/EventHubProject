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

public class PostRepository : Repository<Post> , IPostRepository
{
    private ApplicationDbContext _db;
    public PostRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Post obj)
    {
        _db.posts.Update(obj);
    }

}
