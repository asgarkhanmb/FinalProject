﻿using Service.Helpers.Exceptions;
using System.Net;
using System.Text.Json;

namespace FinalProject.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next,
                                           ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ErrorResponse
            {
                Success = false
            };

            switch (exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid Token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        errorResponse.Message = ex.Message;
                        break;
                    }

                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                    break;

                case NotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = ex.Message;
                    break;
                case UnauthorizeException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = ex.Message;
                    break;
                case ForbiddenException ex:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.Message = ex.Message;
                    break;
                case RequiredException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = ex.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Internal server error!";
                    break;
            }
            _logger.LogError(exception.Message);
            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}

