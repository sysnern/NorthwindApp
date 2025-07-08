using FluentValidation;
using NorthwindApp.Core.DTOs;

public class OrderUpdateDtoValidator : AbstractValidator<OrderUpdateDto>
{
    public OrderUpdateDtoValidator()
    {
        RuleFor(x => x.OrderID).GreaterThan(0);
        RuleFor(x => x.CustomerID).NotEmpty().MaximumLength(5);
        RuleFor(x => x.EmployeeID).GreaterThan(0);
    }
}