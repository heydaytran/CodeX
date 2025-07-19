using MassTransit;

namespace Endpoints.CustomResults;

public class SevenZipContentResult(string content) : IResult
{
    private const string ContentType = "binary/7z";

    private static readonly CoderPropID[] PropertyIds = [
        CoderPropID.DictionarySize,
        CoderPropID.PosStateBits,
        CoderPropID.LitContextBits,
        CoderPropID.LitPosBits,
        CoderPropID.Algorithm,
        CoderPropID.NumFastBytes,
        CoderPropID.MatchFinder,
        CoderPropID.EndMarker];

    private static readonly object[] Properties = [
        1 << 16, // Dictionary size.
        2,
        3,
        0,
        2,
        128,
        "bt4",
        false];

    private readonly string _content = content ??
        throw new ArgumentNullException(nameof(content));

    /// <inherit />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        byte[] arrayToCompress = Encoding.UTF8.GetBytes(_content);

        using Stream inStream = new MemoryStream(arrayToCompress);
        using Stream outStream = new MemoryStream();

        SevenZip.Compression.LZMA.Encoder encoder = new();
        encoder.SetCoderProperties(PropertyIds, Properties);
        encoder.WriteCoderProperties(outStream);

        long length = inStream.Length;
        for (int i = 0; i < 8; i++)
        {
            outStream.WriteByte((byte)(length >> 8 * i));
        }

        encoder.Code(inStream, outStream, -1, -1, null);

        outStream.Position = 0;

        httpContext.Response.OnStarting(() =>
        {
            httpContext.Response.Headers.Append("Content-Encoding", "7zip");
            return Task.CompletedTask;
        });

        var fileResult = Results.File(
            outStream,
            ContentType);
        await fileResult.ExecuteAsync(httpContext);
    }
}
