//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

import { IUserSimpleModel } from './misc.models';
import { IFileModel } from './file.models';
import { IDirectoryModel } from './file.models';
import { IImageModel } from './misc.models';

export interface IGridModel
{
	id: number;
	shortUrl: string;
	shortToken: string;
	deletedAt: string|null;
	createdAt: string;
	name: string;
	description: string;
	owner: IUserSimpleModel;
	gridImages: IGridImageModel[];
	xTiles: number;
	yTiles: number;
	xValues: string[];
	yValues: string[];
	minLayer: number;
	maxLayer: number;
	thumbImage: IFileModel;
	layersDirectory: IDirectoryModel;
}
export interface IGridImageModel
{
	order: number;
	image: IImageModel;
}
export interface IGetGridRequest
{
	shortToken: string;
}
export interface IGetGridResponse
{
	grid: IGridModel;
}
export interface ISearchGridRequest
{
	searchText: string;
	owner: string;
	album: string;
	fields: SearchGridInFieldType[];
	orderByField: SearchGridOrderByFieldType;
	orderBy: SearchGridOrderByType;
	skip: number;
	take: number;
}
export interface IEditGridRequest
{
	shortToken: string;
	name?: string;
	description?: string;
}
export interface IEditGridResponse
{
	grid: IGridModel;
	success: boolean;
	reason: string;
}
export interface IDeleteGridRequest
{
	shortToken: string;
}
export interface IDeleteGridResponse
{
	success: boolean;
	reason: string;
}
export enum SearchGridInFieldType {
	Name = 0,
	Description = 1,
	User = 2
}
export enum SearchGridOrderByFieldType {
	UploadDate = 0,
	UserName = 1
}
export enum SearchGridOrderByType {
	Asc = 0,
	Desc = 1
}
export interface IUploadGridRequest
{
	albumShortToken: string;
	file: any;
	xTiles: number;
	yTiles: number;
	xValues: string;
	yValues: string;
}
export interface IUploadGridResponse
{
	uploaded: boolean;
	reason: string;
	grid: IGridModel;
}
export interface IUploadGridCheckInputRequest
{
	xTiles: number;
	yTiles: number;
	xValues: string;
	yValues: string;
}
