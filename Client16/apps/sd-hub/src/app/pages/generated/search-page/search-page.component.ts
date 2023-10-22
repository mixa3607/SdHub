import {Component} from '@angular/core';
import {Observable, ReplaySubject, Subject} from "rxjs";

export enum SearchInType {
    Images,
    Albums,
    Grids
}

export enum PerformType {
    Search,
    Pagination
}

@Component({
    selector: 'search-page',
    templateUrl: './search-page.component.html',
    styleUrls: ['./search-page.component.scss']
})
export class SearchPageComponent {
    public readonly searchInVariants: { id: SearchInType, value: string }[] = [
        {id: SearchInType.Images, value: 'Images'},
        {id: SearchInType.Albums, value: 'Albums'},
        {id: SearchInType.Grids, value: 'Grids'},
    ];
    public readonly searchInType = SearchInType;
    public readonly performSearch$ = new ReplaySubject<unknown>(1);
    public selectedSearchType: SearchInType = SearchInType.Images;
    public searchText: string = '';

    //separatorKeysCodes: number[] = [ENTER, COMMA];
    //fruitCtrl = new FormControl('');
    //fruits: string[] = ['portrait'];
    //allFruits: string[] = ['Apple', 'Lemon', 'Lime', 'Orange', 'Strawberry'];

    constructor() {
        this.performSearch$.next(null);
    }

    public onSearchClick(): void {
        this.performSearch$.next(null);
    }
}
