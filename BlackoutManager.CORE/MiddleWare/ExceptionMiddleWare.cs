using BlackoutManager.CORE.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlackoutManager.CORE.MiddleWare;

public class ExceptionMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleWare> _logger;

    public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext content)
    {
        try
        {
            await _next(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong while processing {content.Request.Path}");
            await HandleExceptionAsync(content, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext content, Exception ex)
    {
        content.Response.ContentType = "application/json";
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        var errorDetails = new ErrorDetails
        {
            ErrorType = "Failure",
            ErrorMessage = ex.Message
        };

        switch (ex)
        {
            default:
                break;
        }

        string response = JsonConvert.SerializeObject(errorDetails);
        content.Response.StatusCode = (int)statusCode;
        return content.Response.WriteAsync(response);

    }
}
