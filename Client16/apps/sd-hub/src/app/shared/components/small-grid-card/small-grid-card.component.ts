import {Component, Input, OnInit} from '@angular/core';
import { IGridModel } from "apps/sd-hub/src/app/models/autogen/grid.models";

@Component({
    selector: 'small-grid-card',
    templateUrl: './small-grid-card.component.html',
    styleUrls: ['./small-grid-card.component.scss'],
})
export class SmallGridCardComponent implements OnInit {
    @Input() set info(value: IGridModel | null) {
        this._info = value;
        this.name = value?.name ?? '';
        this.uploadAt = value?.createdAt ?? '';
        this.userName = value?.owner?.login ?? '';
        this.thumbUrl = value?.thumbImage?.directUrl ?? '';
        this.shortToken = value?.shortToken ?? '';
    }

    private _info: IGridModel | null = null;

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
