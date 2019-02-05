using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.BL.User
{
    public interface IUserLogic
    {
        PM.Models.ViewModels.Users AddUser(PM.Models.ViewModels.Users user);
        IEnumerable<PM.Models.ViewModels.Users> GetUsers();
        bool EditUser(string UserId, PM.Models.ViewModels.Users userViewModel);
        bool DeleteUser(string UserId);
        PM.Models.ViewModels.Users GetUserById(string UserId);
    }
}
