using FileTypeChecker.Extensions;
using FileTypeChecker.Types;

namespace WestmarchBook.Application.Extensions;

public static class StreamExtensions
{
    internal static string? DetectImageContentType(this Stream stream)
    {
        if (stream.CanRead == false || (stream.CanSeek && stream.Length == 0))
        {
            return null;
        }

        string? result = null;

        if (stream.Is<PortableNetworkGraphic>())
        {
            result = "image/png";
        }
        else if (stream.Is<JointPhotographicExpertsGroup>())
        {
            result = "image/jpeg";
        }

        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        return result;
    }
}
