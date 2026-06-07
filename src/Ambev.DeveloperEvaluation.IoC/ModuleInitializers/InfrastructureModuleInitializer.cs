using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();

        var mongoConnectionString = builder.Configuration["MongoDB:ConnectionString"];
        var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"];

        builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
        builder.Services.AddScoped<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
        builder.Services.AddScoped<IMongoSaleRepository, MongoSaleRepository>();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["Redis:ConnectionString"];
            options.InstanceName = "ambev_";
        });

        builder.Services.AddScoped<ISaleCacheService, RedisSaleCacheService>();
    }
}