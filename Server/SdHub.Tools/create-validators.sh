#!/bin/bash

IN_LINES="$1"
echo "$IN_LINES" | while read line; do
    read -r dir class <<<$(echo "$line" | sed -E 's|([^ ]+)Controller => ([^ ]+)|\1 \2|1')
    echo "DIR: $dir, CLASS: $class"
    mkdir -p "$dir" 2> /dev/null
    echo 'using FluentValidation;

namespace CostsAuthService.Models.Validators;

public class #class#Validator : AbstractValidator<#class#>
{
    public #class#Validator()
    {
        RuleFor(x => x.);
        RuleFor(x => x.);
        RuleFor(x => x.);
    }
}' | sed "s|#class#|$class|g" > "$dir/${class}Validator.cs"
done
