#if TESTWAV
using System.Text.RegularExpressions;
#else
using VGAudio.Containers.Wave;
using VGAudio.Formats;
#endif

namespace FF6PR2MSU;

class Program
{
    public const string GAME_CODE_FF4 = "FF4";
    public const string GAME_CODE_FF5 = "FF5";
    public const string GAME_CODE_FF6 = "FF6";

    private const string MSU_FILE_NAME = "{0}-{1}.pcm";

#if TESTWAV
    private static readonly Regex SIMPLE_WAV_NAME_PATTERN = new Regex(@"(?<=SWAV_BGM_)\w+");
#endif

    private static readonly string OUTPUT_DIRECTORY_PATH = "output"; // relative (cwd)

    static Program()
    {
        int i = 0;
        string outputDirPath = OUTPUT_DIRECTORY_PATH;

        while (Directory.Exists(outputDirPath))
        {
            outputDirPath = OUTPUT_DIRECTORY_PATH + ++i;
        }

        OUTPUT_DIRECTORY_PATH = outputDirPath;
    }

    public static void Main(string[] args)
    {
        if (args.Length == 0 || args.Any(a => a == "-h" || a == "--help" || a == "/?"))
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
            Console.WriteLine($"The provided file \"{bundleFilePath}\" is not a valid Unity bundle file. Exiting program.");
            return;
        }

#if !TESTWAV
        string gameCode = null;
        string bundleFileName = Path.GetFileName(bundleFilePath);

        if (bundleFileName.Contains(GAME_CODE_FF4, StringComparison.InvariantCultureIgnoreCase))
        {
            gameCode = GAME_CODE_FF4;
        }
        else if (bundleFileName.Contains(GAME_CODE_FF5, StringComparison.InvariantCultureIgnoreCase))
        {
            gameCode = GAME_CODE_FF5;
        }
        else if (bundleFileName.Contains(GAME_CODE_FF6, StringComparison.InvariantCultureIgnoreCase))
        {
            gameCode = GAME_CODE_FF6;
        }
        else
        {
            do
            {
                Console.WriteLine("The Pixel Remaster game the Unity bundle file is taken from could not be inferred from the file's name.\nPlease select the Final Fantasy Pixel Remaster game it's taken from to continue:");
                Console.WriteLine("1 - Final Fantasy IV");
                Console.WriteLine("2 - Final Fantasy V");
                Console.WriteLine("3 - Final Fantasy VI (default)");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1":
                        gameCode = GAME_CODE_FF4;
                        break;
                    case "2":
                        gameCode = GAME_CODE_FF5;
                        break;
                    case "":
                    case "3":
                        gameCode = GAME_CODE_FF6;
                        break;
                    default:
                        break;
                }

                Console.WriteLine(); // skips a line so its cleaner looking
            } while(gameCode == null);
        }

        string languageCharacterCode = null;

        if (gameCode == GAME_CODE_FF6)
        {
            do
            {
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

                Console.WriteLine(); // skips a line so its cleaner looking
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

        string romFileName = null;

        do
        {
            Console.WriteLine(
                "\nName of the {0} rom file (without extension)?",
                gameCode switch {
                    GAME_CODE_FF4 => "Final Fantasy IV",
                    GAME_CODE_FF5 => "Final Fantasy V",
                    GAME_CODE_FF6 => "Final Fantasy VI",
                    _ => string.Empty
                });
            romFileName = Console.ReadLine()?.TrimEnd();

            if (Path.GetInvalidFileNameChars().Any(c => (romFileName ?? string.Empty).Contains(c)))
            {
                romFileName = null;
                Console.WriteLine("This file name contains invalid characters. Try again.");
            }

            Console.WriteLine(); // skips a line so its cleaner looking
        } while (string.IsNullOrEmpty(romFileName));
#endif

        BundleFileExtractor bundle;

        try
        {
            bundle = new BundleFileExtractor(bundleFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + " Exiting program.");
            return;
        }

        try
        {
            Directory.CreateDirectory(OUTPUT_DIRECTORY_PATH);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        for (int i = 0; bundle.GetAsset(i, out string name, out byte[] data); i++)
        {
#if !TESTWAV
            if (!parser.LookupName(name, out string[] msuNames))
            {
                Console.WriteLine($@"""{name}"" is not part of the msu patch. Skipping.");
                continue;
            }
#endif

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

#if TESTWAV
            string wavName = name;

            // Attempting to simplify output name
            Match match = SIMPLE_WAV_NAME_PATTERN.Match(wavName);
            if (match.Success)
            {
                wavName = match.Value;
            }

            try
            {
                File.WriteAllBytes(Path.Join(OUTPUT_DIRECTORY_PATH, wavName + ".wav"), convertedWaveAudioData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
#else
            IAudioFormat format = new WaveReader().ReadFormat(convertedWaveAudioData);

            string convertedMsuFileName0 = Path.Join(OUTPUT_DIRECTORY_PATH, string.Format(MSU_FILE_NAME, romFileName, msuNames[0]));

            if (Wav2Msu.Convert(convertedWaveAudioData, convertedMsuFileName0, format.Looping ? format.LoopStart : 0))
            {
                for (int j = 1; j < msuNames.Length; j++)
                {
                    try
                    {
                        File.Copy(convertedMsuFileName0, Path.Join(OUTPUT_DIRECTORY_PATH, string.Format(MSU_FILE_NAME, romFileName, msuNames[j])));
                    }
                    catch
                    {
                        // I guess just skipping for now? It really shouldn't happen if the first one worked well... We should have read/write permissions and everything.
                    }
                }

                Console.WriteLine($@"""{name}"" has been converted successfully.");
            }
            else
            {
                Console.WriteLine($@"""{name}"" could not be converted.");
            }
        }

        Console.WriteLine("MSU-1 audio files created successfully. Press RETURN to exit the program...");
        Console.ReadLine();
#endif
    }

    private static void PrintMan()
    {
        Console.WriteLine(
            "=========+ Final Fantasy VI Pixel Remaster to MSU-1 conversion tool +========= \n" +
            "https://github.com/GoldenKain/FF6PR2MSU\n" + 
            "How to use? Drag and drop the appropriate BGM assets file (the game's Unity .bundle file containing background music assets) from the directory of the PC version of Final Fantasy VI Pixel Remaster.\n\n" +
            "N.B.: At the time of writing this, the name of the Unity bundle file for Final Fantasy VI Pixel Remaster is named \"ff6_bgm_assets_all_2a6190d8b5232a0a02876b83e0f742cc.bundle\", but it's possible that it could change after an update. However, it's safe to assume that this part will stay the same: \"ff6_bgm_assets\".\n\n" +
            "For more information and full credits, please consult the project's Github page.");
    }
}