import {Component, OnInit} from '@angular/core';
import {UntilDestroy, untilDestroyed} from '@ngneat/until-destroy';


type UploadTabType = 'image' | 'grid';
@UntilDestroy()
@Component({
  selector: 'app-upload-page',
  templateUrl: './upload-page.component.html',
  styleUrls: ['./upload-page.component.scss']
})
export class UploadPageComponent implements OnInit {
  public readonly tabs: { enable: boolean, name: string, type: UploadTabType }[] = [
    {name: 'Images', enable: true, type: 'image'},
    {name: 'Grid', enable: true, type: 'grid'},
  ];
  public activeTab: UploadTabType = this.tabs.find(x => x.enable)!.type;

  ngOnInit(): void {
  }



  public onTabChange(tabName: UploadTabType): void {
    this.activeTab = tabName;
  }
}
