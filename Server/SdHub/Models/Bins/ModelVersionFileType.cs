namespace SdHub.Models.Bins;

public enum ModelFileType : byte
{
    Unknown = 0,

    //ckpt 10
    CkptModel = 11,
    CkptYaml = 12,

    //safetensor 20
    SafetensorModel = 21,
    SafetensorYaml = 22,

    //onnx 30
    OnnxModel = 31,
    OnnxYaml = 32,
}