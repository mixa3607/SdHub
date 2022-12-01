using AutoMapper;
using SdHub.Database.Entities.Bins;
using SdHub.Models.Bins;

namespace SdHub.Automapper;

public class BinsProfile : Profile
{
    public BinsProfile()
    {
        CreateMap<EmbeddingEntity, EmbeddingModel>(MemberList.Destination);
        CreateMap<EmbeddingVersionEntity, EmbeddingVersionModel>(MemberList.Destination);

        CreateMap<ModelEntity, ModelModel>(MemberList.Destination);
        CreateMap<ModelVersionEntity, ModelVersionModel>(MemberList.Destination);

        CreateMap<VaeEntity, VaeModel>(MemberList.Destination);
        CreateMap<VaeVersionEntity, VaeVersionModel>(MemberList.Destination);

        CreateMap<HypernetEntity, HypernetModel>(MemberList.Destination);
        CreateMap<HypernetVersionEntity, HypernetVersionModel>(MemberList.Destination);

        CreateMap<GenerationSampleEntity, GenerationSampleModel>(MemberList.Destination);
    }
}