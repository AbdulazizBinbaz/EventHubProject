using EventHub.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.DataAccess.Repository.IRepositiory
{
    public interface IPostRepository : IRepository<Post>
    {
        void Update(Post obj);
    }
}
