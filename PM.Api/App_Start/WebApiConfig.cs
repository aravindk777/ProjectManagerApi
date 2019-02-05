using PM.Api.App_Start;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PM.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();

            // Configure DI
            config.DependencyResolver = new ApiDependencyResolver(DIConfig.SetupInjection());

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Enable Swagger as always base URL for this api
            config.Routes.MapHttpRoute(
                name: "swagger_root",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
