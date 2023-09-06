using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
//using MinimalChatApplication.Middlewares;
using MinimalChatApplication.Repositories;
using MinimalChatApplication.Services;
using System.Text;
using MinimalChatApplication.Hubs;
using MinimalChatApplication.Models;
using MinimalChatApplication.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
builder =>
{
    builder.AllowAnyMethod().AllowAnyHeader()
    .WithOrigins("http://localhost:4200")
    .AllowCredentials();
}));



builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<ILogRepository, LogRepository>();



builder.Services.AddScoped<Connection>();
builder.Services.AddScoped<RequestLoggingMiddleware>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();





//configuring db path
builder.Services.AddDbContext<MinimalChatContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MinimalChatContext")));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<MinimalChatContext>()
            .AddDefaultTokenProviders();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add JWT authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {

//             options.RequireHttpsMetadata = false;
//             options.SaveToken = true;
//             options.TokenValidationParameters = new TokenValidationParameters()
//             {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidAudience = builder.Configuration["Jwt:Audience"],
//                ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//             };
//    });
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});








// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<MinimalChatContext>();
    context.Database.EnsureCreated();
    
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapHub<ChatHub>("/chatHub");
app.MapControllers();
app.UseMiddleware<RequestLoggingMiddleware>();
app.Run();
