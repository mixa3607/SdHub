import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FileAdminApi } from "apps/SdHub/src/app/shared/services/api/file-admin.api";
import { PerformType } from "apps/SdHub/src/app/pages/generated/search-page/search-page.component";
import { HttpErrorResponse } from "@angular/common/http";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { IPaginationResponse } from "apps/SdHub/src/app/models/autogen/misc.models";
import { IFileModel } from "apps/SdHub/src/app/models/autogen/file.models";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: 'admin-files',
  templateUrl: './admin-files.component.html',
  styleUrls: ['./admin-files.component.scss'],
})
export class AdminFilesComponent implements OnInit {
  displayedColumns: string[] = ['id', 'name', 'hash', 'url'];
  @ViewChild('scrollTo', {read: ElementRef}) scrollTo?: ElementRef;
  public fileImportIsOpen = false;
  public importUrl: string = '';
  public importing: boolean = false;

  public loading = false;
  public searchText: string = '';
  public storage: string = '';
  public searchResult: IPaginationResponse<IFileModel> | null = null;

  public pageSize = 50;
  public page = 0;
  public totalPages = 1;

  constructor(private fileApi: FileAdminApi,
              private toastr: ToastrService,) {
    this.runSearch(PerformType.Search);
  }

  ngOnInit(): void {
  }

  public onImportClick(): void {
    this.importing = true;
    this.fileApi.import({fileUrl: this.importUrl}).subscribe({
      next: resp => {
        this.importing = false;
        this.toastr.success('Imported as ' + resp.directUrl);
        this.importUrl = '';
        this.fileImportIsOpen = false;
      },
      error: (err: HttpErrorResponse) => {
        this.importing = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    });
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
    this.fileApi.search({
      skip,
      take,
      searchText: this.searchText,
      storage: this.storage
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
}
