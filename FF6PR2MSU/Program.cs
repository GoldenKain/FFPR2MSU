using AssetsTools;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using VGAudio.Containers.Wave;
using VGAudio.Formats;

namespace FF6PR2MSU;

class Program
{
    public const string GAME_CODE_FF4 = "FF4";
    public const string GAME_CODE_FF5 = "FF5";
    public const string GAME_CODE_FF6 = "FF6";

    private const string TEMP_DIRECTORY_PATH = "tmp"; // relative
    private const string OUTPUT_DIRECTORY_PATH = "output"; // relative

    public static void Main(string[] args)
    {
        if (args.Length == 0 || args.Any(a => a.Contains("-h") || a.Contains("--help") || a.Contains("/?") || a.Contains("help")))
        {
            PrintMan();
            return;
        }

        var bundleFilePath = args[0];

        if (!File.Exists(bundleFilePath))
        {
            Console.WriteLine($"Could not find the file \"{bundleFilePath}\". Exiting program.");
        }

        if (Path.GetExtension(bundleFilePath)?.ToLower() != ".bundle")
        {
            Console.WriteLine($"The provided file \"{bundleFilePath}\" is not a Unity bundle file. Exiting program.");
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
            Console.WriteLine("Either this Final Fantasy VI Pixel Remaster BGM Unity bundle file was renamed or it's for one of the other Pixel Remaster games (which are not supported). Exiting program.");
            return;
        }

        if (!Directory.Exists(OUTPUT_DIRECTORY_PATH))
        {
            Directory.CreateDirectory(OUTPUT_DIRECTORY_PATH);
        }

        CleanUp(true);

        if (!ParseBundleFile(args[0]) || Directory.GetFiles(TEMP_DIRECTORY_PATH).Length < 1)
        {
            Console.WriteLine("Bundle file not valid. Exiting program.");
            CleanUp();
            return;
        }

        if (!ParseRawAudioAssets())
        {
            Console.WriteLine("An error occured when converting the audio files. Exiting program.");
            CleanUp();
            return;
        }

        var files = Directory.GetFiles(TEMP_DIRECTORY_PATH, "*.wav", SearchOption.AllDirectories);

        if (files == null || files.Length < 1)
        {
            Console.WriteLine("An error occured when converting the audio files. Exiting program.");
            CleanUp();
            return;
        }

        string? languageCharacterCode = null;

        do
        {
            Console.WriteLine("Which language for the opera scenes? (default: 1)");
            Console.WriteLine("1 - No voice");
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
            
            Console.WriteLine(); // skips a line so its cleaner
        } while (languageCharacterCode == null);

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

        string romFileName;

        do
        {
            try
            {
                Console.WriteLine("Name of the rom (without extension)?");
                string? name = Console.ReadLine()?.TrimEnd();

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (Path.GetInvalidFileNameChars().Any(c => name.Contains(c)))
                {
                    Console.WriteLine("This file name contains invalid characters. Try again.");
                    continue;
                }

                romFileName = name;
                break;
            }
            finally
            {
                Console.WriteLine();
            }
        } while (true);

        foreach (string file in files)
        {
            string filename = Path.GetFileName(file);

            if (!File.Exists(file))
            {
                Console.WriteLine($"{filename} This file doesn't exist or doesn't have proper access. Skipping.");
                continue;
            }
            
            if (!file.EndsWith(".wav", StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            var match = Regex.Match(filename, gameCode + "_[0-9]+[abcde12]*(_[A-Z]{3})*");
            string? msuName = null;
            
            if (!match.Success || (msuName = parser.LookupName(match.Value.Substring(4))) == null)
            {
                Console.WriteLine($@"""{filename}"" is not part of the msu patch. Skipping.");
                continue;
            }
            
            using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            IAudioFormat format = new WaveReader().ReadFormat(stream);

            if (Wav2Msu.Convert(file, Path.Join(OUTPUT_DIRECTORY_PATH, $"{romFileName}-{msuName}.pcm"), format.Looping ? format.LoopStart : 0L))
            {
                Console.WriteLine($@"""{filename}"" has been converted successfully.");
            }
            else
            {
                Console.WriteLine($@"""{filename}"" could not be converted.");
            }
        }

        CleanUp();
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

    private static void CleanUp(bool silent = false)
    {
        if (!silent)
        {
            Console.WriteLine("Cleaning work directory...");
        }

        if (!Directory.Exists(TEMP_DIRECTORY_PATH))
        {
            return;
        }

        try 
        {
            foreach (string filePath in Directory.EnumerateFiles(TEMP_DIRECTORY_PATH))
            {
                File.Delete(filePath);
            }

            Directory.Delete(TEMP_DIRECTORY_PATH, true);
        }
        catch
        {
            if (!silent)
            {
                Console.WriteLine("An error occured when running the post-execution cleanup. Continuing regardless.");
            }

            return;
        }
    }

    private static bool ParseRawAudioAssets()
    {
        // TODO find out how to do it by myself without having to resort to calling MogAudio executable...

        if (!Directory.Exists(TEMP_DIRECTORY_PATH))
        {
            Directory.CreateDirectory(TEMP_DIRECTORY_PATH);
        }

        if (!File.Exists("AudioMog.exe") || !File.Exists("TerminalSettings.json"))
        {
            Console.WriteLine("AudioMog (and/or its settings file) not present in the same directory as this program.\n" + "Please, download the latest release of AudioMog at https://github.com/Yoraiz0r/AudioMog and extract the full content of the .zip in the same directory as FF6PR2MSU. Exiting program.");
            return false;
        }

        try
        {
            var settingsFile = string.Empty;
            foreach (var line in File.ReadAllLines("TerminalSettings.json"))
            {
                settingsFile += line + Environment.NewLine;
            }

            string quitWhenDoneSetting = "\"ImmediatelyQuitOnceAllTasksAreDone\": ";
            settingsFile = settingsFile.Replace(quitWhenDoneSetting + "false", quitWhenDoneSetting + "true");

            File.WriteAllText("TerminalSettings.json", settingsFile);

            string audioMogArgs = string.Empty;
            foreach (var file in Directory.GetFiles(TEMP_DIRECTORY_PATH, "*.bytes", SearchOption.TopDirectoryOnly))
            {
                audioMogArgs += $"\"{file}\" ";
            }

            var processStartInfo = new ProcessStartInfo("AudioMog.exe", audioMogArgs)
            {
                RedirectStandardOutput = true
            };

            Console.WriteLine("Converting the game's audio assets to .wav (this can take a few minutes)...");

            Process? p = Process.Start(processStartInfo);

            // To prevent deadlock error when the StandardOutput buffer is at capacity.
            // https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.redirectstandardoutput?view=net-7.0
            p?.StandardOutput.ReadToEnd();
            p?.StandardOutput.Close();

            p?.WaitForExit();
            p?.Dispose();

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool ParseBundleFile(string bundleFilePath)
    {
        if (!Directory.Exists(TEMP_DIRECTORY_PATH))
        {
            Directory.CreateDirectory(TEMP_DIRECTORY_PATH);
        }

        try
        {
            var am = new AssetsManager();

            BundleFileInstance bundle = am.LoadBundleFile(bundleFilePath);
            var assetInst = am.LoadAssetsFileFromBundle(bundle, 0, true);

            var assets = assetInst.table.GetAssetsOfType(AssetClassID.TextAsset);

            foreach (var inf in assets)
            {
                var baseField = am.GetTypeInstance(assetInst, inf).GetBaseField();

                var name = baseField.Get(0).Value.AsString();
                var data = baseField.Get(1).Value.AsStringBytes();

                File.WriteAllBytes(Path.Join(TEMP_DIRECTORY_PATH, $"{name}.bytes"), data);
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}