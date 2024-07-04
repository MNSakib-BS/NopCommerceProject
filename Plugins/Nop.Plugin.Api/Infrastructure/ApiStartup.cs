using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Authorization.Policies;
using Nop.Plugin.Api.Authorization.Requirements;
using Nop.Plugin.Api.Configuration;
using Nop.Web.Framework.Infrastructure.Extensions;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Maps;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Plugin.Arch.Core.Services.Helpers;


namespace Nop.Plugin.Api.Infrastructure
{
    public class ApiStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var apiConfigSection = configuration.GetSection("Api");

            if (apiConfigSection != null)
            {
                var apiConfig = services.ConfigureStartupConfig<ApiConfiguration>(apiConfigSection);

                if (!string.IsNullOrEmpty(apiConfig.SecurityKey))
                {
                    services.AddAuthentication(options =>
                            {
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
                            {
                                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiConfig.SecurityKey)),
                                    ValidateIssuer = false, // ValidIssuer = "The name of the issuer",
                                    ValidateAudience = false, // ValidAudience = "The name of the audience",
                                    ValidateLifetime = true, // validate the expiration and not before values in the token
                                    ClockSkew = TimeSpan.FromMinutes(apiConfig.AllowedClockSkewInMinutes)
                                };
                            });

                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    AddAuthorizationPipeline(services);

                    //services.AddSwaggerGen();
                    //services.AddSwaggerGen(c =>
                    //{
                    //    c.SwaggerDoc("v1", new OpenApiInfo
                    //    {
                    //        Version = "v1",
                    //        Title = "API",
                    //        Description = "Arch eStore API",
                    //    });
                    //    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    //});
                }
            }

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            }); 
            
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true; // make the available api versions visible for the client
                opt.AssumeDefaultVersionWhenUnspecified = true; // automaticaly use api_version=1.0 in case it was not specified
                opt.DefaultApiVersion = ApiVersion.Default; // default version of 1.0 to the api
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            var rewriteOptions = new RewriteOptions()
                .AddRewrite("api/token", "/token", true);

            app.UseRewriter(rewriteOptions);

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Need to enable rewind so we can read the request body multiple times
            // This should eventually be refactored, but both JsonModelBinder and all of the DTO validators need to read this stream.
            //app.UseWhen(x => x.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase),
            //            builder =>
            //            {
            //                builder.Use(async (context, next) =>
            //                {
            //                    Console.WriteLine("API Call");
            //                    context.Request.EnableBuffering();
            //                    await next();
            //                });
            //            });

            app.MapWhen(
                (context) =>
                {
                    return context.Request.Path.StartsWithSegments(new PathString("/api"));
                },
                a =>
                {
                    
                    a.Use(async (context, next) =>
                                {
                                    Console.WriteLine("API Call");
                                    context.Request.EnableBuffering();
                                    await next();
                                });

                    a.UseExceptionHandler("/api/error/500/Error");

                    a.UseRouting();
                    a.UseAuthentication();
                    a.UseAuthorization();
                    a.UseEndpoints(endpoints =>
                    {
                        endpoints
                            .MapControllers();
                    });
                }
            );

            //app.UseSwagger();
            //app.UseSwaggerUI(c => {
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arch eStore API");
            //});
        }

        public int Order => 1;

        private static void AddAuthorizationPipeline(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
                                  policy =>
                                  {
                                      policy.Requirements.Add(new ActiveApiPluginRequirement());
                                      policy.Requirements.Add(new AuthorizationSchemeRequirement());
                                      policy.Requirements.Add(new CustomerRoleRequirement());
                                      policy.RequireAuthenticatedUser();
                                  });
            });

            services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, CustomerRoleAuthorizationPolicy>();


            //from dependency register


            //services.AddScoped<IClientService, ClientService>(); 
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IArchService, ArchService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IDriverApiService, DriverApiService>();
            services.AddScoped<ICustomerApiService, CustomerApiService>();
            services.AddScoped<ICategoryApiService, CategoryApiService>();
            services.AddScoped<IProductApiService, ProductApiService>();
            services.AddScoped<IProductCategoryMappingsApiService, ProductCategoryMappingsApiService>();
            services.AddScoped<IProductManufacturerMappingsApiService, ProductManufacturerMappingsApiService>();
            services.AddScoped<IOrderApiService, OrderApiService>();
            services.AddScoped<IOrderNoteService, OrderNoteService>();
            services.AddScoped<IShoppingCartItemApiService, ShoppingCartItemApiService>();
            services.AddScoped<IOrderItemApiService, OrderItemApiService>();
            services.AddScoped<IProductAttributesApiService, ProductAttributesApiService>();
            services.AddScoped<IProductPictureService, ProductPictureService>();
            services.AddScoped<IProductAttributeConverter, ProductAttributeConverter>();
            services.AddScoped<ISpecificationAttributeApiService, SpecificationAttributesApiService>();
            services.AddScoped<INewsLetterSubscriptionApiService, NewsLetterSubscriptionApiService>();
            services.AddScoped<IManufacturerApiService, ManufacturerApiService>();
            services.AddScoped<IResanehlabService, ResanehlabService>();
            services.AddScoped<IOrderSchedulingService, OrderSchedulingService>();
            services.AddScoped<IMappingHelper, MappingHelper>();
            services.AddScoped<ICustomerRolesHelper, CustomerRolesHelper>();
            services.AddScoped<IJsonHelper, JsonHelper>();
            services.AddScoped<IDTOHelper, DTOHelper>();
            services.AddScoped<IJsonFieldsSerializer, JsonFieldsSerializer>();
            services.AddScoped<IFieldsValidator, FieldsValidator>();
            services.AddScoped<IObjectConverter, ArchObjectConverter>();
            services.AddScoped<ITypeConverter, TypeConverter>();
            services.AddScoped<IFactory<Category>, CategoryFactory>();
            services.AddScoped<IFactory<Product>, ProductFactory>();
            services.AddScoped<IFactory<Customer>, CustomerFactory>();
            services.AddScoped<IFactory<Address>, AddressFactory>();
            services.AddScoped<IFactory<Order>, OrderFactory>();
            services.AddScoped<IFactory<OrderNote>, OrderNoteFactory>();
            services.AddScoped<IFactory<ShoppingCartItem>, ShoppingCartItemFactory>();
            services.AddScoped<IFactory<Manufacturer>, ManufacturerFactory>();
            services.AddScoped<IFactory<ReturnRequest>, ReturnRequestFactory>();
            services.AddScoped<IJsonPropertyMapper, JsonPropertyMapper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Dictionary<string, object>>();

        }
    }
}
