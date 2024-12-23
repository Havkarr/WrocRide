using Microsoft.AspNetCore.Identity;
using WrocRide.Entities;
using WrocRide.Exceptions;
using WrocRide.Models;

namespace WrocRide.Services
{
    public interface IUserService
    {
        UserDto GetUser();
        void UpdateUser(UpdateUserDto dto);
    }
    public class UserService : IUserService
    {
        private readonly WrocRideDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(WrocRideDbContext dbContext, IUserContextService userContextService, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _passwordHasher = passwordHasher;
        }

        public UserDto GetUser()
        {
            int? userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new NotFoundException("User not found"); 
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            var result =  new UserDto()
            {
                Name = user.Name,
                Surename = user.Surename,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                JoinAt = user.JoinAt
            };

            return result;
        }

        public void UpdateUser(UpdateUserDto dto)
        {
            int? userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new NotFoundException("User not found"); 
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (!string.IsNullOrEmpty(dto.Name))
            {
                user.Name = dto.Name;
            }

            if (!string.IsNullOrEmpty(dto.Surename))
            {
                user.Surename = dto.Surename;
            }

            if (!string.IsNullOrEmpty(dto.Email))
            {
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var hashedPassword = _passwordHasher.HashPassword(user, dto.Password);
                user.PasswordHash = hashedPassword;
            }

            _dbContext.SaveChanges();

            return;
        }
    }
}
