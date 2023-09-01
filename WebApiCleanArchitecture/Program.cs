using Application.Handlers;
using Application.Interfaces;
using Application.Products.Commands;
using Application.Products.Queries;
using Domain.Models;
using FluentMigrator.Runner;
using Infrastructure.Caching;
using Infrastructure.Repositories;
using Infrastructure.Services;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Connectyon
builder.Services.AddTransient<IDbConnection>(c => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddTransient<IProductRepository, ProductRepository>();

// MediatR
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

// Command Handlers
builder.Services.AddTransient<IRequestHandler<CreateProductCommand, int>, ProductCommandHandlers>();
builder.Services.AddTransient<IRequestHandler<UpdateProductCommand, bool>, ProductCommandHandlers>();
builder.Services.AddTransient<IRequestHandler<DeleteProductCommand, bool>, ProductCommandHandlers>();

// Query Handlers
builder.Services.AddTransient<IRequestHandler<GetAllProductQuery, List<Product>>, ProductQueryHandlers>();
builder.Services.AddTransient<IRequestHandler<GetProductByNameQuery, Product>, ProductQueryHandlers>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddMediatR(typeof(RegisterUserCommandHandler).Assembly);

builder.Services.AddScoped<IAuthService, AuthService>();


// Cache
builder.Services.AddScoped<ICachingService, CachingService>();
builder.Services.AddStackExchangeRedisCache(o =>
{
    o.InstanceName = "instace";
    o.Configuration = "localhost:6379";

});

// Add FluentMigrator services
builder.Services.AddFluentMigratorCore().ConfigureRunner(rb => rb .AddSqlServer().WithGlobalConnectionString("DefaultConnection").ScanIn(typeof(Program).Assembly).For.Migrations()).AddLogging(lb => lb.AddFluentMigratorConsole());

// JwtSettings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = securityKey
        };
    });

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Your API", Version = "v1" });

    // Set up Bearer token authentication
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
app.UseAuthentication(); // Enable JWT Authentication
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{    
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    c.DisplayRequestDuration();
});

// Run migrations before mapping controllers
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.MapControllers();

app.Run();
