import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Observable } from "rxjs";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { HttpErrorResponse } from "@angular/common/http";
import { httpErrorResponseHandler } from "apps/sd-hub/src/app/shared/http-error-handling/handlers";
import { ToastrService } from "ngx-toastr";
import { PerformType } from "apps/sd-hub/src/app/pages/generated/search-page/search-page.component";
import {
  IGridModel,
  SearchGridInFieldType,
  SearchGridOrderByFieldType,
  SearchGridOrderByType
} from "apps/sd-hub/src/app/models/autogen/grid.models";
import { GridApi } from "apps/sd-hub/src/app/shared/services/api/grid.api";
import { IPaginationResponse } from "apps/sd-hub/src/app/models/autogen/misc.models";

@UntilDestroy()
@Component({
  selector: 'search-in-grids',
  templateUrl: './search-in-grids.component.html',
  styleUrls: ['./search-in-grids.component.scss'],
})
export class SearchInGridsComponent implements OnInit {
  public readonly orderBy: { name: string, field: SearchGridOrderByFieldType, order: SearchGridOrderByType | null, icon: string | null }[] = [
    {
      name: 'Upload date',
      field: SearchGridOrderByFieldType.UploadDate,
      order: SearchGridOrderByType.Asc,
      icon: 'arrow_downward',
    },
    {
      name: 'User name',
      field: SearchGridOrderByFieldType.UserName,
      order: null,
      icon: null,
    },
  ];
  public readonly searchFieldsCheckboxes: { id: SearchGridInFieldType, value: string, checked: boolean }[] = [
    {id: SearchGridInFieldType.Name, value: 'Name', checked: true},
    {id: SearchGridInFieldType.Description, value: 'Description', checked: true},
    {id: SearchGridInFieldType.User, value: 'User name', checked: false},
  ];

  // region search text
  private _searchText = '';
  get searchText(): string {
    return this._searchText;
  }

  @Input() set searchText(value: string) {
    this._searchText = value;
  }

  // endregion

  // region search btn
  @Input() set searchButtonClick(value: Observable<unknown> | null) {
    value?.pipe(untilDestroyed(this)).subscribe(x => this.runSearch(PerformType.Search));
  }

  // endregion

  @Output() searchTextChange = new EventEmitter<string>();
  @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;
  public loading = false;
  public searchResult: IPaginationResponse<IGridModel> | null = null;
  public pageSize = 50;
  public page = 0;
  public totalPages = 1;

  constructor(private gridApi: GridApi,
              private toastr: ToastrService,) {
  }

  ngOnInit(): void {
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
    this.gridApi.search({
      skip,
      take,
      orderBy: this.orderBy.find(x => x.order != null)!.order!,
      orderByField: this.orderBy.find(x => x.order != null)!.field!,
      fields: this.searchFieldsCheckboxes.filter(x => x.checked).map(x => x.id),
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

  public onOrderBtnClick(field: SearchGridOrderByFieldType): void {
    for (const entry of this.orderBy) {
      if (entry.field === field) {
        entry.order = entry.order === SearchGridOrderByType.Asc
          ? SearchGridOrderByType.Desc
          : SearchGridOrderByType.Asc;
        if (entry.order === SearchGridOrderByType.Desc)
          entry.icon = 'arrow_upward';
        else if (entry.order === SearchGridOrderByType.Asc)
          entry.icon = 'arrow_downward';
      } else {
        entry.order = null;
        entry.icon = null;
      }
    }
  }
}
