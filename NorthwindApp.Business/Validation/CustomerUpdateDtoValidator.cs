using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class CustomerUpdateDtoValidator : AbstractValidator<CustomerUpdateDto>
    {
        public CustomerUpdateDtoValidator()
        {
            RuleFor(x => x.CustomerID).NotEmpty().Length(5);
            RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(40);
        }
    }
}
