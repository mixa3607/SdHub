import {AbstractControl} from "@angular/forms";

export function getErrorMessage(formControl: AbstractControl): string {
  if (!formControl.invalid)
    return '';
  if (formControl.hasError('required'))
    return 'You must enter a value';
  if (formControl.hasError('email'))
    return 'Not a valid email';
  if (formControl.hasError('minlength'))
    return 'Min len';
  if (formControl.hasError('maxlength'))
    return 'Max len';
  if (formControl.hasError('min'))
    return 'Min';
  if (formControl.hasError('max'))
    return 'Max';
  if (formControl.hasError('not_matching'))
    return 'Passwords not matched';
  if (formControl.hasError('bypassme'))
    return formControl.errors?.['bypassme'] as string;

  return 'Unknown error';
}
