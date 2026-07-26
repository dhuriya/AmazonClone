using AmazonClone.Application.Features.Products.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonClone.Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Price)
                .GreaterThan(0);
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId)
                .GreaterThan(0);
        }
    }
}
