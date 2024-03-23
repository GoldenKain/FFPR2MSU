using System.Text.RegularExpressions;

namespace FF6PR2MSU;

public class TrackNameParser
{
    private const string LOOKUP_TABLES_FILE_NAME = "LookupTables.ini";

    // 32b, 33 and 34c are the only FF6 tracks with lyrics that are part of the patch
    private const string TRACK_NAME_LOOKUP_PATTERN = @"(?<={0}_)(?((32b|33|34c))[0-9]+[a-e12]*{1}|[0-9]+[a-e12]*)(?=\.)";

    private readonly Dictionary<string, List<string>> wav2msuTable;

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
            var section = parser?.ReadData(iniStream)?.Sections.GetSectionData(gameCode);

            if (section == null)
            {
                throw new Exception("This game is not supported or the INI file is invalid.");
            }

            // Copying data in a Dictionary just because it's simpler to use
            this.wav2msuTable = new Dictionary<string, List<string>>();

            foreach (var data in section.Keys)
            {
                // APPARENTLY... the parser can't understand comment characters when used at the end of lines... So I have to handle them myself.
                // What's the point of using an external library if it won't even properly parse the data...? Might as well redo it myself. TBD.
                if (data.Value.Contains(';'))
                {
                    data.Value = data.Value.Remove(data.Value.IndexOf(';')).Trim();
                }

                string[] values = data.Value.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (values == null || values.Length <= 0)
                {
                    continue;
                }

                if (wav2msuTable.TryGetValue(data.KeyName, out List<string> tracks))
                {
                    tracks.AddRange(values);
                }
                else
                {
                    wav2msuTable.Add(data.KeyName, new List<string>(values));
                }
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

    public bool LookupName(string gameTrackCode, out List<string> msuTrackCodes, out string sanitizedGameTrackCode)
    {
        msuTrackCodes = null;
        sanitizedGameTrackCode = null;
        Match match = lookupPattern.Match(gameTrackCode);

        return match.Success && wav2msuTable.TryGetValue(sanitizedGameTrackCode = match.Value, out msuTrackCodes);
    }
}