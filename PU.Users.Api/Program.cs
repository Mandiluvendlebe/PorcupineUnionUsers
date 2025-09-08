using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Services;
using AutoMapper;
using PU.Users.Api.DTOs;
using PU.Users.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Detect environment
var env = builder.Environment;

// Configure database provider based on environment
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (env.IsEnvironment("Test"))
    {
        // CI / Automated Tests → InMemory DB
        options.UseInMemoryDatabase("PUUsersTestDb");
    }
    else
    {
        // Local + Production → SQL Server
        var conn = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=PUUsersDb;Trusted_Connection=True;";
        options.UseSqlServer(conn);
    }
});

// Register application services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PU.Users.Api.Repositories.IUserRepository, PU.Users.Api.Repositories.UserRepository>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<PermissionService>();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Setup AutoMapper mapping profile
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<User, UserDto>().ReverseMap();
    cfg.CreateMap<Group, GroupDto>().ReverseMap();
    cfg.CreateMap<Permission, PermissionDto>().ReverseMap();
});
IMapper mapper = mapperConfig.CreateMapper();
app.Services.GetRequiredService<IMapper>();

// Apply migrations & seed ONLY for local/prod, NOT CI
if (!env.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await SeedData.EnsureSeedAsync(db);
}

// ✅ Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();

// Required for WebApplicationFactory<T>
public partial class Program { }
