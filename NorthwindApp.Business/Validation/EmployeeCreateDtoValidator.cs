using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
    {
        public EmployeeCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı boş olamaz.")
                .MaximumLength(10).WithMessage("Ad en fazla 10 karakter olabilir.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı boş olamaz.")
                .MaximumLength(20).WithMessage("Soyad en fazla 20 karakter olabilir.");

            RuleFor(x => x.Title)
                .MaximumLength(30).WithMessage("Pozisyon en fazla 30 karakter olabilir.");

            RuleFor(x => x.City)
                .MaximumLength(15).WithMessage("Şehir en fazla 15 karakter olabilir.");

            RuleFor(x => x.Country)
                .MaximumLength(15).WithMessage("Ülke en fazla 15 karakter olabilir.");
        }
    }
}
