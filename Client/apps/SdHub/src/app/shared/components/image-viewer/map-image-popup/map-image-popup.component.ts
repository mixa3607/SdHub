import { Component, OnInit } from '@angular/core';
import { ITileInfo } from "apps/SdHub/src/app/shared/components/image-viewer/image-viewer.component";

@Component({
  selector: 'map-image-popup',
  templateUrl: './map-image-popup.component.html',
  styleUrls: ['./map-image-popup.component.scss'],
})
export class MapImagePopupComponent implements OnInit {
  public tileInfo: ITileInfo | null = null;

  constructor() {
  }

  ngOnInit(): void {
  }

  goToLink(url: string) {
    window.open(url, "_blank");
  }
}
