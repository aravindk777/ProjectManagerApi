using PM.Data.Entities;
using PM.Models.DataModel;
using System.Linq;

namespace PM.Data.Repos.User
{
    public class UserRepository : Repository<Users>, IUserRepository
    {
        public UserRepository(PMDbContext dbContext) : base(dbContext) { }

        public override Users GetById(object identifier)
        {
            return Search(user => user.UserId.ToLower().Equals(identifier.ToString().ToLower())).FirstOrDefault();
        }
    }
}
