﻿using Microsoft.Extensions.DependencyInjection;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using PM.Api.Controllers;
using PM.BL.Projects;
using PM.BL.Tasks;
using PM.BL.Users;
using PM.Data.Entities;
using PM.Data.Repos;
using PM.Data.Repos.Projects;
using PM.Data.Repos.Tasks;
using PM.Data.Repos.Users;
using PM.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                .AddScoped<IRepository<User>, Repository<User>>()
                .AddScoped<IProjectRepo, ProjectRepo>()
                .AddScoped<IRepository<Project>, Repository<Project>>()
                .AddScoped<ITaskRepository, TaskRepository>()
                .AddScoped<IRepository<Task>, Repository<Task>>()


                // ---- Service Providers ----
                .AddScoped<IUserLogic, UserLogic>()
                .AddScoped<IProjectLogic, ProjectLogic>()
                .AddScoped<ITaskLogic, TaskLogic>()

                // ---- Logging ----
                .AddLogging(log =>
                {
                    log.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                    SetupLogging();
                })

                // ---- API Controllers ----
                .AddScoped<UsersController, UsersController>()
                .AddScoped<ProjectsController, ProjectsController>()
                .AddScoped<HealthController, HealthController>()
                .AddScoped<TasksController, TasksController>()
                ;
            return injectors.BuildServiceProvider();
        }

        static void SetupLogging()
        {
            // Initialize the Logger
            var nlogConfig = new LoggingConfiguration();

            // Targets
            var fileTarget = new FileTarget("FileTarget")
            {
                //ArchiveAboveSize = 1024 * 1024,
                ArchiveEvery = FileArchivePeriod.Day,
                CreateDirs = true,
                FileName = ConfigurationManager.AppSettings["logfile"],
                Layout = ConfigurationManager.AppSettings["loglayout"],
                ArchiveNumbering = ArchiveNumberingMode.Sequence,                
                Header = NLog.Layouts.Layout.FromString("_________________________________"),
                Footer = NLog.Layouts.Layout.FromString("=================================")
            };
            nlogConfig.AddTarget(fileTarget);
            nlogConfig.AddRuleForAllLevels(fileTarget);

            // Setup the configuration
            NLog.LogManager.Configuration = nlogConfig;
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