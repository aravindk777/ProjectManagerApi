using AutoMapper;
using PM.BL.Common;
using PM.Data.Repos.User;
using PM.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace PM.BL.User
{
    public class UserLogic : BaseLogic, IUserLogic
    {
        private readonly IUserRepository userRepository;
        private IMapper mapper;

        public UserLogic(IUserRepository _userRepository) : base()
        {
            userRepository = _userRepository;
            mapper = base.InitializeMapping();

        }

        public Users AddUser(Users user)
        {
            var userModel = mapper.Map<Users, Models.DataModel.Users>(user);
            var result = userRepository.Create(userModel);
            return mapper.Map<Models.DataModel.Users, Users>(result);
        }

        public bool DeleteUser(string UserId)
        {
            throw new NotImplementedException();
        }

        public bool EditUser(string UserId, Users userViewModel)
        {
            throw new NotImplementedException();
        }

        public Users GetUserById(string UserId)
        {
            var result = userRepository.GetById(UserId);
            return mapper.Map<Models.DataModel.Users, Users>(result);
        }

        public IEnumerable<Users> GetUsers()
        {
            return mapper.Map<IEnumerable<Models.DataModel.Users>, IEnumerable<Users>>(userRepository.GetAll());
        }
    }
}
