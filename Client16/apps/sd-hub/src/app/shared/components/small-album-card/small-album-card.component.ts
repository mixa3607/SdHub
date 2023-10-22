import {Component, Input, OnInit} from '@angular/core';
import {IAlbumModel} from "apps/sd-hub/src/app/models/autogen/album.models";

@Component({
    selector: 'small-album-card',
    templateUrl: './small-album-card.component.html',
    styleUrls: ['./small-album-card.component.scss'],
})
export class SmallAlbumCardComponent implements OnInit {
    @Input() set albumImagesCount(value: number | null) {
        this.imagesCount = value || -1;
    }

    @Input() set albumInfo(value: IAlbumModel | null) {
        this._albumInfo = value;
        this.name = value?.name ?? '';
        this.uploadAt = value?.createdAt ?? '';
        this.userName = value?.owner?.login ?? '';
        this.thumbUrl = value?.thumbImage?.directUrl ?? '';
        this.shortToken = value?.shortToken ?? '';
    }

    private _albumInfo: IAlbumModel | null = null;

    public name = '';
    public uploadAt = '';
    public userName = '';
    public thumbUrl = '';
    public shortToken = '';
    public imagesCount = -1;

    constructor() {
    }

    ngOnInit(): void {
    }
}
