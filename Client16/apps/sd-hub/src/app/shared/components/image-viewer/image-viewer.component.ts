import {
  ApplicationRef,
  Component,
  ComponentFactoryResolver,
  EventEmitter,
  Injectable,
  Injector,
  Input,
  OnInit,
  Output, ViewContainerRef
} from '@angular/core';
import {
  Coords,
  CRS,
  geoJSON,
  GridLayer,
  latLngBounds,
  Layer,
  Map,
  MapOptions,
  point, TileLayer,
  tileLayer,
  TileLayerOptions
} from "leaflet";
import { FeatureCollection } from "geojson";
import {
  MapImagePopupComponent
} from "apps/sd-hub/src/app/shared/components/image-viewer/map-image-popup/map-image-popup.component";

export interface IGridLegendTitle {
  name: string;
  background: string;
}

export interface IGridOptions {
  xTiles: number;
  yTiles: number;
  tileWidth: number;
  tileHeight: number;

  maxNativeZoom: number;
  minNativeZoom: number;
  minZoom?: number | null;
  maxZoom?: number | null;
  startZoom?: number | null;
  tilesUrlTemplate: string;

  showLegend: boolean;
  xLegend: IGridLegendTitle[];
  yLegend: IGridLegendTitle[];

  tiles?: ITileInfo[] | null;
}

export interface ITileInfo {
  name: string | null;
  description: string | null;
  shortUrl: string;
  shortCode: string;
  props: {name: string; value: string}[]|null;
}

@Component({
  selector: 'image-viewer',
  templateUrl: './image-viewer.component.html',
  styleUrls: ['./image-viewer.component.scss'],
})
export class ImageViewerComponent implements OnInit {
  @Input() showCloseButton = false;
  @Output() closeClick = new EventEmitter<boolean>();

  private map?: Map;

  public mapOptions: MapOptions = {
    layers: [],
    attributionControl: false,
    crs: CRS.Simple
  };
  public mapLayers: Layer[] = [];

  public gridOptions: IGridOptions | null = null;
  public xElemWidth = 0;
  public xElemMarginLeft = 0;
  public yElemHeight = 0;
  public yElemMarginTop = 0;


  constructor(private popUpService: PopUpService,
              private viewContainerRef: ViewContainerRef) {
  }

  public initOptions(opts: IGridOptions): void {
    this.gridOptions = opts;
    this.gridOptions.maxZoom ??= this.gridOptions.maxNativeZoom + 1;
    this.gridOptions.minZoom ??= this.gridOptions.minNativeZoom;
    this.gridOptions.startZoom ??= this.gridOptions.minZoom;
    this.gridOptions.tiles ??= [];
  }

  ngOnInit(): void {
  }

  private buildGeoJsonLayer(map: Map): Layer {
    if (this.gridOptions == null)
      throw new Error('opts is null');
    const geoJson: FeatureCollection = {
      "type": "FeatureCollection",
      "features": []
    };
    for (let i = 0; i < this.gridOptions.tiles!.length; i++) {
      const tile = this.gridOptions.tiles![i];
      const xTile = i % this.gridOptions.xTiles;
      const yTile = Math.floor(i / this.gridOptions.xTiles);

      const xLT = xTile * this.gridOptions.tileWidth;
      const yLT = yTile * this.gridOptions.tileHeight;
      const xRB = xLT + this.gridOptions.tileWidth;
      const yRB = yLT + this.gridOptions.tileHeight;

      const points = [
        map.unproject([xLT, yLT], this.gridOptions.maxNativeZoom),
        map.unproject([xRB, yLT], this.gridOptions.maxNativeZoom),
        map.unproject([xRB, yRB], this.gridOptions.maxNativeZoom),
        map.unproject([xLT, yRB], this.gridOptions.maxNativeZoom),
      ];
      geoJson.features.push({
        type: 'Feature',
        id: i,
        properties: {
          "name": tile.name,
          "popupContent": tile.description
        },
        geometry: {
          type: 'Polygon',
          coordinates: [
            points.map(x => [x.lng, x.lat])
          ],
        }
      })
    }
    const myStyle = {
      "opacity": .5,
      "fillOpacity": 0
    };
    const geoLayer = geoJSON(geoJson, {
        onEachFeature: (feature, layer) => {
          layer.bindPopup(this.popUpService.returnPopUpHTML(this.gridOptions!.tiles![feature.id as number], this.viewContainerRef),
            {
              minWidth: 450
            });
        },
        style: myStyle,
      }
    );
    return geoLayer;
  }

  private buildImagesLayer(map: Map): Layer {
    if (this.gridOptions == null)
      throw new Error('opts is null');
    const totalWidth = this.gridOptions.tileWidth * this.gridOptions.xTiles;
    const totalLength = this.gridOptions.tileHeight * this.gridOptions.yTiles;
    const layerBounds = latLngBounds(
      map.unproject([0, 0], this.gridOptions.maxNativeZoom),
      map.unproject([totalWidth, totalLength], this.gridOptions.maxNativeZoom));
    const staticLayer = new StaticLayer(this.gridOptions.tilesUrlTemplate, {
      attribution: '',
      tileSize: point(this.gridOptions.tileWidth, this.gridOptions.tileHeight),
      maxNativeZoom: this.gridOptions.maxNativeZoom,
      minNativeZoom: this.gridOptions.minNativeZoom,
      maxZoom: this.gridOptions.maxZoom ?? this.gridOptions.maxNativeZoom + 1,
      minZoom: this.gridOptions.minZoom ?? this.gridOptions.minNativeZoom,
      bounds: layerBounds
    });
    return staticLayer;
  }

  onMapMove(evt: any) {
    if (this.gridOptions == null)
      return;
    if (this.map == null)
      return;
    const zoom = this.map.getZoom();
    const layersDiff = this.gridOptions.maxNativeZoom - zoom;

    let mapCenter = this.map.project(this.map.getCenter(), zoom);
    let mapSize = this.map.getSize();

    this.xElemMarginLeft = mapSize.x / 2 - mapCenter.x;
    this.xElemWidth = this.gridOptions.tileWidth / Math.pow(2, layersDiff);

    this.yElemMarginTop = mapSize.y / 2 - mapCenter.y;
    this.yElemHeight = this.gridOptions.tileHeight / Math.pow(2, layersDiff);
  }

  onMapReady(map: Map) {
    if (this.gridOptions == null)
      return;
    this.map = map;

    const staticLayer = this.buildImagesLayer(map);
    this.mapLayers.push(staticLayer);

    const geoLayer = this.buildGeoJsonLayer(map);
    this.mapLayers.push(geoLayer);

    const totalWidth = this.gridOptions.tileWidth * this.gridOptions.xTiles;
    const totalLength = this.gridOptions.tileHeight * this.gridOptions.yTiles;
    const centerX = totalWidth / 2;
    const centerY = totalLength / 2;
    const centerLatLng = map.unproject([centerX, centerY], this.gridOptions.maxNativeZoom);
    map.setView(centerLatLng, this.gridOptions.startZoom!);

    //TODO change logic
    const w = this.gridOptions.tileWidth * 3;
    const h = this.gridOptions.tileHeight * 3;
    const mapBounds = latLngBounds(
      map.unproject([-w, -h], this.gridOptions.maxNativeZoom),
      map.unproject([w + totalWidth, h + totalLength], this.gridOptions.maxNativeZoom));
    this.map.setMaxBounds(mapBounds);
  }
}


@Injectable({providedIn: 'root'})
export class PopUpService {
  constructor(
    private injector: Injector,
    private applicationRef: ApplicationRef,
    private componentFactoryResolver: ComponentFactoryResolver
  ) {
  }

  returnPopUpHTML(tileInfo: ITileInfo, viewContainerRef: ViewContainerRef): HTMLElement {
    // Create element
    const popup = document.createElement('popup-component');
    const factory = this.componentFactoryResolver.resolveComponentFactory(MapImagePopupComponent);
    const popupComponentRef = factory.create(this.injector, [], popup);
    this.applicationRef.attachView(popupComponentRef.hostView);
    popupComponentRef.instance.tileInfo = tileInfo;
    return popup;
  }
}

class StaticLayer extends TileLayer {
  public override getTileUrl(coords: any) {
    console.log(coords);
    return super.getTileUrl.call(this, coords);
  }
}
