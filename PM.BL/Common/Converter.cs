using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.BL.Common
{
    public static class Converter
    {
        #region DataModel to ViewModel conversions
        public static PM.Models.ViewModels.Projects AsViewModel(this PM.Models.DataModel.Projects projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModel.Projects, PM.Models.ViewModels.Projects>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.DataModel.Projects, PM.Models.ViewModels.Projects>(projectData);
        }

        public static IEnumerable<PM.Models.ViewModels.Projects> AsViewModel(this IEnumerable<PM.Models.DataModel.Projects> projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModel.Projects, PM.Models.ViewModels.Projects>().ReverseMap();
            });

            return config.CreateMapper().Map< IEnumerable<PM.Models.DataModel.Projects>, IEnumerable<PM.Models.ViewModels.Projects>>(projectData);
        }

        public static PM.Models.ViewModels.Users AsViewModel(this PM.Models.DataModel.Users userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModel.Users, PM.Models.ViewModels.Users>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.DataModel.Users, PM.Models.ViewModels.Users>(userData);
        }

        public static IEnumerable<PM.Models.ViewModels.Users> AsViewModel(this IEnumerable<PM.Models.DataModel.Users> userData)
        {
            var config = new MapperConfiguration(cfg => {                
                cfg.CreateMap<PM.Models.DataModel.Users, PM.Models.ViewModels.Users>().ReverseMap();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<IEnumerable<PM.Models.DataModel.Users>, IEnumerable<PM.Models.ViewModels.Users>>(userData);
        }
        #endregion

        #region ViewModel to Data Model conversions
        public static PM.Models.DataModel.Projects AsDataModel(this PM.Models.ViewModels.Projects projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.Projects, PM.Models.DataModel.Projects>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.ViewModels.Projects, PM.Models.DataModel.Projects>(projectData);
        }

        public static IEnumerable<PM.Models.DataModel.Projects> AsDataModel(this IEnumerable<PM.Models.ViewModels.Projects> projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<IEnumerable<PM.Models.ViewModels.Projects>, IEnumerable<PM.Models.DataModel.Projects>>().ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.ViewModels.Projects>, IEnumerable<PM.Models.DataModel.Projects>>(projectData);
        }

        public static PM.Models.DataModel.Users AsDataModel(this PM.Models.ViewModels.Users userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.Users, PM.Models.DataModel.Users>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.ViewModels.Users, PM.Models.DataModel.Users>(userData);
        }

        public static IEnumerable<PM.Models.DataModel.Users> AsDataModel(this IEnumerable<PM.Models.ViewModels.Users> userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<IEnumerable<PM.Models.ViewModels.Users>, IEnumerable<PM.Models.DataModel.Users>>().ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.ViewModels.Users>, IEnumerable<PM.Models.DataModel.Users>>(userData);
        }
        #endregion
    }
}
