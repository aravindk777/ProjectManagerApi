using PM.BL.Common;
using PM.Data.Repos.Users;
using PM.Models.ViewModels;
using System.Collections.Generic;

namespace PM.BL.Users
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepository userRepository;

        public UserLogic(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        public User AddUser(Models.ViewModels.User user)
        {
            return userRepository.Create(user.AsDataModel()).AsViewModel();
        }

        public bool DeleteUser(string UserId)
        {
            return userRepository.Delete(userRepository.GetById(UserId));
        }

        public bool EditUser(string UserId, Models.ViewModels.User userViewModel)
        {
            if (userRepository.GetById(UserId) != null)
                return userRepository.Update(userViewModel.AsDataModel());
            else
                return false;
        }

        public User GetUserById(string UserId)
        {
            return userRepository.GetById(UserId).AsViewModel();
        }

        public IEnumerable<Models.ViewModels.User> GetUsers()
        {
            return userRepository.GetAll().AsViewModel();
        }
    }
}
