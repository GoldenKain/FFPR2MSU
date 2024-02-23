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
    public static bool Convert(byte[] inputData, string outputFile, long loop_point)
    {
        FileStream output = null;

        try
        {
            try
            {
                output = File.OpenWrite(outputFile);

                if (output == null)
                    throw new Exception();
            }
            catch
            {
                throw new Exception($"wav2msu: can't open {outputFile}\n");
            }

            int dataSize = Validate(inputData);

            if (dataSize == -1)
            {
                throw new Exception("wav2msu: Input WAV data did not validate.");
            }

            output.Write(Encoding.ASCII.GetBytes("MSU1"));

            // idk why we'd want to lose information to then write the same about of bytes anyways but... ok. :|
            loop_point = (int)loop_point;
            output.Write(BitConverter.GetBytes(loop_point));

            output.Write(inputData[(inputData.Length - dataSize)..]);

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
            output?.Dispose();
        }
    }

    private static int Validate(byte[] inputData)
    {
        int offset = 0;
        byte[] riff_header = new byte[4];

        // Check riff header and endianness ('RIFF' = little-endian)

        if (Read(inputData, ref riff_header, ref offset, riff_header.Length) != 4 || BitConverter.ToInt32(riff_header) != 0x46464952)
        {
            Console.WriteLine("wav2msu: Incorrect header: Invalid format or endianness\n");
            Console.WriteLine($"         Value was: 0x{BitConverter.ToInt32(riff_header):x}\n");
            return -1;
        }

        offset = 20;

        // Format has to be PCM (=1)
        byte[] format = new byte[2];

        if (Read(inputData, ref format, ref offset, format.Length) != 2 || BitConverter.ToInt16(format) != 1)
        {
            Console.WriteLine($"wav2msu: Not in PCM format! (format was: {BitConverter.ToInt16(format):d})\n");
            return -1;
        }

        // The data needs to be in stereo format
        byte[] channels = new byte[2];

        Read(inputData, ref channels, ref offset, channels.Length);

        // Sample Rate has to be 44.1kHz
        byte[] sample_rate = new byte[4];
        Read(inputData, ref sample_rate, ref offset, sample_rate.Length);

        offset = 34;

        // We need 16bps
        byte[] bits_per_sample = new byte[2];
        Read(inputData, ref bits_per_sample, ref offset, bits_per_sample.Length);

        if (BitConverter.ToInt16(channels) != 2 || BitConverter.ToInt32(sample_rate) != 44100 || BitConverter.ToInt16(bits_per_sample) != 16)
        {
            Console.WriteLine("wav2msu: Not in 16bit 44.1kHz stereo!\n");
            Console.WriteLine($"         Got instead: {BitConverter.ToInt16(bits_per_sample):d}bit, {BitConverter.ToInt32(sample_rate):d}Hz, {BitConverter.ToInt16(channels):d}ch\n");
            return -1;
        }

        // 'DATA'
        byte[] data_header = new byte[4];
        Read(inputData, ref data_header, ref offset, data_header.Length);

        if (BitConverter.ToInt32(data_header) != 0x61746164)
        {
            Console.WriteLine("wav2msu: Sample data not where expected!\n");
            return -1;
        }
        
        byte[] data_size = new byte[4];
        Read(inputData, ref data_size, ref offset, data_size.Length);

        return BitConverter.ToInt32(data_size);
    }

    /**
     * <summary>"Reads" the requested number of bytes from the input (if possible) and copies the results in the output. Made to closely match the signature of FileStream.Read().</summary>
     * <param name="input">Input array that is to be read</param>
     * <param name="output">Reference of the output array. The read data will be copied in that array, thus, it must already have been initialized</param>
     * <param name="offset">Point at which the input array with be read (a.k.a. the offset)</param>
     * <param name="length">Number of bytes to be read from the input array</param>
     * <returns>The number of bytes that <i>could</i> be read from the input byte array</returns>
     **/
    private static int Read(byte[] input, ref byte[] output, ref int offset, int length)
    {
        if (input.Length < offset)
            return -1;

        if (input.Length < offset + length)
            length -= offset + length - input.Length;

        Array.Copy(input, offset, output, 0, length);

        offset += length;
        return length;
    }
}