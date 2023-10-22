import { Pipe, PipeTransform } from '@angular/core';
import { DateTime } from 'luxon';

@Pipe({
  name: 'localDate',
})
export class LocalDatePipe implements PipeTransform {
  transform(value: Date, args: string): string {
    if (!value || !args) {
      return '';
    }
    return DateTime.fromJSDate(value).toLocal().toFormat(args);
  }
}
