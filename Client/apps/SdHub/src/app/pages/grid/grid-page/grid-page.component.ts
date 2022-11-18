import {Component, OnInit} from '@angular/core';
import {CRS, geoJSON, latLngBounds, Layer, Map, MapOptions, point, tileLayer} from "leaflet";
import {FeatureCollection} from "geojson";


interface IGridLegendTitle {
    name: string;
    background: string;
}

interface IGridOptions {
    xTiles: number;
    yTiles: number;
    tileWidth: number;
    tileHeight: number;
    fromLayer: number;
    toLayer: number;
    tilesUrlTemplate: string;
    xLegend: IGridLegendTitle[];
    yLegend: IGridLegendTitle[];
}

@Component({
    selector: 'app-grid-page',
    templateUrl: './grid-page.component.html',
    styleUrls: ['./grid-page.component.scss'],
})
export class GridPageComponent implements OnInit {
    private map?: Map;

    public mapOptions: MapOptions = {
        layers: [],
        attributionControl: false,
        crs: CRS.Simple
    };
    public mapLayers: Layer[] = [];

    public gridOptions: IGridOptions;
    public xElemWidth = 0;
    public xElemMarginLeft = 0;
    public yElemHeight = 0;
    public yElemMarginTop = 0;


    constructor() {
        const opts: IGridOptions = {
            xTiles: 11,
            yTiles: 11,
            tileWidth: 512,
            tileHeight: 896,
            fromLayer: 18,
            toLayer: 11,
            tilesUrlTemplate: '/layers/{z}/{x}_{y}.webp',
            yLegend: [],
            xLegend: [],
        }
        const xPrompts = [
            'sitting',
            'standing',
            'squatting',
            'rabbit pose',
            'bent over',
            'leaning forward',
            'paw pose',
            'claw pose',
            'crossed legs',
            'ojou-sama pose',
            'fighting stance',
            'legs apart',
            'arms up',
            'arms behind back',
            'arms behind head',
            'hands on hips',
            'hands up',
            'all fours',
            'hands between legs',
            'covering mouth',
            'outstretched arms',
            'top-down bottom-up',
            'on back',
            'on stomach',
            'lying',
            'on side',
            'standing on one leg',
        ]
        const yPrompts = Array.from({length: 151 - 70}, (v, k) => k + 70).map(x => x.toString());
        opts.xTiles = xPrompts.length;
        opts.yTiles = yPrompts.length;

        for (let i = 0; i < opts.xTiles; i++) {
            const background = i % 2 == 0 ? 'rgba(238,238,238,0.9)' : 'rgba(218,218,218,0.9)';
            opts.xLegend.push({name: xPrompts[i], background});
        }
        for (let i = 0; i < opts.yTiles; i++) {
            const background = i % 2 == 0 ? 'rgba(238,238,238,0.9)' : 'rgba(218,218,218,0.9)';
            opts.yLegend.push({name: yPrompts[i], background});
        }
        this.gridOptions = opts;
        console.log(opts);
    }

    ngOnInit(): void {
    }

    onMapMove(evt: any) {
        if (this.map == null)
            return;
        const zoom = this.map.getZoom();
        const layersDiff = this.gridOptions.fromLayer - zoom;

        let mapCenter = this.map.project(this.map.getCenter(), zoom);
        let mapSize = this.map.getSize();

        this.xElemMarginLeft = mapSize.x / 2 - mapCenter.x;
        this.xElemWidth = this.gridOptions.tileWidth / Math.pow(2, layersDiff);

        this.yElemMarginTop = mapSize.y / 2 - mapCenter.y;
        this.yElemHeight = this.gridOptions.tileHeight / Math.pow(2, layersDiff);
        //console.log(mapSize, mapCenter, this.map.getCenter(), this.yElemMarginTop)
    }

    onMapReady(map: Map) {
        this.map = map;

        const geoJson1: FeatureCollection = {
            "type": "FeatureCollection",
            "features": [
                {
                    "type": "Feature",
                    "properties": {
                        "name": "GeoJSON",
                        "popupContent": "This is GeoJSON test"
                    },
                    "geometry": {
                        "coordinates": [
                            [
                                [
                                    map.unproject([0, 0], 18).lng,
                                    map.unproject([0, 0], 18).lat,
                                ],
                                [
                                    map.unproject([0, 896], 18).lng,
                                    map.unproject([0, 896], 18).lat,
                                ],
                                [
                                    map.unproject([512, 896], 18).lng,
                                    map.unproject([512, 896], 18).lat,
                                ],
                                [
                                    map.unproject([512, 0], 18).lng,
                                    map.unproject([512, 0], 18).lat,
                                ],
                            ]
                        ],
                        "type": "Polygon"
                    }
                }
            ]
        };
        const myStyle = {
            "opacity": .5,
            "fillOpacity": 0
        };
        const geoLayer = geoJSON(geoJson1, {
                onEachFeature: function (feature, layer) {
                    layer.bindPopup('<h1>' + feature.properties.name + '</h1><p>name: ' + feature.properties.popupContent + '</p>');
                },
                style: myStyle,
            }
        );
        this.mapLayers.push(geoLayer);

        const totalWidth = this.gridOptions.tileWidth * this.gridOptions.xTiles;
        const totalLength = this.gridOptions.tileHeight * this.gridOptions.yTiles;
        const layerBounds = latLngBounds(
            map.unproject([0, 0], this.gridOptions.fromLayer),
            map.unproject([totalWidth, totalLength], this.gridOptions.fromLayer));

        const centerX = this.gridOptions.tileWidth * this.gridOptions.xTiles / 2;
        const centerY = this.gridOptions.tileHeight * this.gridOptions.yTiles / 2;
        map.setView(map.unproject([this.gridOptions.tileWidth * 2, this.gridOptions.tileHeight * 2], this.gridOptions.fromLayer), this.gridOptions.toLayer + 2);

        const layer = tileLayer(this.gridOptions.tilesUrlTemplate, {
            attribution: '',
            tileSize: point(this.gridOptions.tileWidth, this.gridOptions.tileHeight),
            maxNativeZoom: this.gridOptions.fromLayer,
            maxZoom: this.gridOptions.fromLayer + 1,
            minZoom: this.gridOptions.toLayer,
            bounds: layerBounds
        });
        this.mapLayers.push(layer);

        //TODO change logic
        const mapBounds = latLngBounds(
            map.unproject([-totalWidth, -totalLength], this.gridOptions.fromLayer),
            map.unproject([totalWidth * 1.5, totalLength * 1.5], this.gridOptions.fromLayer));
        this.map.setMaxBounds(mapBounds);
    }
}
