using System.Xml.Linq;

namespace FF6PR2MSU;

public static class Conversion
{
    // Instantiated in case it's called before init I guess
    private static Dictionary<string, string> wav2msuDictionary = new Dictionary<string, string>();

    public static void InitializeDictionary(string languageCode)
    {
        XDocument doc = XDocument.Load("wav2msu.xml");
        IEnumerable<XElement> rootNodes = doc.Root.DescendantNodes().OfType<XElement>();
        wav2msuDictionary = rootNodes.ToDictionary(v => string.Format(v.Value, languageCode), v => v.Attribute("id").Value);
    }

    public static string GetName(string wavFileName)
    {
        if (!wav2msuDictionary.TryGetValue(wavFileName, out string pcmNumber))
        {
            return null;
        }

        return pcmNumber;
    }
}