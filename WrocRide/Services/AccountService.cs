using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WrocRide.Entities;
using WrocRide.Exceptions;
using WrocRide.Helpers;
using WrocRide.Models;
using WrocRide.Models.Enums;

namespace WrocRide.Services
{
    public interface IAccountService
    {
        void Register(RegisterUserDto dto);
        void RegisterDriver(RegisterDriverDto dto);
        string Login(LoginUserDto dto);
    }

    public class AccountService : IAccountService
    {
        private readonly WrocRideDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtAuthentication _jwtAuthentication;

        public AccountService(WrocRideDbContext dbContext, IPasswordHasher<User> passwordHasher, JwtAuthentication jwtAuthentication)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _jwtAuthentication = jwtAuthentication;
        }

        public void Register(RegisterUserDto dto)
        {
            using var dbContextTransaction = _dbContext.Database.BeginTransaction();
            try
            {
                var newUser = new User()
                {
                    Name = dto.Name,
                    Surename = dto.Surename,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    RoleId = dto.RoleId,
                    JoinAt = DateTime.Now
                };

                var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.PasswordHash = hashedPassword;

                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();

                var role = _dbContext.Roles.FirstOrDefault(r => r.Id == dto.RoleId);
                if(role == null)
                {
                    throw new NotFoundException("Role not found");
                }
                
                if(role.Name == "Client")
                {
                    var client = new Client()
                    {
                        UserId = newUser.Id
                    };
                    _dbContext.Clients.Add(client);
                }
                else if(role.Name == "Admin")
                {
                    var admin = new Admin()
                    {
                        UserId = newUser.Id
                    };
                    _dbContext.Admins.Add(admin);
                }
                else
                {
                    throw new BadRequestException("Invalid role assigned");
                }

                _dbContext.SaveChanges();
                dbContextTransaction.Commit();
            }
            catch(Exception)
            {
                dbContextTransaction.Rollback();
                throw new Exception();
            }
        }

        public void RegisterDriver(RegisterDriverDto dto)
        {
            using var dbContextTransaction = _dbContext.Database.BeginTransaction();
            try
            {
                var newUser = new User()
                {
                    Name = dto.Name,
                    Surename = dto.Surename,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    RoleId = dto.RoleId,
                    JoinAt = DateTime.Now
                };

                var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.PasswordHash = hashedPassword;

                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();

                var document = new Document()
                {
                    FileLocation = dto.FileLocation,
                    RequestDate = DateTime.Now,
                    DocumentStatus = DocumentStatus.UnderVerification
                };

                _dbContext.Documents.Add(document);
                _dbContext.SaveChanges();

                var car = new Car()
                {
                    LicensePlate = dto.LicensePlate,
                    Brand = dto.Brand,
                    Model = dto.Model,
                    BodyColor = dto.BodyColor,
                };

                _dbContext.Cars.Add(car);
                _dbContext.SaveChanges();

                var driver = new Driver()
                {
                    UserId = newUser.Id,
                    Pricing = dto.Pricing,
                    DriverStatus = DriverStatus.UnderVerification,
                    DocumentId = document.Id,
                    CarId = car.Id
                };

                _dbContext.Drivers.Add(driver);
                _dbContext.SaveChanges();
                dbContextTransaction.Commit();
            }

            catch(Exception)
            {
                dbContextTransaction.Rollback();
                throw new Exception();
            }
        }

        public string Login(LoginUserDto dto)
        {
            var user = _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == dto.Email);

            if(user == null)
            {
                throw new BadRequestException("Invalid email or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if(result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid email or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Name} {user.Surename}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthentication.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtAuthentication.Expires);

            var tokenOptions = new JwtSecurityToken(issuer: _jwtAuthentication.Issuer,
                audience: _jwtAuthentication.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return token;
        }
    }
}
