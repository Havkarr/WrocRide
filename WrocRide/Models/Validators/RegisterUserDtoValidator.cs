﻿using FluentValidation;
using FluentValidation.AspNetCore;
using WrocRide.Entities;

namespace WrocRide.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(WrocRideDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(25);

            RuleFor(x => x.Surename)
                .NotEmpty()
                .MaximumLength(25);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty();
                
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.RoleId)
                .NotEmpty()
                .InclusiveBetween(1, 3);

            RuleFor(x => x.Password).MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(e => e.Email == value);

                    if(emailInUse)
                    {
                        context.AddFailure("Email","Email is already taken");
                    }
                });
        }
    }
}
