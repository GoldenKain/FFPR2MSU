// This class is simply a conversion of the original author's code to C#
// slightly adjusted so its functionalities could be called as functions instead
// of a main and having it be ran as a program in and of itself. Thus, I do not
// think it right to give myself any credit for the code contained in this class
// since it's a shameless conversion from C to C# without any big difference
// (outside of removing some calls that I did not need for the purpose of this
// program).
//
// As such, and in accordance with the requirements of the MIT license, the 
// original copyright information and the license of the original code will be
// added below.
//
// ======================== ORIGINAL COPYRIGHT NOTICE ========================
//
// Copyright (c) 2011 Johannes Baiter <johannes.baiter@gmail.com>
// Based on C# code by Kawa <http://helmet.kafuka.org/thepile/Wav2msu>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

using System.Text;

namespace FF6PR2MSU;

/// <see>
/// https://github.com/jbaiter/wav2msu
/// </see>
public class Wav2Msu
{
    public static bool Convert(string inputFile, string outputFile, long loop_point)
    {
        FileStream? input = null;
        FileStream? output = null;

        try
        {
            try
            {
                input = System.IO.File.OpenRead(inputFile);

                if (input == null)
                    throw new Exception($"wav2msu: can't open {inputFile}");
            }
            catch
            {
                throw new Exception($"wav2msu: can't open {inputFile}");
            }

            try
            {
                output = System.IO.File.OpenWrite(outputFile);

                if (output == null)
                    throw new Exception();
            }
            catch
            {
                throw new Exception($"wav2msu: can't open {outputFile}\n");
            }

            if (Validate(input) == -1)
            {
                throw new Exception("wav2msu: Input WAV data did not validate.");
            }

            output.Write(Encoding.ASCII.GetBytes("MSU1"));

            // idk why we'd want to lose information to then write the same about of bytes anyways but... ok. :|
            loop_point = (int)loop_point;
            output.Write(BitConverter.GetBytes(loop_point));

            byte[] c = new byte[1];

            while (input.Read(c, 0, c.Length) > 0)
            {
                output.Write(c, 0, c.Length);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            output?.Close();
            input?.Close();
        }
    }

    private static int Validate(FileStream inputFile)
    {
        byte[] riff_header = new byte[4];

        // Check riff header and endianness ('RIFF' = little-endian)
        if (inputFile.Read(riff_header, 0, riff_header.Length) != 4 || BitConverter.ToInt32(riff_header) != 0x46464952)
        {
            Console.WriteLine("wav2msu: Incorrect header: Invalid format or endianness\n");
            Console.WriteLine("         Value was: 0x%x\n", BitConverter.ToInt32(riff_header));
            return -1;
        }

        inputFile.Seek(20, SeekOrigin.Begin);

        // Format has to be PCM (=1)
        byte[] format = new byte[2];

        if (inputFile.Read(format, 0, format.Length) != 2 || BitConverter.ToInt16(format) != 1)
        {
            Console.WriteLine("wav2msu: Not in PCM format! (format was: %d)\n", BitConverter.ToInt16(format));
            return -1;
        }

        // The data needs to be in stereo format
        byte[] channels = new byte[2];

        inputFile.Read(channels, 0, channels.Length);

        // Sample Rate has to be 44.1kHz
        byte[] sample_rate = new byte[4];
        inputFile.Read(sample_rate, 0, sample_rate.Length);

        inputFile.Seek(34, SeekOrigin.Begin);

        // We need 16bps
        byte[] bits_per_sample = new byte[2];
        inputFile.Read(bits_per_sample, 0, bits_per_sample.Length);

        if (BitConverter.ToInt16(channels) != 2 || BitConverter.ToInt32(sample_rate) != 44100 || BitConverter.ToInt16(bits_per_sample) != 16)
        {
            Console.WriteLine("wav2msu: Not in 16bit 44.1kHz stereo!\n");
            Console.WriteLine("         Got instead: %dbit, %dHz, %dch\n", bits_per_sample, sample_rate, channels);
            return -1;
        }

        // 'DATA'
        byte[] data_header = new byte[4];
        inputFile.Read(data_header, 0, data_header.Length);

        if (BitConverter.ToInt32(data_header) != 0x61746164)
        {
            Console.WriteLine("wav2msu: Sample data not where expected!\n");
            return -1;
        }
        
        byte[] data_size = new byte[4];
        inputFile.Read(data_size, 0, data_size.Length);

        return BitConverter.ToInt32(data_size);
    }
}