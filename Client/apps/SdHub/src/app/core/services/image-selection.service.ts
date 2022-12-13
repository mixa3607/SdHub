import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ImageSelectionService {

  private _selectedImages$ = new BehaviorSubject<string[]>([]);

  public selectedImages$ = this._selectedImages$.asObservable();

  public hasSelectedImages$ = this._selectedImages$.pipe(
    map((selectedImages) => selectedImages.length > 0)
  );

  public toggleSelected(imageToken: string) {
    const previousValue = this._selectedImages$.value;
    const needDelete = previousValue.includes(imageToken);

    const updatedState = needDelete
      ? previousValue.filter((val) => val !== imageToken)
      : [...previousValue, imageToken];

    this._selectedImages$.next(updatedState);
  }

  public isSelected(imageToken: string) {
    return this._selectedImages$.pipe(
      map((selectedImages) => selectedImages.includes(imageToken))
    )
  }

  public getSelectedImages() {
    return this._selectedImages$.getValue();
  }

  public clearSelection() {
    this._selectedImages$.next([]);
  }
}
