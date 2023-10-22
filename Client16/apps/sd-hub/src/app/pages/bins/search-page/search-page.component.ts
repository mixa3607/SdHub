import { Component, OnInit } from '@angular/core';
import { ReplaySubject } from "rxjs";

export enum SearchInType {
  Models,
  Vaes,
  Embeddings,
  Hypernets
}

@Component({
  selector: 'search-page',
  templateUrl: './search-page.component.html',
  styleUrls: ['./search-page.component.scss'],
})
export class SearchPageComponent {
  public readonly searchInVariants: { id: SearchInType, value: string }[] = [
    {id: SearchInType.Models, value: 'Models'},
    {id: SearchInType.Vaes, value: 'Vae'},
    {id: SearchInType.Embeddings, value: 'Embedding'},
    {id: SearchInType.Hypernets, value: 'Hypernetworks'},
  ];
  public readonly searchInType = SearchInType;
  public readonly performSearch$ = new ReplaySubject<unknown>(1);
  public selectedSearchType: SearchInType = SearchInType.Models;
  public searchText: string = '';

  constructor() {
    this.performSearch$.next(null);
  }

  public onSearchClick(): void {
    this.performSearch$.next(null);
  }
}
