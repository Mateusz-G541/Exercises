using FluentValidation;
using RestaurantAPI.Entities;
using System.Linq;

namespace RestaurantAPI.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {

        public RegisterUserDtoValidator(RestaurantDBContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
               .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
               .Equal(p => p.Password);

            RuleFor(x => x.Email)
               .Custom((value, context) =>
               {
                   var emailInUse = dbContext.User.Any(ue => ue.Email == value);
                   if(emailInUse)
                   {
                       context.AddFailure("Email", "Email already used");
                   }
               });
        }
    }
}
