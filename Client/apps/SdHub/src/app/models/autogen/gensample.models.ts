//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

import { IImageModel } from './misc.models';
import { IModelVersionModel } from './model.models';
import { IHypernetVersionModel } from './hypernet.models';
import { IVaeVersionModel } from './vae.models';
import { IEmbeddingVersionModel } from './embedding.models';

export interface IGenerationSampleModel
{
	id: number;
	image: IImageModel;
	modelVersionId: number;
	modelVersion: IModelVersionModel;
	hypernetVersionId: number;
	hypernetVersion: IHypernetVersionModel;
	vaeVersionId: number;
	vaeVersion: IVaeVersionModel;
	embeddingVersionId: number;
	embeddingVersion: IEmbeddingVersionModel;
}
