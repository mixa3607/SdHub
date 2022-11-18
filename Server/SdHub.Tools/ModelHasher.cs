using System.Security.Cryptography;

public class ModelHasher
{
    public void Run()
    {
        var srcFilesFile = "H:/STABLE_DIFFUSION/all_ckpt.txt";
        var dstHashesFile = "H:/STABLE_DIFFUSION/_999_all_ckpt_hashes.md";
        using var dstHashes = File.AppendText(dstHashesFile);
        dstHashes.WriteLine("| file | hash_v1 |");
        dstHashes.WriteLine("|------|---------|");
        foreach (var filePath in File.ReadAllLines(srcFilesFile))
        {
            Console.WriteLine(filePath);
            var absFilePath = Path.Combine("H:/STABLE_DIFFUSION/", filePath);
            var hash = BuildModelHash(absFilePath);
            dstHashes.WriteLine($"{filePath}|{hash}");
        }
    }

    string BuildModelHash(string fileName)
    {
        var hasher = SHA256.Create();
        using var ckpt = File.OpenRead(fileName);
        var buff = new byte[0x10000];
        ckpt.Seek(0x100000, SeekOrigin.Begin);
        ckpt.Read(buff);
        var hash = hasher.ComputeHash(buff);
        var hStr = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        return hStr;
    }
}