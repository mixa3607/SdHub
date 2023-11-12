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

        CreateMap<ModelEntity, ModelModel>(MemberList.Destination);
        CreateMap<ModelFileEntity, ModelVersionFileModel>(MemberList.Destination);

        CreateMap<VaeEntity, VaeModel>(MemberList.Destination);

        CreateMap<HypernetEntity, HypernetModel>(MemberList.Destination);

        CreateMap<GenerationSampleEntity, GenerationSampleModel>(MemberList.Destination);
    }
}