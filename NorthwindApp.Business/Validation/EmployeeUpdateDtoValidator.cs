using FluentValidation;
using NorthwindApp.Core.DTOs;

public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateDtoValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad alanı boş olamaz.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad alanı boş olamaz.");
    }
}