using PM.Data.Entities;
using PM.Models.DataModel;
using System.Linq;

namespace PM.Data.Repos.Users
{
    public class UserRepository : Repository<Models.DataModel.User>, IUserRepository
    {
        public UserRepository(PMDbContext dbContext) : base(dbContext) { }

        public override User GetById(object identifier)
        {
            return Search(user => user.UserId.ToLower().Equals(identifier.ToString().ToLower())).FirstOrDefault();
        }
    }
}
