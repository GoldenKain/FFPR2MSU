using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using VGAudio.Containers.Wave;
using VGAudio.Formats;

namespace FF6PR2MSU;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Invalid command");
            Environment.Exit(0);
        }

        string languageCharacterCode = null;

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
            
            Console.WriteLine(); // skip lines so its cleaner
        } while (languageCharacterCode == null);

        Conversion.InitializeDictionary(languageCharacterCode);

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

                romFileName = name;
                break;
            }
            finally
            {
                Console.WriteLine();
            }
        } while (true);

        foreach (string file in args)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"{file} This file doesn't exist. Skipping.");
                continue;
            }
            
            if (!file.EndsWith(".wav"))
            {
                continue;
            }

            string path = Path.GetFullPath(file);
            string filename = Path.GetFileNameWithoutExtension(path);

            var match = Regex.Match(filename, "FF6_[0-9]+[abcde12]*(_[A-Z]{3})*");
            string msuName = null;
            
            if (!match.Success || (msuName = Conversion.GetName(match.Value)) == null)
            {
                Console.WriteLine($@"""{file}"" is not part of the msu patch or is named wrong. Skipping.");
                continue;
            }
            
            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            WaveReader reader = new WaveReader();
            IAudioFormat format = reader.ReadFormat(stream);

            bool looping = format.Looping;
            int loopStart = format.LoopStart;

            string param = @$"-o ""{$"{Environment.CurrentDirectory}/{romFileName}-{msuName}.pcm"}"" ";

            if (looping)
            {
                param += $"-l {loopStart} ";
            }

            param += @$"""{path}"" ";

            Process process = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                process = Process.Start("wav2msu.out", param);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process = Process.Start("wav2msu.exe", param);
            }
            else
            {
                Console.WriteLine("Unsupported OS.");
                Environment.Exit(0);
            }

            process.WaitForExit();

            switch (process.ExitCode)
            {
                case -1:
                    Console.WriteLine($@"""{file}"" could not be converted.");
                    break;
                case 0:
                    Console.WriteLine($@"""{file}"" has been converted successfully.");
                    break;
            }
        }
    }
}