import {Component, Input, OnInit} from '@angular/core';
import {IImageModel} from "apps/SdHub/src/app/models/autogen/misc.models";

@Component({
    selector: 'small-image-card',
    templateUrl: './small-image-card.component.html',
    styleUrls: ['./small-image-card.component.scss'],
})
export class SmallImageCardComponent implements OnInit {
    get imageInfo(): IImageModel | null {
        return this._imageInfo;
    }

    @Input() set imageInfo(value: IImageModel | null) {
        this._imageInfo = value;
        this.loadImage(value);
    }

    private _imageInfo: IImageModel | null = null;

    public name: string = '';
    public uploadAt: string = '';
    public userName: string = '';
    public software: string = '';
    public thumbUrl: string = '';
    public dims: string = '';
    public shortToken: string = '';

    constructor() {
    }

    ngOnInit(): void {
    }

    private loadImage(value: IImageModel | null): void {
        this.name = value?.name ?? '';
        this.uploadAt = value?.createdAt ?? '';
        this.userName = value?.owner?.login ?? '';
        this.software = value?.parsedMetadata?.tags?.[0]?.software ?? 'unknown';
        this.thumbUrl = value?.thumbImage?.directUrl ?? '';
        this.dims = value?.parsedMetadata?.width != null && value?.parsedMetadata?.height != null
            ? `${value?.parsedMetadata?.width}x${value?.parsedMetadata?.height}`
            : '';
        this.shortToken = value?.shortToken ?? '';
    }
}
