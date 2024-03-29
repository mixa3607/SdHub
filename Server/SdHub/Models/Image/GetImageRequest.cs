﻿using FluentValidation;

namespace SdHub.Models.Image;

public class GetImageRequest
{
    public string? ShortToken { get; set; }
    public class Validator : AbstractValidator<GetImageRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ShortToken).NotEmpty();
        }
    }
}