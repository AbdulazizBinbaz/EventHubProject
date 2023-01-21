using EventHub.DataAccess.Repository.IRepositiory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{

    ICategoryRepository Category { get; }
    IEventRepository Event { get; }
    IApplicationUserRepository ApplicationUser { get; }
    IPostRepository Post { get; }

    void Save();



}
