using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Northwind.WebApi.ModelsExt.Errors;
using Northwind.WebApi.ModelsExt.ModelAuthentic;
using NuGet.Protocol;
using System.Net;
using System.Text;

namespace Northwind.WebApi.ServiceExt
{
    internal static class Extensions
    {
        internal static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(el =>
            {
                el.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Northwind API", Version = "1.0" });
                el.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                el.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme="Bearer",
                Name="Bearer",
                In=ParameterLocation.Header
            },
            new string[]{}
        }
    });
                el.ResolveConflictingActions(api => api.First());
            });
        }
        internal static void AddJwtAuthentication(
            this IServiceCollection services, JWTSettings settings)
        {
            var key = Encoding.ASCII.GetBytes((settings != null ? settings.SecretKey : string.Empty) ?? string.Empty);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = c =>
        {
            if (c.Exception is SecurityTokenExpiredException exc)
            {
               
                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                c.Response.ContentType = "application/json";
                var detailDic = new Dictionary<string, string>() { { "ExpiredTokenDate", exc.Expires.ToString("dd/MM/yyyy HH:mm") },{"Message",exc.ToJson()}  };
                var result = JsonConvert.SerializeObject(new ApiError("TheTokenIsExpired",JsonConvert.SerializeObject(detailDic)));
                return c.Response.WriteAsync(result);
            }
            else
            {
#if DEBUG
                c.NoResult();
                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                c.Response.ContentType = "text/plain";
                return c.Response.WriteAsync(c.Exception.ToString());
#else
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(new ApiException("UnHandledException"));
                                return c.Response.WriteAsync(result);
#endif
            }
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new ApiException("YouNotAuthorized"));
                return context.Response.WriteAsync(result);
            }

            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new ApiException("Forbidden"));
            return context.Response.WriteAsync(result);
        },
    };

});
        }
    }
}
