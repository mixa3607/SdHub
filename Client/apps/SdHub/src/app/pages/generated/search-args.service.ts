import {Injectable} from '@angular/core';
import {BehaviorSubject, first, Subject} from "rxjs";
import {
    ISearchImageRequest,
    SearchImageInFieldType,
    SearchImageOrderByFieldType,
    SearchImageOrderByType, SoftwareGeneratedTypes
} from "apps/SdHub/src/app/models/autogen/image.models";

export enum PerformType {
    Search,
    Pagination
}

@Injectable({
    providedIn: 'root'
})
export class SearchArgsService {
    private readonly defaultSearchInImagesArgs: ISearchImageRequest = {
        fields: [SearchImageInFieldType.Name, SearchImageInFieldType.Description, SearchImageInFieldType.Prompt],
        softwares: [SoftwareGeneratedTypes.AutomaticWebUi, SoftwareGeneratedTypes.DreamStudio, SoftwareGeneratedTypes.NovelAi],
        searchText: '',
        orderByField: SearchImageOrderByFieldType.UploadDate,
        orderBy: SearchImageOrderByType.Asc,
        alsoFromGrids: false,
        onlyFromRegisteredUsers: true,
        searchAsRegexp: false,
        skip: 0,
        take: 0,
        owner: ''
    }
    public readonly searchInImagesArgs$: BehaviorSubject<ISearchImageRequest>;
    public readonly searchPerform$: Subject<PerformType>;

    constructor() {
        this.searchInImagesArgs$ = new BehaviorSubject<ISearchImageRequest>(this.defaultSearchInImagesArgs);
        this.searchPerform$ = new Subject<PerformType>();
    }

    public updateImagesArgs(upd: Partial<ISearchImageRequest>): void {
        this.searchInImagesArgs$.pipe(first()).subscribe(x => {
            this.searchInImagesArgs$.next({
                ...x,
                ...upd
            });
        });
    }

    public resetImagesArgs(): void {
        this.searchInImagesArgs$.next(this.defaultSearchInImagesArgs);
    }
}
