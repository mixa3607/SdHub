import {Component, OnInit} from '@angular/core';
import {UntilDestroy} from "@ngneat/until-destroy";
import {CdkDragDrop, moveItemInArray} from "@angular/cdk/drag-drop";
import {ENTER} from "@angular/cdk/keycodes";
import {MatChipInputEvent} from "@angular/material/chips";
import {AbstractControl, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {getErrorMessage} from '../../../shared/form-error-handling/handlers';

@UntilDestroy()
@Component({
  selector: 'upload-grid',
  templateUrl: './upload-grid.component.html',
  styleUrls: ['./upload-grid.component.scss'],
})
export class UploadGridComponent implements OnInit {
  readonly separatorKeysCodes = [ENTER] as const;

  public xForm: FormGroup;

  public yForm: FormGroup;

  constructor(private formBuilder: FormBuilder,) {
    this.xForm = formBuilder.group({
      xTiles: [1, Validators.compose([
        Validators.required,
        Validators.min(0),
      ])],
      xValues: ['', Validators.compose([
        Validators.required
      ])]
    });
    this.yForm = formBuilder.group({
      yTiles: [1, Validators.compose([
        Validators.required,
        Validators.min(0),
      ])],
      yValues: ['', Validators.compose([
        Validators.required
      ])]
    });
  }

  ngOnInit(): void {
  }

  public getErrorMessage(formControl: AbstractControl): string {
    return getErrorMessage(formControl);
  }


  drop(values: string[], event: CdkDragDrop<string[]>): void {
    moveItemInArray(values, event.previousIndex, event.currentIndex);
  }

  onRemoveClick(values: string[], idx: number): void {
    moveItemInArray(values, idx, values.length - 1);
    values.pop();
  }

  onAddValue(values: string[], event: MatChipInputEvent): void {
    const newValues = event.value.split(',').map(x => x.trim());
    values.push(...newValues);
    event.chipInput!.clear();
  }
}
