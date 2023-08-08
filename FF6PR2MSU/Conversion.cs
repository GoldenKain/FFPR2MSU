using System.Xml.Linq;
using System.Reflection;

namespace FF6PR2MSU;

public static class Conversion
{
    // Instantiated in case it's called before init I guess
    private static Dictionary<string, string> wav2msuDictionary = new Dictionary<string, string>();

    public static void InitializeDictionary(string languageCode)
    {
        try
        {
            Stream xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FF6PR2MSU.wav2msu.xml");

            if (xmlStream == null)
            {
                throw new Exception();
            }

            XDocument doc = XDocument.Load(xmlStream);
            IEnumerable<XElement> rootNodes = doc.Root.DescendantNodes().OfType<XElement>();
            wav2msuDictionary = rootNodes.ToDictionary(v => string.Format(v.Value, languageCode), v => v.Attribute("id").Value);
        }
        catch
        {
            Console.WriteLine("Error occured when reading the XML file.");
            Environment.Exit(0);
        }
    }

    public static string LookupName(string wavFileName)
    {
        if (!wav2msuDictionary.TryGetValue(wavFileName, out string pcmNumber))
        {
            return null;
        }

        return pcmNumber;
    }
}