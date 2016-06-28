using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomErrorPages
{
    public class CustomErrorPagesMiddleware
    {
        IHostingEnvironment _env;
        RequestDelegate _next;
        private readonly ILogger _logger;
        private static readonly IDictionary<int, string> _errorPages = new Dictionary<int, string>();

        public CustomErrorPagesMiddleware(IHostingEnvironment env, ILoggerFactory loggerFactory, RequestDelegate next)
        {
            _env = env;
            _next = next;
            _logger = loggerFactory.CreateLogger<CustomErrorPagesMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "An unhandled exception has occurred while executing the request");

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error page middleware will not be executed.");
                    throw;
                }
                try
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 500;
                    return;
                }
                catch (Exception ex2)
                {
                    // If there's a Exception while generating the error page, re-throw the original exception. 
                    _logger.LogError(0, ex2, "An exception was thrown attempting to display the error page.");
                }
                throw;
            }
            finally
            {
                var statusCode = context.Response.StatusCode;
                if (Startup.ErrorPages.Keys.Contains(statusCode))
                {
                    context.Request.Path = Startup.ErrorPages[statusCode];
                    await _next(context);
                }
            }
            //try
            //{
            //    await _next(context);
            //}
            //catch (Exception ex)
            //{
            //    context.Response.Clear();
            //    context.Response.StatusCode = 500;
            //    var config = new ConfigurationBuilder()
            //    .SetBasePath(_env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .Build();

            //    foreach (var c in config.GetSection("ErrorPages").GetChildren())
            //    {
            //        var key = Convert.ToInt32(c.Key);
            //        if (!_errorPages.Keys.Contains(key))
            //        {
            //            _errorPages.Add(key, c.Value);
            //        }
            //    }

            //    if (_errorPages.Keys.Contains(context.Response.StatusCode))
            //    {
            //        context.Request.Path = _errorPages[context.Response.StatusCode];
            //        await _next(context);
            //    }
            //}
        }
    }
}
