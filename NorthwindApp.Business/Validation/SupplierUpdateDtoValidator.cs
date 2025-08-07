using FluentValidation;
using NorthwindApp.Core.DTOs;

public class SupplierUpdateDtoValidator : AbstractValidator<SupplierUpdateDto>
{
    public SupplierUpdateDtoValidator()
    {
        RuleFor(x => x.SupplierId)
            .GreaterThan(0).WithMessage("Tedarikçi ID sıfırdan büyük olmalı.");

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
