import {Component} from '@angular/core';

export enum SearchInType{
    Images,
    Albums,
    Grids
}

@Component({
    selector: 'search-page',
    templateUrl: './search-page.component.html',
    styleUrls: ['./search-page.component.scss']
})
export class SearchPageComponent {
    public readonly searchInVariants: {id: SearchInType, value: string}[] = [
        {id: SearchInType.Images, value: 'Images'},
        {id: SearchInType.Albums, value: 'Albums'},
        {id: SearchInType.Grids, value: 'Grids'},
    ];
    public readonly searchInType = SearchInType;
    public selectedSearchType: SearchInType = SearchInType.Images;

    constructor() {
    }

}
