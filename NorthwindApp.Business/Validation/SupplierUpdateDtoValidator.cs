using FluentValidation;
using NorthwindApp.Core.DTOs;

public class SupplierUpdateDtoValidator : AbstractValidator<SupplierUpdateDto>
{
    public SupplierUpdateDtoValidator()
    {
        RuleFor(x => x.SupplierId).GreaterThan(0);
        RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Şirket adı boş olamaz.");
    }
}
