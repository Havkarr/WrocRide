using FluentValidation;
using WrocRide.Entities;

namespace WrocRide.Models.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator(WrocRideDbContext dbContext)
        {
            RuleFor(x => x.Name)
<<<<<<< HEAD
                .NotEmpty()
                .MaximumLength(25)
                .When(x => x.Name != null);

            RuleFor(x => x.Surename)
                .NotEmpty()
                .MaximumLength(25)
                .When(x => x.Surename != null);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .When(x => x.PhoneNumber != null);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .When(x => x.Email != null);
=======
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
>>>>>>> a93b2aa8f314e8c61b44c5323103c01c713f105e

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(e => e.Email == value);

                    if (emailInUse)
                    {
                        context.AddFailure("Email", "Email is already taken");
                    }
                });

<<<<<<< HEAD
            RuleFor(x => x.Password)
                .MinimumLength(8)
                .When(x => x.Password != null);
            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password);
=======
            RuleFor(x => x.Password).MinimumLength(8);
            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);
>>>>>>> a93b2aa8f314e8c61b44c5323103c01c713f105e
        }
    }
}
