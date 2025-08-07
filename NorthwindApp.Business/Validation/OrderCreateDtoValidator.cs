using FluentValidation;
using NorthwindApp.Core.DTOs;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Müşteri ID boş olamaz.")
            .MaximumLength(5).WithMessage("Müşteri ID en fazla 5 karakter olabilir.");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Çalışan ID sıfırdan büyük olmalı.");

        RuleFor(x => x.OrderDate)
            .Must(date => !date.HasValue || date.Value <= DateTime.Now)
            .WithMessage("Sipariş tarihi gelecek bir tarih olamaz.");
    }
}


