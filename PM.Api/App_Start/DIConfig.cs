using Microsoft.Extensions.DependencyInjection;
using PM.Api.Controllers;
using PM.BL.User;
using PM.Data.Entities;
using PM.Data.Repos;
using PM.Data.Repos.Project;
using PM.Data.Repos.User;
using PM.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace PM.Api.App_Start
{
    public class DIConfig
    {
        /// <summary>
        /// Configure Dependency injection using Microsoft Dependency injection
        /// </summary>
        /// <returns>Service provider instance</returns>
        public static IServiceProvider SetupInjection()
        {
            var injectors = new ServiceCollection()
                // ---- Db connections ----
                .AddScoped<PMDbContext, PMDbContext>()
                //.AddScoped<IDbConnect<TeradataDbConnect>, TeradataDbConnect>()

                // ---- Repositories ----
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRepository<Users>, Repository<Users>>()
                .AddScoped<IProjectRepo, ProjectRepo>()
                .AddScoped<IRepository<Projects>, Repository<Projects>>()
                //.AddScoped<ITdDataRepository, TeraDataRepository>()


                // ---- Service Providers ----
                .AddScoped<IUserLogic, UserLogic>()
                //.AddScoped<IDataService, DataService>()

                // ---- Logging ----
                //.AddLogging(log =>
                //{
                //    log.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                //    SetupLogging();
                //})

                // ---- API Controllers ----
                .AddScoped<UsersController, UsersController>()
                ;
            return injectors.BuildServiceProvider();
        }
    }

    /// <summary>
    /// Asp.net web api dependency resolver extension for DI injections to work. 
    /// </summary>
    /// <remarks>Referenced from StackOverflow. https://stackoverflow.com/a/50711651/548659
    /// </remarks>
    public class ApiDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Service Provider object
        /// </summary>
        protected IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Instantiating the DI provider
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ApiDependencyResolver(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceProvider.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return new ApiDependencyResolver(ServiceProvider.CreateScope().ServiceProvider);
        }

        public void Dispose()
        {
            // Not yet needed
        }
    }
}