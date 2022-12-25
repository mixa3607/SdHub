using AutoMapper;
using SdHub.Database.Entities.Bins;
using SdHub.Models.Bins;
using SdHub.Models.Bins.Embeddings;
using SdHub.Models.Bins.Hypernets;
using SdHub.Models.Bins.Vaes;
using SdHub.Models.Samples;

namespace SdHub.Automapper;

public class BinsProfile : Profile
{
    public BinsProfile()
    {
        CreateMap<EmbeddingEntity, EmbeddingModel>(MemberList.Destination);
        CreateMap<EmbeddingVersionEntity, EmbeddingVersionModel>(MemberList.Destination);

        CreateMap<ModelEntity, ModelModel>(MemberList.Destination);
        CreateMap<ModelVersionEntity, ModelVersionModel>(MemberList.Destination);
        CreateMap<ModelVersionFileEntity, ModelVersionFileModel>(MemberList.Destination);

        CreateMap<VaeEntity, VaeModel>(MemberList.Destination);
        CreateMap<VaeVersionEntity, VaeVersionModel>(MemberList.Destination);

        CreateMap<HypernetEntity, HypernetModel>(MemberList.Destination);
        CreateMap<HypernetVersionEntity, HypernetVersionModel>(MemberList.Destination);

        CreateMap<GenerationSampleEntity, GenerationSampleModel>(MemberList.Destination);
    }
}