using FluentValidation;
using NorthwindApp.Core.DTOs;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.CustomerID).NotEmpty().MaximumLength(5);
        RuleFor(x => x.EmployeeID).GreaterThan(0);
    }
}


