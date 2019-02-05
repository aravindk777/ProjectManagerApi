using AutoMapper;
using PM.BL.Common;
using PM.Data.Repos.User;
using PM.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace PM.BL.User
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepository userRepository;

        public UserLogic(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        public Users AddUser(Users user)
        {
            return userRepository.Create(user.AsDataModel()).AsViewModel();
        }

        public bool DeleteUser(string UserId)
        {
            return userRepository.Delete(userRepository.GetById(UserId));
        }

        public bool EditUser(string UserId, Users userViewModel)
        {
            return userRepository.Update(userViewModel.AsDataModel());
        }

        public Users GetUserById(string UserId)
        {
            return userRepository.GetById(UserId).AsViewModel();
        }

        public IEnumerable<Users> GetUsers()
        {
            return userRepository.GetAll().AsViewModel();
        }
    }
}
