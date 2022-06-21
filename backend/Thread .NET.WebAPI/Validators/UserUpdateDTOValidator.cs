using FluentValidation;
using Thread_.NET.Common.DTO.User;

namespace Thread_.NET.Validators
{
    public sealed class UserUpdateDTOValidator:AbstractValidator<UserDTO>
    {
        public UserUpdateDTOValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();
            RuleFor(x => x.UserName)
                .NotEmpty()
                    .WithMessage("Username is mandatory.")
                .MinimumLength(3)
                    .WithMessage("Username should be minimum 3 character.");
        }
    }
}
