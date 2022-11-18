public class LimitedFileStream : Stream
{
    private readonly FileStream _fileStream;
    private long _begin;
    private long _length;

    public LimitedFileStream(FileStream fileStream, long begin, long length)
    {
        _fileStream = fileStream;
        _begin = begin;
        _length = length;
        _fileStream.Seek(_begin, SeekOrigin.Begin);
    }
    public override void Flush()
    {
        _fileStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _fileStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _fileStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _fileStream.Flush();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead => _fileStream.CanRead;
    public override bool CanSeek => _fileStream.CanSeek;
    public override bool CanWrite => false;
    public override long Length => _length;

    public override long Position
    {
        get => _fileStream.Position - _begin;
        set => _fileStream.Position = value + _begin;
    }
}