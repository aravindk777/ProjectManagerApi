using PM.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Data.Repos.Users
{
    public interface IUserRepository : IRepository<Models.DataModel.User>
    {
        bool DeleteUser(string userId);
    }
}
