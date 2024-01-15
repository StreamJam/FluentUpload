using System.Net;

namespace FluentUploads;

internal class CustomProgressContent : HttpContent
{
    private readonly Stream _stream;
    private readonly Action<decimal>? _onProgress;
    private readonly int _bufferSize = 4096;

    public CustomProgressContent(Stream stream, Action<decimal>? onOnProgress = null)
    {
        _stream = stream;
        _onProgress = onOnProgress;
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var buffer = new byte[_bufferSize];
        long totalBytesRead = 0;
        int bytesRead;

        while ((bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            await stream.WriteAsync(buffer, 0, bytesRead);
            totalBytesRead += bytesRead;
            _onProgress?.Invoke(totalBytesRead / _stream.Length);
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        length = _stream.Length;
        return true;
    }
}
