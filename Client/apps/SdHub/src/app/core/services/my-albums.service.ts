import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map, shareReplay, switchMap } from 'rxjs/operators';
import { AlbumApi } from '../../shared/services/api/album.api';
import { AuthStateService } from './auth-state.service';

@Injectable({ providedIn: 'root' })
export class MyAlbumsService {

  private reloadAlbums$ = new BehaviorSubject(null);

  private currentUsername$ = this.authStateService.user$.pipe(
    map((user) => user?.login),
    shareReplay(1)
  );

  private myAlbumsRes$ = this.reloadAlbums$.pipe(
    switchMap(() => this.currentUsername$),
    switchMap((username) => username
      ? this.albumsApi.search({ owner: username })
      : []
    ),
    shareReplay(1)
  );

  public myAlbums$ = this.myAlbumsRes$.pipe(
    map(({ albums }) => albums),
    shareReplay(1)
  )

  public myAlbumsTotal$ = this.myAlbumsRes$.pipe(
    map(({ total }) => total),
    shareReplay(1)
  );

  public hasAlbums$ = this.myAlbumsTotal$.pipe(
    map((total) => total > 0),
    shareReplay(1)
  );

  constructor(private authStateService: AuthStateService, private albumsApi: AlbumApi) { }

  public reloadAlbums() {
    this.reloadAlbums$.next(null);
  }
}
