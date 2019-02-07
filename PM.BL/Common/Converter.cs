using AutoMapper;
using System.Collections.Generic;

namespace PM.BL.Common
{
    public static class Converter
    {
        #region DataModel to ViewModel conversions
        public static PM.Models.ViewModels.Project AsViewModel(this PM.Models.DataModel.Project projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModel.Project, PM.Models.ViewModels.Project>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.DataModel.Project, PM.Models.ViewModels.Project>(projectData);
        }

        public static IEnumerable<PM.Models.ViewModels.Project> AsViewModel(this IEnumerable<PM.Models.DataModel.Project> projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModel.Project, PM.Models.ViewModels.Project>().ReverseMap();
            });

            return config.CreateMapper().Map< IEnumerable<PM.Models.DataModel.Project>, IEnumerable<PM.Models.ViewModels.Project>>(projectData);
        }

        public static PM.Models.ViewModels.User AsViewModel(this PM.Models.DataModel.User userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.DataModel.User, PM.Models.ViewModels.User>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.DataModel.User, PM.Models.ViewModels.User>(userData);
        }

        public static IEnumerable<PM.Models.ViewModels.User> AsViewModel(this IEnumerable<PM.Models.DataModel.User> userData)
        {
            var config = new MapperConfiguration(cfg => {                
                cfg.CreateMap<PM.Models.DataModel.User, PM.Models.ViewModels.User>().ReverseMap();
            });
            var mapper = config.CreateMapper();
            return mapper.Map<IEnumerable<PM.Models.DataModel.User>, IEnumerable<PM.Models.ViewModels.User>>(userData);
        }
        #endregion

        #region ViewModel to Data Model conversions
        public static PM.Models.DataModel.Project AsDataModel(this PM.Models.ViewModels.Project projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.Project, PM.Models.DataModel.Project>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.ViewModels.Project, PM.Models.DataModel.Project>(projectData);
        }

        public static IEnumerable<PM.Models.DataModel.Project> AsDataModel(this IEnumerable<PM.Models.ViewModels.Project> projectData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<IEnumerable<PM.Models.ViewModels.Project>, IEnumerable<PM.Models.DataModel.Project>>().ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.ViewModels.Project>, IEnumerable<PM.Models.DataModel.Project>>(projectData);
        }

        public static PM.Models.DataModel.User AsDataModel(this PM.Models.ViewModels.User userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PM.Models.ViewModels.User, PM.Models.DataModel.User>().ReverseMap();
            });

            return config.CreateMapper().Map<PM.Models.ViewModels.User, PM.Models.DataModel.User>(userData);
        }

        public static IEnumerable<PM.Models.DataModel.User> AsDataModel(this IEnumerable<PM.Models.ViewModels.User> userData)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<IEnumerable<PM.Models.ViewModels.User>, IEnumerable<PM.Models.DataModel.User>>().ReverseMap();
            });

            return config.CreateMapper().Map<IEnumerable<PM.Models.ViewModels.User>, IEnumerable<PM.Models.DataModel.User>>(userData);
        }
        #endregion
    }
}
