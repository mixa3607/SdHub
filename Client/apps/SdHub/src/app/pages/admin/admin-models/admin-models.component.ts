import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { PerformType } from "apps/SdHub/src/app/pages/generated/search-page/search-page.component";
import { HttpErrorResponse } from "@angular/common/http";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { IPaginationResponse, SdVersion } from "apps/SdHub/src/app/models/autogen/misc.models";
import { ToastrService } from "ngx-toastr";
import { ModelApi } from "apps/SdHub/src/app/shared/services/api/model.api";
import { IModelModel, SearchModelInFieldType } from "apps/SdHub/src/app/models/autogen/model.models";

@Component({
  selector: 'admin-models',
  templateUrl: './admin-models.component.html',
  styleUrls: ['./admin-models.component.scss'],
})
export class AdminModelsComponent implements OnInit {
  displayedColumns: string[] = ['id', 'name', 'sdVersion', 'edit'];
  @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;

  public loading = false;
  public searchText: string = '';
  public searchResult: IPaginationResponse<IModelModel> | null = null;

  public pageSize = 50;
  public page = 0;
  public totalPages = 1;

  constructor(private modelsApi: ModelApi,
              private toastr: ToastrService,) {
    this.runSearch(PerformType.Search);
  }

  ngOnInit(): void {
  }


  public onSearchClick(): void {
    this.runSearch(PerformType.Search);
  }

  public onPageChange(): void {
    this.runSearch(PerformType.Pagination);
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
    this.modelsApi.search({
      skip,
      take,
      searchText: this.searchText,
      sdVersions: [SdVersion.V1, SdVersion.V2, SdVersion.Unknown],
      fields: [SearchModelInFieldType.V1Hash, SearchModelInFieldType.FullHash, SearchModelInFieldType.Name, SearchModelInFieldType.KnownNames]
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

  public sdVerToStr(ver: SdVersion): string{
    return SdVersion[ver];
  }

  public onEditClick(model: IModelModel): void{

  }
}
