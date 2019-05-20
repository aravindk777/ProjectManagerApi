using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace PM.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private ILogger _logger;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            _logger = LogManager.GetCurrentClassLogger();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _logger.Info("Api started...");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error("Unhandled Exception in the Api. Terminating? " + e.IsTerminating, e.ExceptionObject);
        }
    }
}
