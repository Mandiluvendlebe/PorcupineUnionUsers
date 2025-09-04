using Microsoft.EntityFrameworkCore;
using PU.Users.Api.Data;
using PU.Users.Api.Services;
using AutoMapper;
using PU.Users.Api.DTOs;
using PU.Users.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// DB
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=PUUsersDb;Trusted_Connection=True;";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PU.Users.Api.Repositories.IUserRepository, PU.Users.Api.Repositories.UserRepository>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<PermissionService>();

// Automapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Automapper profile
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<User, UserDto>().ReverseMap();
    cfg.CreateMap<Group, GroupDto>().ReverseMap();
    cfg.CreateMap<Permission, PermissionDto>().ReverseMap();
});
IMapper mapper = mapperConfig.CreateMapper();
app.Services.GetRequiredService<IMapper>();

// Migrate/Seed on startup (dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await SeedData.EnsureSeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles(); // serve index.html by default
app.UseStaticFiles();

app.MapControllers();

app.Run();
