//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

export interface IFileModel
{
	id: number;
	name: string;
	hash: string;
	mimeType: string;
	extension: string;
	size: number;
	createdAt: string;
	directUrl: string;
}
export interface IDirectoryModel
{
	id: number;
	name: string;
	size: number;
	createdAt: string;
	directUrl: string;
}
export interface IImportFileRequest
{
	fileUrl: string;
}