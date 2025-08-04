using FluentValidation;
using NorthwindApp.Core.DTOs;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().MaximumLength(5);
        RuleFor(x => x.EmployeeId).GreaterThan(0);
    }
}


