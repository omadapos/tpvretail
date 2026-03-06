namespace OmadaPOS.Libreria.Utils;

public class Util
{
    public static bool ValidateURL(string urlText)
    {
        bool result;

        try
        {
            Uri check = new Uri(urlText);
            result = true;
        }
        catch (UriFormatException)
        {
            result = false;
        }

        return result;
    }
}
