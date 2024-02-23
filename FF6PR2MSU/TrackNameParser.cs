using System.Text.RegularExpressions;

namespace FF6PR2MSU;

public class TrackNameParser
{
    private const string LOOKUP_TABLES_FILE_NAME = "LookupTables.ini";

    // 32b, 33 and 34c are the only FF6 tracks with lyrics that are part of the patch
    private const string TRACK_NAME_LOOKUP_PATTERN = @"(?<={0}_)(?((32b|33|34c))[0-9]+[a-e12]*{1}|[0-9]+[a-e12]*)(?=\.)";

    private readonly Dictionary<string, string> wav2msuTable;

    private readonly Regex lookupPattern;

    /// <param name="gameCode">Game's code as </param>
    /// <param name="languageCode">Code of the language chosen by the user for the vocal tracks (applicable for FFVI only)</param>
    public TrackNameParser(string gameCode, string languageCode = null)
    {
        this.lookupPattern = new Regex(string.Format(TRACK_NAME_LOOKUP_PATTERN, gameCode, languageCode ?? string.Empty));

        StreamReader iniStream = null;

        try
        {
            if (!File.Exists(LOOKUP_TABLES_FILE_NAME))
            {
                throw new Exception($"{LOOKUP_TABLES_FILE_NAME} could not be found.");
            }

            iniStream = new StreamReader(LOOKUP_TABLES_FILE_NAME);

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
            this.wav2msuTable = new Dictionary<string, string>();

            foreach (var data in section.Keys)
            {
                var value = data.Value;

                // APPARENTLY... the parser can't understand comment characters when used at the end of lines... So I have to handle them myself.
                // What's the point of using an external library if it won't even properly parse the data...? Might as well redo it myself. TBD.
                if (value.Contains(';'))
                {
                    value = value.Remove(value.IndexOf(';')).Trim();
                }

                wav2msuTable.Add(data.KeyName, value);
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

    public bool LookupName(string gameTrackCode, out string msuTrackCode)
    {
        msuTrackCode = null;
        Match match = lookupPattern.Match(gameTrackCode);

        return match.Success && wav2msuTable.TryGetValue(match.Value, out msuTrackCode);
    }
}