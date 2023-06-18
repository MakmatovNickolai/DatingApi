using Common.Exceptions;
using Common.Models;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;

namespace DatingApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadRequestException brex)
        {
            await EnrichResponse(context.Response, StatusCodes.Status400BadRequest, new ErrorResponse
            {
                Success = false,
                Code = "BAD_REQUEST",
                Message = brex.Message
            });
        }
        catch (UnverifiedException)
        {
            await EnrichResponse(context.Response, StatusCodes.Status400BadRequest, new ErrorResponse
            {
                Success = false,
                Code = "UNVERIFIED_USER",
                Message = "User didn't verify provided email"
            });
        }
        catch (DuplicatedObjectException)
        {
            await EnrichResponse(context.Response, StatusCodes.Status403Forbidden, new ErrorResponse
            {
                Success = false,
                Code = "UNIQUE_VIOLATION",
                Message = "Such entity already exists"
            });
        }
        catch (UnauthorizedAccessException)
        {
            await EnrichResponse(context.Response, StatusCodes.Status401Unauthorized, new ErrorResponse
            {
                Success = false,
                Code = "UNAUTHORIZED_ACCESS",
                Message = "Access is unauthorized"
            });
        }
        catch (InvalidJwtException ex)
        {
            await EnrichResponse(context.Response, StatusCodes.Status400BadRequest, new ErrorResponse
            {
                Success = false,
                Code = "INVALID_JWT",
                Message = ex.Message
            });
        }
        catch (SecurityTokenException ex)
        {
            await EnrichResponse(context.Response, StatusCodes.Status400BadRequest, new ErrorResponse
            {
                Success = false,
                Code = "INVALID_TOKEN",
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetService<ILogger<ExceptionMiddleware>>();
            logger!.LogError($"{ex.Message}\r\n{ex.StackTrace}");
            await EnrichResponse(context.Response, StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Success = false,
                Code = "INTERNAL",
                Message = ex.Message
            });
        }
    }

    private async Task EnrichResponse(HttpResponse response, int statusCode, ErrorResponse error)
    {
        response.StatusCode = statusCode;
        response.ContentType = MediaTypeNames.Application.Json;

        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        await response.WriteAsync(JsonConvert.SerializeObject(error, new JsonSerializerSettings
        {
            ContractResolver = contractResolver
        }));
    }
}
