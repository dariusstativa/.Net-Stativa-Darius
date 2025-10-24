using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lab04.Middleware
{
    public class CorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger< CorrelationMiddleware> _logger;

        public  CorrelationMiddleware(RequestDelegate next, ILogger< CorrelationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An unhandled exception occurred. TraceId: {TraceId}",
                    context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private  async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            ErrorResponse errorResponse;
            int statusCode;

            switch (ex)
            {
                case InvalidOperationException vex:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new ErrorResponse("VALIDATION_ERROR", vex.Message);
                    break;

                case DbUpdateException dbEx:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new ErrorResponse("DATABASE_ERROR", dbEx.Message);
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new ErrorResponse("SERVER_ERROR", "An unexpected error occurred.");
                    break;
            }

            errorResponse.TraceId = context.TraceIdentifier;
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }

        public class ErrorResponse
        {
            public ErrorResponse() { }
            public ErrorResponse(string errorCode, string errorMessage) : this()
            {
                ErrorCode = errorCode;
                ErrorMessage = errorMessage;
            }

            public string TraceId { get; set; } = string.Empty;
            public string ErrorMessage { get; set; } = string.Empty;
            public string ErrorCode { get; set; } = string.Empty;
        }
    }
}
