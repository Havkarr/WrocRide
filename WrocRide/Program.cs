using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WrocRide.Entities;
using WrocRide.Helpers;
using WrocRide.Middleware;
using WrocRide.Models;
using WrocRide.Models.Validators;
using WrocRide.Seeders;
using WrocRide.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthentication>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddDbContext<WrocRideDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDriverDto>, RegisterDriverDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateCarDto>, UpdateCarDtoValidator>();
builder.Services.AddScoped<IValidator<DriverQuery>, DriverQueryValidator>();
builder.Services.AddScoped<IValidator<UpdateDriverPricingDto>, UpdateDriverPricingDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateDriverStatusDto>, UpdateDriverStatusDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateRideStatusDto>, UpdateRideStatusDtoValidator>();
builder.Services.AddScoped<IValidator<RideDriverDecisionDto>, RideDriverDecisionDtoValidator>();
builder.Services.AddScoped<IValidator<RideQuery>, RideQueryValidator>();
builder.Services.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();
builder.Services.AddScoped<IValidator<CreateRatingDto>, CreateRatingDtoValidator>();
builder.Services.AddScoped<IValidator<DriverRatingsQuery>, DriverRatingsQueryValidator>();
builder.Services.AddScoped<IValidator<DocumentQuery>, DocumentQueryValidator>();

builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IRideService, RideService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(jwtOptions);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
    };

});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WrocRide API")
);

app.UseAuthorization();

app.MapControllers();

app.Seed();

app.Run();
