﻿using Microsoft.Extensions.Logging;
using Proo.APIs.Errors;
using System.Net;
using System.Text.Json;

namespace Proo.APIs.Middlewares
{
    public class ExeptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHostEnvironment _env;

        public ExeptionMiddleware(RequestDelegate next , ILoggerFactory loggerFactory , IHostEnvironment env)
        {
            _next = next;
            _loggerFactory = loggerFactory;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext); // go to next middlware
                                                 // Take an Action With the Response 
            }
            catch (Exception ex)
            {
                // logg Exeption 
                var logger = _loggerFactory.CreateLogger<ExeptionMiddleware>();
                logger.LogError(ex.Message);

                // return Response itself [header , body]

                httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment() ? new ApiExeptionResponse((int)HttpStatusCode.InternalServerError,
                    ex.Message, ex.StackTrace.ToString())
                    : new ApiExeptionResponse((int)HttpStatusCode.InternalServerError);

                var json = JsonSerializer.Serialize(response);

                await httpContext.Response.WriteAsync(json);
            }

            
        }
    }
}
