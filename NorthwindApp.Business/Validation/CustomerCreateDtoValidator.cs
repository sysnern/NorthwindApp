using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class CustomerCreateDtoValidator : AbstractValidator<CustomerCreateDto>
    {
        public CustomerCreateDtoValidator()
        {
            RuleFor(c => c.CustomerId)
                .NotEmpty().WithMessage("Müşteri ID boş bırakılamaz.")
                .MaximumLength(5).WithMessage("Müşteri ID en fazla 5 karakter olabilir.");

            RuleFor(c => c.CompanyName)
                .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.")
                .MaximumLength(40).WithMessage("Şirket adı en fazla 40 karakter olabilir.");

            RuleFor(c => c.ContactName)
                .MaximumLength(30).WithMessage("İletişim kişisi adı en fazla 30 karakter olabilir.");

            RuleFor(c => c.ContactTitle)
                .MaximumLength(30).WithMessage("İletişim kişisi unvanı en fazla 30 karakter olabilir.");

            RuleFor(c => c.Address)
                .MaximumLength(60).WithMessage("Adres en fazla 60 karakter olabilir.");

            RuleFor(c => c.City)
                .MaximumLength(15).WithMessage("Şehir adı en fazla 15 karakter olabilir.");

            RuleFor(c => c.Region)
                .MaximumLength(15).WithMessage("Bölge adı en fazla 15 karakter olabilir.");

            RuleFor(c => c.PostalCode)
                .MaximumLength(10).WithMessage("Posta kodu en fazla 10 karakter olabilir.");

            RuleFor(c => c.Country)
                .MaximumLength(15).WithMessage("Ülke adı en fazla 15 karakter olabilir.");

            RuleFor(c => c.Phone)
                .MaximumLength(24).WithMessage("Telefon numarası en fazla 24 karakter olabilir.");

            RuleFor(c => c.Fax)
                .MaximumLength(24).WithMessage("Faks numarası en fazla 24 karakter olabilir.");
        }
    }
}
