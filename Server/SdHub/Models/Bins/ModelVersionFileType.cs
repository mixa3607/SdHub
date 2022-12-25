namespace SdHub.Models.Bins;

public enum ModelVersionFileType : byte
{
    Unknown = 0,
    CkptModel,
    SafetensorModel,
    OnnxModel
}