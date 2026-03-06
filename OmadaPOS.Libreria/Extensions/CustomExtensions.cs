using OmadaPOS.Libreria.Utils;

namespace OmadaPOS.Libreria.Extensions;

public static class StringExtension
{
    public static string ConvertUrlString(this string image)
    {
        if (Uri.TryCreate(image, UriKind.Absolute, out Uri uriResult)
           && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
        {
            return image;
        }
         
        var s = string.Concat(Constants.URL_STORAGE, image);

        return s;
    }
}
