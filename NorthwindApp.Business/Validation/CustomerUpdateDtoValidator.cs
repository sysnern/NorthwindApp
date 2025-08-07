using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class CustomerUpdateDtoValidator : AbstractValidator<CustomerUpdateDto>
    {
        public CustomerUpdateDtoValidator()
        {
            RuleFor(c => c.CustomerId)
                .NotEmpty().WithMessage("Müşteri ID boş bırakılamaz.")
                .MaximumLength(5).WithMessage("Müşteri ID en fazla 5 karakter olabilir.");

            RuleFor(c => c.CompanyName)
                .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.")
                .MaximumLength(40).WithMessage("Şirket adı en fazla 40 karakter olabilir.");

            RuleFor(c => c.ContactName)
                .MaximumLength(30).WithMessage("İletişim kişisi adı en fazla 30 karakter olabilir.");

            RuleFor(c => c.City)
                .MaximumLength(15).WithMessage("Şehir adı en fazla 15 karakter olabilir.");

            RuleFor(c => c.Country)
                .MaximumLength(15).WithMessage("Ülke adı en fazla 15 karakter olabilir.");
        }
    }
}
