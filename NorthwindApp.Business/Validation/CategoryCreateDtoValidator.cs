using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .MaximumLength(40).WithMessage("Kategori adı en fazla 40 karakter olabilir.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("Açıklama en fazla 255 karakter olabilir.");
        }
    }
}
