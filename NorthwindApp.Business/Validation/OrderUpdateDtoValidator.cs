using FluentValidation;
using NorthwindApp.Core.DTOs;

public class OrderUpdateDtoValidator : AbstractValidator<OrderUpdateDto>
{
    public OrderUpdateDtoValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.CustomerId).NotEmpty().MaximumLength(5);
        RuleFor(x => x.EmployeeId).GreaterThan(0);
    }
}