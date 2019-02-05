using AutoMapper;
using PM.BL.Common;
using PM.Data.Repos.User;
using PM.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace PM.BL.User
{
    public class UserLogic : BaseLogic, IUsersLogic
    {
        private readonly IUserRepository userRepository;
        private IMapper mapper;

        public UserLogic(IUserRepository _userRepository) : base()
        {
            userRepository = _userRepository;
            mapper = base.InitializeMapping();

        }

        public void AddUser(Users user)
        {
            var userModel = mapper.Map<PM.Models.ViewModels.Users, PM.Models.DataModel.Users>(user);
            var result = userRepository.Create(userModel);
        }

        public bool DeleteUser(string UserId)
        {
            throw new NotImplementedException();
        }

        public bool EditUser(string UserId, Users userViewModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Users> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
