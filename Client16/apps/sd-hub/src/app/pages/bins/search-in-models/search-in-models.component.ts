import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { IPaginationResponse, SdVersion } from "apps/sd-hub/src/app/models/autogen/misc.models";
import { IModelModel, SearchModelInFieldType } from "apps/sd-hub/src/app/models/autogen/model.models";
import { Observable } from "rxjs";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { PerformType } from "apps/sd-hub/src/app/pages/generated/search-page/search-page.component";
import { ToastrService } from "ngx-toastr";
import { HttpErrorResponse } from "@angular/common/http";
import { httpErrorResponseHandler } from "apps/sd-hub/src/app/shared/http-error-handling/handlers";
import { ModelApi } from "apps/sd-hub/src/app/shared/services/api/model.api";
import { AuthStateService } from "apps/sd-hub/src/app/core/services/auth-state.service";

@UntilDestroy()
@Component({
  selector: 'search-in-models',
  templateUrl: './search-in-models.component.html',
  styleUrls: ['./search-in-models.component.scss'],
})
export class SearchInModelsComponent implements OnInit {
  public readonly sdVersionCheckboxes: { id: SdVersion, value: string, checked: boolean }[] = [
    {id: SdVersion.V1, value: 'V1', checked: true},
    {id: SdVersion.V2, value: 'V2', checked: true},
    {id: SdVersion.Unknown, value: 'Unknown', checked: false},
  ];
  public readonly searchFieldsCheckboxes: { id: SearchModelInFieldType, value: string, checked: boolean }[] = [
    {id: SearchModelInFieldType.Name, value: 'Name', checked: true},
    {id: SearchModelInFieldType.KnownNames, value: 'Known names', checked: true},
    {id: SearchModelInFieldType.V1Hash, value: 'Model hash', checked: true},
    {id: SearchModelInFieldType.FullHash, value: 'Full sha256', checked: false},
  ];

  @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;
  @Input() searchText: string = '';

  @Input() set searchButtonClick(value: Observable<unknown> | null) {
    value?.pipe(untilDestroyed(this)).subscribe(x => this.runSearch(PerformType.Search));
  }

  public loading = false;
  public searchResult: IPaginationResponse<IModelModel> | null = null;
  public pageSize = 50;
  public page = 0;
  public totalPages = 1;
  public isAdmin = false;
  public addModelIsOpen = false;
  public addModelName = '';

  constructor(
    private modelApi: ModelApi,
    private toastr: ToastrService,
    private authState: AuthStateService,
  ) {
    authState.isAdmin$.pipe(untilDestroyed(this)).subscribe(x => this.isAdmin = x);
  }

  ngOnInit(): void {
  }

  public onAddModelClick(): void{
    this.modelApi.create({name: this.addModelName}).subscribe({
      next: resp => {
        this.addModelName = '';
        this.addModelIsOpen = false;
      },
      error: (err: HttpErrorResponse) => {
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  private runSearch(type: PerformType): void {
    this.loading = true;
    let take = this.pageSize;
    let skip = 0;
    if (type === PerformType.Search) {
      skip = 0;
    } else if (type === PerformType.Pagination) {
      skip = this.page * this.pageSize;
    }
    this.modelApi.search({
      skip,
      take,
      fields: this.searchFieldsCheckboxes.filter(x => x.checked).map(x => x.id),
      sdVersions: this.sdVersionCheckboxes.filter(x => x.checked).map(x => x.id),
      searchText: this.searchText,
    }).subscribe({
      next: resp => {
        this.loading = false;
        this.searchResult = resp;
        this.totalPages = Math.floor(resp.total / this.pageSize) + ((resp.total % this.pageSize) === 0 ? 0 : 1);
        if (type === PerformType.Search) {
          this.page = 0;
        }
        if (type === PerformType.Pagination) {
          this.scrollTo?.nativeElement?.scrollIntoView({behavior: "smooth"});
        }
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        this.searchResult = null;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
  }

  public onPageChange(): void {
    this.runSearch(PerformType.Pagination);
  }

  public sdVerToStr(ver: SdVersion): string{
    return SdVersion[ver];
  }
}
