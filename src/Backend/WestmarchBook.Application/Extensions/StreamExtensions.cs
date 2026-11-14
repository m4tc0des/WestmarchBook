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

        string? contentType = null;

        if (stream.Is<PortableNetworkGraphic>())
        {
            contentType = "image/png";
        }

        else if(stream.Is<JointPhotographicExpertsGroup>())
        {
            contentType = "image/jpeg";
        }

        return contentType;
    }
}
