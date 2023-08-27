using System.Text.RegularExpressions;
using VGAudio.Containers.Wave;
using VGAudio.Formats;

namespace FF6PR2MSU;

class Program
{
    public const string GAME_CODE_FF4 = "FF4";
    public const string GAME_CODE_FF5 = "FF5";
    public const string GAME_CODE_FF6 = "FF6";

    private const string OUTPUT_DIRECTORY_PATH = "output"; // relative

    public static void Main(string[] args)
    {
        if (args.Length == 0 || args.Any(a => a.Contains("-h") || a.Contains("--help") || a.Contains("/?")))
        {
            PrintMan();
            return;
        }

        var bundleFilePath = args[0];

        if (!File.Exists(bundleFilePath))
        {
            Console.WriteLine($"Could not find the file \"{bundleFilePath}\". Exiting program.");
            return;
        }

        if (Path.GetExtension(bundleFilePath)?.ToLower() != ".bundle")
        {
            Console.WriteLine($"The provided file \"{bundleFilePath}\" is not a Unity bundle file. Exiting program.");
            return;
        }

        string gameCode;
        string bundleFileName = Path.GetFileName(bundleFilePath);

        /*if (bundleFileName.Contains(GAME_CODE_FF4, StringComparison.CurrentCultureIgnoreCase))
        {
            gameCode = GAME_CODE_FF4;
        }
        else if (bundleFileName.Contains(GAME_CODE_FF5, StringComparison.CurrentCultureIgnoreCase))
        {
            gameCode = GAME_CODE_FF5;
        }
        else*/ if (bundleFileName.Contains(GAME_CODE_FF6, StringComparison.CurrentCultureIgnoreCase))
        {
            gameCode = GAME_CODE_FF6;
        }
        else
        {
            // TODO: ask which FF game it is in case the bundle file was renamed
            Console.WriteLine("Either this Final Fantasy VI Pixel Remaster BGM Unity bundle file was renamed or it's from some other game (other Pixel Remaster games are not supported). Exiting program.");
            return;
        }

        BundleFileExtractor bundle;

        try
        {
            bundle = new BundleFileExtractor(bundleFilePath);
        }
        catch
        {
            Console.WriteLine("Bundle file not valid. Exiting program.");
            return;
        }

        string? languageCharacterCode = null;

        if (gameCode == GAME_CODE_FF6)
        {
            do
            {
                Console.WriteLine(); // skips a line so its cleaner looking
                Console.WriteLine("Which language for the opera scenes?");
                Console.WriteLine("1 - No voice/instrumental (default)");
                Console.WriteLine("2 - Dutch");
                Console.WriteLine("3 - English");
                Console.WriteLine("4 - French");
                Console.WriteLine("5 - Italian");
                Console.WriteLine("6 - Japanese");
                Console.WriteLine("7 - Korean");
                Console.WriteLine("8 - Spanish");

                switch (Console.ReadLine()?.Trim())
                {
                    case "":
                    case "1":
                        languageCharacterCode = string.Empty;
                        break;
                    case "2":
                        languageCharacterCode = "_DEU";
                        break;
                    case "3":
                        languageCharacterCode = "_ENG";
                        break;
                    case "4":
                        languageCharacterCode = "_FRE";
                        break;
                    case "5":
                        languageCharacterCode = "_ITA";
                        break;
                    case "6":
                        languageCharacterCode = "_JPN";
                        break;
                    case "7":
                        languageCharacterCode = "_KOR";
                        break;
                    case "8":
                        languageCharacterCode = "_SPA";
                        break;
                }
            } while (languageCharacterCode == null);
        }

        TrackNameParser parser;

        try
        {
            parser = new TrackNameParser(gameCode, languageCharacterCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        string? romFileName = null;

        do
        {
            Console.WriteLine("\nName of the rom (without extension)?");
            romFileName = Console.ReadLine()?.TrimEnd();

            if (Path.GetInvalidFileNameChars().Any(c => (romFileName ?? string.Empty).Contains(c)))
            {
                romFileName = null;
                Console.WriteLine("This file name contains invalid characters. Try again.");
            }
        } while (string.IsNullOrEmpty(romFileName));

        if (!Directory.Exists(OUTPUT_DIRECTORY_PATH))
        {
            Directory.CreateDirectory(OUTPUT_DIRECTORY_PATH);
        }

        for (int i = 0; bundle.GetAsset(i, out string name, out byte[] data); i++)
        {
            var match = Regex.Match(name, gameCode + "_[0-9]+[abcde12]*(_[A-Z]{3})*");
            string msuName;
            
            if (!match.Success || (msuName = parser.LookupName(match.Value.Substring(4))) == null)
            {
                Console.WriteLine($@"""{name}"" is not part of the msu patch. Skipping.");
                continue;
            }

            byte[] convertedWaveAudioData;

            try
            {
                convertedWaveAudioData = AudioMogHcaDecoder.ConvertMabfToWav(data);
            }
            catch
            {
                Console.WriteLine($@"An error occured while converting ""{name}"". Skipping.");
                continue;
            }

            IAudioFormat format = new WaveReader().ReadFormat(convertedWaveAudioData);

            if (Wav2Msu.Convert(convertedWaveAudioData, Path.Join(OUTPUT_DIRECTORY_PATH, $"{romFileName}-{msuName}.pcm"), format.Looping ? format.LoopStart : 0))
            {
                Console.WriteLine($@"""{name}"" has been converted successfully.");
            }
            else
            {
                Console.WriteLine($@"""{name}"" could not be converted.");
            }
        }

        Console.WriteLine("MSU-1 audio files created successfully. Press RETURN to exit the program...");
        Console.ReadLine();
    }

    private static void PrintMan()
    {
        Console.WriteLine(
            "=========+ Final Fantasy VI Pixel Remaster to MSU-1 conversion tool +========= \n" +
            "https://github.com/GoldenKain/FF6PR2MSU\n" + 
            "How to use? Simply drag and drop the appropriate music .bundle file (file containing Unity assets) from the directory of the PC version of Final Fantasy VI Pixel Remaster and you should be good to go! Well... Almost...\n\n" +
            "To work, this program also needs (for now at least...) the executable for AudioMog, a program that converts assets of Square Enix games to the desired files (in this case, wave audio files). Here's a link to their github page <https://github.com/Yoraiz0r/AudioMog>. You simply need to download the latest version of the program and extract the contents of the zip file in the same directory as FF6PR2MSU. \n" +
            "Something else you might want to know, the name of the Unity bundle file for Final Fantasy VI Pixel Remaster is named \"ff6_bgm_assets_all_181eb630118efa8542dab51f7e8d2795.bundle\" or something to that effect. I imagine it's possible that the name of the file might change a little after some updates, but that part should stay the same: \"ff6_bgm_assets\". \n\n" +
            "For more information and full credits, please consult the project's Github page.");
    }
}