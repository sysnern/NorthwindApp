using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class SupplierCreateDtoValidator : AbstractValidator<SupplierCreateDto>
    {
        public SupplierCreateDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Şirket adı boş olamaz.")
                .MaximumLength(40).WithMessage("Şirket adı en fazla 40 karakter olabilir.");

            RuleFor(x => x.ContactName)
                .MaximumLength(30).WithMessage("İletişim kişisi adı en fazla 30 karakter olabilir.");

            RuleFor(x => x.City)
                .MaximumLength(15).WithMessage("Şehir en fazla 15 karakter olabilir.");

            RuleFor(x => x.Country)
                .MaximumLength(15).WithMessage("Ülke en fazla 15 karakter olabilir.");
        }
    }
}