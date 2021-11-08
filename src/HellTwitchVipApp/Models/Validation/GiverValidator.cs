using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using HellTwitchVipApp.Models.Dto;

namespace HellTwitchVipApp.Models.Validation
{
    public class GiverValidator : AbstractValidator<GiverDto>
    {
        public GiverValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Count).NotEmpty().WithMessage("Please specify a first name");
        }
    }
}
