using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class ProductFilterDtoValidator : AbstractValidator<ProductFilterDto>
    {
        public ProductFilterDtoValidator()
        {
            this.ClassLevelCascadeMode = CascadeMode.Continue;

            RuleFor(p => p.CategoryId)
                .GreaterThan(0)
                .When(p => p.CategoryId.HasValue)
                .WithMessage("Kategori ID 0'dan büyük olmalıdır.");


            RuleFor(p => p.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(p => p.MinPrice.HasValue)
                .WithMessage("Minimum fiyat 0 veya daha büyük olmalı.");

            RuleFor(p => p.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(p => p.MaxPrice.HasValue)
                .WithMessage("Maksimum fiyat 0 veya daha büyük olmalı.");

            RuleFor(p => p)
                .Must(p => !p.MinPrice.HasValue || !p.MaxPrice.HasValue || p.MinPrice <= p.MaxPrice)
                .WithMessage("Minimum fiyat maksimum fiyattan büyük olamaz.");
        }
    }
}
