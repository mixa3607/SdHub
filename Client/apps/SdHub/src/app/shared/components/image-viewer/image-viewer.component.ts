import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
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
import {FeatureCollection} from "geojson";

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
    tilesUrlTemplate: string;

    showLegend: boolean;
    xLegend: IGridLegendTitle[];
    yLegend: IGridLegendTitle[];
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


    constructor() {
    }

    public initOptions(opts: IGridOptions): void {
        this.gridOptions = opts;
    }

    ngOnInit(): void {
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

        const totalWidth = this.gridOptions.tileWidth * this.gridOptions.xTiles;
        const totalLength = this.gridOptions.tileHeight * this.gridOptions.yTiles;
        const layerBounds = latLngBounds(
            map.unproject([0, 0], this.gridOptions.maxNativeZoom),
            map.unproject([totalWidth, totalLength], this.gridOptions.maxNativeZoom));

        const centerX = this.gridOptions.tileWidth * this.gridOptions.xTiles / 2;
        const centerY = this.gridOptions.tileHeight * this.gridOptions.yTiles / 2;
        map.setView(map.unproject([this.gridOptions.tileWidth * 2, this.gridOptions.tileHeight * 2], this.gridOptions.maxNativeZoom), this.gridOptions.minNativeZoom + 2);

        const t = new StaticLayer(this.gridOptions.tilesUrlTemplate, {
            attribution: '',
            tileSize: point(this.gridOptions.tileWidth, this.gridOptions.tileHeight),
            maxNativeZoom: this.gridOptions.maxNativeZoom,
            minNativeZoom: this.gridOptions.minNativeZoom,
            maxZoom: this.gridOptions.maxZoom ?? this.gridOptions.maxNativeZoom + 1,
            minZoom: this.gridOptions.minZoom ?? this.gridOptions.minNativeZoom,
            bounds: layerBounds
        });
        this.mapLayers.push(t);


        //TODO change logic
        const mapBounds = latLngBounds(
            map.unproject([-totalWidth, -totalLength], this.gridOptions.maxNativeZoom),
            map.unproject([totalWidth * 1.5, totalLength * 1.5], this.gridOptions.maxNativeZoom));
        this.map.setMaxBounds(mapBounds);
    }
}

class StaticLayer extends TileLayer {
    public override getTileUrl(coords: any) {
        console.log(coords);
        return super.getTileUrl.call(this, coords);
    }
}