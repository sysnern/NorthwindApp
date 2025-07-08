using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(x => x.CustomerID).NotEmpty().Length(5);
            RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(40);
        }
    }
}
