using AutoMapper;
using PM.BL.User;
using PM.Models.DataModel;
using PM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.BL.Common
{
    public class BaseLogic
    {
        public BaseLogic()
        {
        }

        public IMapper InitializeMapping()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.Users, PM.Models.DataModel.Users>().ReverseMap();
                cfg.CreateMap<PM.Models.ViewModels.Projects, PM.Models.DataModel.Projects>().ReverseMap();
            });
            return config.CreateMapper();
        }
    }
}
