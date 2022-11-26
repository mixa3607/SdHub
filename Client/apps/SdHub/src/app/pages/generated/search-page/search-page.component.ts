import {Component} from '@angular/core';
import {PerformType, SearchArgsService} from "apps/SdHub/src/app/pages/generated/search-args.service";

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
    public searchText: string = '';

    //separatorKeysCodes: number[] = [ENTER, COMMA];
    //fruitCtrl = new FormControl('');
    //fruits: string[] = ['portrait'];
    //allFruits: string[] = ['Apple', 'Lemon', 'Lime', 'Orange', 'Strawberry'];

    constructor(private argsService: SearchArgsService) {
    }

    public onSearchClick(): void{
        this.argsService.searchPerform$.next(PerformType.Search);
    }
}
