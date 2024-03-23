using System.Text.RegularExpressions;

namespace FF6PR2MSU;

public class TrackNameParser
{
    private const string LOOKUP_TABLES_FILE_NAME = "LookupTables.ini";

    // 32b, 33 and 34c are the only FF6 tracks with lyrics that are part of the patch
    private const string TRACK_NAME_LOOKUP_PATTERN = @"(?<={0}_)(?((32b|33|34c))[0-9]+[a-e12]*{1}|[0-9]+[a-e12]*)(?=\.)";

    private readonly Dictionary<string, string[]> wav2msuTable;

    private readonly Regex lookupPattern;

    /// <param name="gameCode">Game's 3-letter code</param> 
    /// <param name="languageCode">Code of the language chosen by the user for the vocal tracks (applicable for FFVI only)</param>
    public TrackNameParser(string gameCode, string languageCode = null)
    {
        this.lookupPattern = new Regex(string.Format(TRACK_NAME_LOOKUP_PATTERN, gameCode, languageCode ?? string.Empty));

        StreamReader iniStream = null;

        try
        {
            string lookupTablePath = LOOKUP_TABLES_FILE_NAME;

            // priority to the lookup table in the cwd
            if (!File.Exists(lookupTablePath))
            {
                lookupTablePath = Path.Join(AppContext.BaseDirectory, LOOKUP_TABLES_FILE_NAME);

                if (!File.Exists(lookupTablePath))
                {
                    throw new Exception($"{LOOKUP_TABLES_FILE_NAME} could not be found.");
                }                
            }

            iniStream = new StreamReader(lookupTablePath);

            if (iniStream == null)
            {
                throw new Exception("The INI file could not be read.");
            }

            var parser = new IniParser.FileIniDataParser();
            parser.Parser.Configuration.AllowKeysWithoutSection = false;
            parser.Parser.Configuration.CommentRegex = new Regex("(^|\\s);(.*)");

            var iniData = parser?.ReadData(iniStream);
            iniData?.ClearAllComments();

            if (iniData == null)
            {
                throw new Exception("There was an error reading the INI file.");
            }

            var section = iniData.Sections.GetSectionData(gameCode);

            if (section == null)
            {
                throw new Exception($@"Could not retried information for section ""{gameCode}"" in the INI file.");
            }

            // Copying data in a Dictionary just because it's simpler to use
            this.wav2msuTable = new Dictionary<string, string[]>();

            foreach (var data in section.Keys)
            {
                string[] values = data.Value.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (values == null || values.Length <= 0)
                {
                    continue;
                }

                wav2msuTable.Add(data.KeyName, values);
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            iniStream?.Close();
            iniStream?.Dispose();
        }
    }

    public bool LookupName(string gameTrackCode, out string[] msuTrackCodes)
    {
        msuTrackCodes = null;
        Match match = lookupPattern.Match(gameTrackCode);

        return match.Success && wav2msuTable.TryGetValue(match.Value, out msuTrackCodes) && msuTrackCodes != null && msuTrackCodes.Length > 0;
    }
}