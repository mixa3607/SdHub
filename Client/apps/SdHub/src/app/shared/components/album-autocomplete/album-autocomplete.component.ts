import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from "@angular/forms";
import { AlbumApi } from "apps/SdHub/src/app/shared/services/api/album.api";
import { debounceTime, filter, startWith } from "rxjs";
import { UntilDestroy, untilDestroyed } from "@ngneat/until-destroy";
import { HttpErrorResponse } from "@angular/common/http";
import { httpErrorResponseHandler } from "apps/SdHub/src/app/shared/http-error-handling/handlers";
import { ToastrService } from "ngx-toastr";
import { SearchAlbumInFieldType } from "apps/SdHub/src/app/models/autogen/album.models";


interface IAllowedAlbum {
  name: string,
  shortToken: string
}

@UntilDestroy()
@Component({
  selector: 'album-autocomplete',
  templateUrl: './album-autocomplete.component.html',
  styleUrls: ['./album-autocomplete.component.scss'],
})
export class AlbumAutocompleteComponent implements OnInit {
  private _userLogin: string | null = null;
  get userLogin(): string | null {
    return this._userLogin;
  }

  @Input() set userLogin(value: string | null) {
    this._userLogin = value;
  }


  public albumForm: FormControl<string | IAllowedAlbum>;
  public allowedAlbums: IAllowedAlbum[] = [];
  public albumsLoading = false;
  public uploadToAlbum: string | null = null;
  public query = '';

  @Output() uploadToAlbumChange = new EventEmitter<string | null>();

  constructor(private albumApi: AlbumApi,
              private toastr: ToastrService,) {
    this.albumForm = new FormControl<string | IAllowedAlbum>('', {nonNullable: true});
    this.albumForm.valueChanges
      .pipe(startWith(''), untilDestroyed(this), filter(x => typeof x === 'string'), debounceTime(500))
      .subscribe(x => {
        this.query = x as string;
        this.reloadAlbums();
      });
    this.albumForm.valueChanges
      .pipe(untilDestroyed(this))
      .subscribe(x => {
        this.uploadToAlbum = typeof x === 'string' ? null : x.shortToken;
        this.uploadToAlbumChange.emit(this.uploadToAlbum);
      });
  }

  ngOnInit(): void {
  }


  public displayAlbumFn(alb: IAllowedAlbum) {
    return alb?.name ?? alb;
  }

  public reloadAlbums(): void {
    this.albumApi.search({
      owner: this._userLogin!,
      searchText: this.query,
      fields: [SearchAlbumInFieldType.Description, SearchAlbumInFieldType.Name],
      take: 50
    }).subscribe({
      next: resp => {
        this.albumsLoading = false;
        this.allowedAlbums = resp.items.map(x => ({name: x.name, shortToken: x.shortToken}));
      },
      error: (err: HttpErrorResponse) => {
        this.albumsLoading = false;
        httpErrorResponseHandler(err, this.toastr);
      }
    })
  }
}
