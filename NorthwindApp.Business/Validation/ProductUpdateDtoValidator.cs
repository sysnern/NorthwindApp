using FluentValidation;
using NorthwindApp.Core.DTOs;

namespace NorthwindApp.Business.Validation
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            RuleFor(p => p.ProductId)
                .GreaterThan(0).WithMessage("Ürün ID sıfırdan büyük olmalı.");

            RuleFor(p => p.ProductName)
                .NotEmpty().WithMessage("Ürün adı boş bırakılamaz.")
                .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalı.");

            RuleFor(p => p.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Birim fiyat 0 veya daha büyük olmalı.");

            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("Kategori ID sıfırdan büyük olmalı.");

            RuleFor(p => p.SupplierId)
                .GreaterThan(0).WithMessage("Tedarikçi ID sıfırdan büyük olmalı.");
        }
    }
}
