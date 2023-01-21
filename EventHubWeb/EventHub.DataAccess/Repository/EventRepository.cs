using EventHub.DataAccess.Data;
using EventHub.DataAccess.Repository.IRepositiory;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.DataAccess.Repository;

public class EventRepository : Repository<Event> , IEventRepository
{
    private ApplicationDbContext _db;
    public EventRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Event obj)
    {
        _db.events.Update(obj);
    }
}
