import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

export interface IPageSizeOption {
    size: number;
    name: string;
}

export interface IPaginationPosition {
    skip: number;
    take: number;
}

@Component({
    selector: 'paginator',
    templateUrl: './paginator.component.html',
    styleUrls: ['./paginator.component.scss'],
})
export class PaginatorComponent implements OnInit {
    public readonly defaultPageSize = 50;

    @Input() showPgeSizeChanger = false;

    @Input() pageSize = this.defaultPageSize;
    @Output() pageSizeChange = new EventEmitter<number>();

    @Input() totalPages: number = 1;

    @Input() page: number = 0;
    @Output() pageChange = new EventEmitter<number>();

    @Input() pageSizeVariants: IPageSizeOption[] = [
        {size: 20, name: '20'},
        {size: 50, name: '50'},
        {size: 100, name: '100'},
    ]

    constructor() {
    }

    ngOnInit(): void {
    }

    public getPageButtons(): { page: number, enable: boolean }[] {
        const buttons: { page: number, enable: boolean }[] = [];
        const maxShowPerDir = 5;
        let fromPage = this.page - maxShowPerDir;
        if (fromPage < 0)
            fromPage = 0;
        let toPage = fromPage + maxShowPerDir * 2;
        if (toPage >= this.totalPages)
            toPage = this.totalPages - 1;
        if (toPage - fromPage < maxShowPerDir * 2 + 1 && fromPage != 0)
            fromPage = toPage - maxShowPerDir * 2;

        for (let i = fromPage; i <= toPage; i++) {
            buttons.push({page: i, enable: this.page !== i});
        }

        return buttons;
    }

    public setPage(page: number): void {
        const prevPage = this.page;
        if (this.totalPages < page)
            this.page = this.totalPages - 1;
        else
            this.page = page;

        if (prevPage != this.page)
            this.pageChange.emit(this.page);
    }
}
