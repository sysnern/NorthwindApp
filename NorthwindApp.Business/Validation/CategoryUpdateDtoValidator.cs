using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
    {
        public CategoryUpdateDtoValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Kategori ID 0'dan büyük olmalıdır.");

            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .MaximumLength(40).WithMessage("Kategori adı en fazla 40 karakter olabilir.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Açıklama en fazla 255 karakter olabilir.");
        }
    }
}
