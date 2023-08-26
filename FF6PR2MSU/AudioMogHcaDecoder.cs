// Code originally written by Yoraiz0r for his program AudioMog. This file is
// simply stripping down the process to the bare minimum needed for this specific
// purpose. Much like for Wav2Msu, this file contains no original code and it 
// wouldn't be right to take credit for any of this, so I won't.
//
// As per the requirements of the MIT license, the original copyright information
// will be included right after this blurb.
//
// Link to the Github repository for AudioMog:
// https://github.com/Yoraiz0r/AudioMog/
//
// ======================== ORIGINAL COPYRIGHT NOTICE ========================
//
// MIT License
//
// Copyright (c) 2021 Yoraiz0r
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using VGAudio.Containers.Wave;
using VGAudio.Formats;
using VGAudio.Containers.Hca;

namespace FF6PR2MSU
{
	public static class AudioMogHcaDecoder
    {
		private static readonly byte[] HcaEncryptionKey = new byte[0x100]
		{
			0x3A, 0x32, 0x32, 0x32, 0x03, 0x7E, 0x12, 0xF7, 0xB2, 0xE2, 0xA2, 0x67, 0x32, 0x32, 0x22, 0x32, // 00-0F
			0x32, 0x52, 0x16, 0x1B, 0x3C, 0xA1, 0x54, 0x7B, 0x1B, 0x97, 0xA6, 0x93, 0x1A, 0x4B, 0xAA, 0xA6, // 10-1F
			0x7A, 0x7B, 0x1B, 0x97, 0xA6, 0xF7, 0x02, 0xBB, 0xAA, 0xA6, 0xBB, 0xF7, 0x2A, 0x51, 0xBE, 0x03, // 20-2F
			0xF4, 0x2A, 0x51, 0xBE, 0x03, 0xF4, 0x2A, 0x51, 0xBE, 0x12, 0x06, 0x56, 0x27, 0x32, 0x32, 0x36, // 30-3F
			0x32, 0xB2, 0x1A, 0x3B, 0xBC, 0x91, 0xD4, 0x7B, 0x58, 0xFC, 0x0B, 0x55, 0x2A, 0x15, 0xBC, 0x40, // 40-4F
			0x92, 0x0B, 0x5B, 0x7C, 0x0A, 0x95, 0x12, 0x35, 0xB8, 0x63, 0xD2, 0x0B, 0x3B, 0xF0, 0xC7, 0x14, // 50-5F
			0x51, 0x5C, 0x94, 0x86, 0x94, 0x59, 0x5C, 0xFC, 0x1B, 0x17, 0x3A, 0x3F, 0x6B, 0x37, 0x32, 0x32, // 60-6F
			0x30, 0x32, 0x72, 0x7A, 0x13, 0xB7, 0x26, 0x60, 0x7A, 0x13, 0xB7, 0x26, 0x50, 0xBA, 0x13, 0xB4, // 70-7F
			0x2A, 0x50, 0xBA, 0x13, 0xB5, 0x2E, 0x40, 0xFA, 0x13, 0x95, 0xAE, 0x40, 0x38, 0x18, 0x9A, 0x92, // 80-8F
			0xB0, 0x38, 0x00, 0xFA, 0x12, 0xB1, 0x7E, 0x00, 0xDB, 0x96, 0xA1, 0x7C, 0x08, 0xDB, 0x9A, 0x91, // 90-9F
			0xBC, 0x08, 0xD8, 0x1A, 0x86, 0xE2, 0x70, 0x39, 0x1F, 0x86, 0xE0, 0x78, 0x7E, 0x03, 0xE7, 0x64, // A0-AF
			0x51, 0x9C, 0x8F, 0x34, 0x6F, 0x4E, 0x41, 0xFC, 0x0B, 0xD5, 0xAE, 0x41, 0xFC, 0x0B, 0xD5, 0xAE, // B0-BF
			0x41, 0xFC, 0x3B, 0x70, 0x71, 0x64, 0x33, 0x32, 0x12, 0x32, 0x32, 0x36, 0x70, 0x34, 0x2B, 0x56, // C0-CF
			0x22, 0x70, 0x3A, 0x13, 0xB7, 0x26, 0x60, 0xBA, 0x1B, 0x94, 0xAA, 0x40, 0x38, 0x00, 0xFA, 0xB2, // D0-DF
			0xE2, 0xA2, 0x67, 0x32, 0x32, 0x12, 0x32, 0xB2, 0x32, 0x32, 0x32, 0x32, 0x75, 0xA3, 0x26, 0x7B, // E0-EF
			0x83, 0x26, 0xF9, 0x83, 0x2E, 0xFF, 0xE3, 0x16, 0x7D, 0xC0, 0x1E, 0x63, 0x21, 0x07, 0xE3, 0x01, // F0-FF
		};

		public static readonly byte[] MabfString = { 109, 97, 98, 102 };

		public static int Magic_Mtrl = 1819440237;

		public static byte[] ConvertMabfToWav(byte[] fileBytes)
        {
            var innerFileStartOffset = -1;

			for (int i = 0; i < fileBytes.Length - MabfString.Length; i++)
			{
				if (MabfString.Length > (fileBytes.Length - i))
					continue;

				for (int j = 0; j < MabfString.Length; j++)
					if (fileBytes[i + j] != MabfString[j])
						continue;

				innerFileStartOffset = i;
				break;
			}
			
			if (innerFileStartOffset < 0)
				throw new Exception("");

            var stream = new MemoryStream(fileBytes);
            var reader = new BinaryReader(stream);

			var header = new AudioBinaryFileHeader(reader, innerFileStartOffset);

			var position = innerFileStartOffset + header.HeaderSize;
			uint sectionOffset = 0;

			for (int sectionIndex = 0; sectionIndex < header.SectionsCount; sectionIndex++)
			{
				reader.BaseStream.Position = position;

				if (reader.ReadUInt32() == Magic_Mtrl)
                {
					reader.BaseStream.Position = position + 0x08;
					sectionOffset = reader.ReadUInt32();
					break;
				}

				position += 16;
			}

			if (sectionOffset == 0)
				throw new Exception("Could not find offset position of the audio material's section.");

			var materialSectionOffset = innerFileStartOffset + sectionOffset;

			var positionOfOffsetFromMaterialSectionOffset = materialSectionOffset + 0x10;

			var entry = new MaterialEntry();
			entry.Read(reader, materialSectionOffset, positionOfOffsetFromMaterialSectionOffset);

			byte[] rawContentBytes = fileBytes[(int)entry.InnerStreamStartPosition..(int)(entry.InnerStreamStartPosition + entry.InnerStreamSize)];

			HcaReader hcareader = new HcaReader();
			BinaryReader fullBinaryReader = new BinaryReader(new MemoryStream(fileBytes));
			fullBinaryReader.BaseStream.Position = entry.ExtraDataOffset + 0x0d;
			var encryption = fullBinaryReader.ReadByte();

			if (encryption == 1)
				DecryptHCA(entry.ExtraDataId, entry.ExtraDataOffset, rawContentBytes, fullBinaryReader);

			AudioData audioData = hcareader.Read(rawContentBytes);
			return new WaveWriter().GetFile(audioData);
		}

		private static void DecryptHCA(ushort extraDataId, long extraDataOffset, byte[] hcaFileBytes, BinaryReader fullBinaryReader)
		{
			var key_start = extraDataId & 0xff;
			fullBinaryReader.BaseStream.Position = extraDataOffset + 0x02;
			var header_size = fullBinaryReader.ReadUInt16();
			var start = header_size;
			var offset = 0;

			for (int byteIndex = 0; byteIndex < hcaFileBytes.Length; byteIndex++)
			{
				var hcaByte = hcaFileBytes[byteIndex];

				bool canSwap = byteIndex >= start + offset;
				if (!canSwap)
					continue;

				var encryptionIndex = key_start + offset + byteIndex - start;
				var wrappedEncryptionIndex = encryptionIndex % HcaEncryptionKey.Length;
				hcaByte ^= HcaEncryptionKey[wrappedEncryptionIndex];

				hcaFileBytes[byteIndex] = hcaByte;
			}
		}

		public class AudioBinaryFileHeader
		{
			public byte SectionsCount;

			public int HeaderSize;

			public AudioBinaryFileHeader(BinaryReader reader, long offsetForFileStart)
			{
				reader.BaseStream.Position = offsetForFileStart + 0x08;
				SectionsCount = reader.ReadByte();
				byte descriptorLength = reader.ReadByte();

				int bytesNeededToPad = 16 - descriptorLength % 16;
				HeaderSize = 16 + descriptorLength + bytesNeededToPad;
			}
		}
	}

	public class MaterialEntry
	{
		public long ExtraDataOffset;
		public long InnerStreamStartPosition;
		public uint InnerStreamSize;
		public ushort ExtraDataId;

		public void Read(BinaryReader binaryReader, long mtrlSectionOffset, long pointerPosition)
		{
			binaryReader.BaseStream.Position = pointerPosition;
			uint localSectionOffset = binaryReader.ReadUInt32();
			long headerPosition = mtrlSectionOffset + localSectionOffset;

			binaryReader.BaseStream.Position = headerPosition + 0x18;
			uint streamSize = binaryReader.ReadUInt32();

			binaryReader.BaseStream.Position = headerPosition + 0x1c;
			ExtraDataId = binaryReader.ReadUInt16();
			ExtraDataOffset = headerPosition + 0x20;

			InnerStreamStartPosition = ExtraDataOffset + 0x10;

			binaryReader.BaseStream.Position = ExtraDataOffset + 0x16;
			var bigEndian1 = binaryReader.ReadByte();
			var bigEndian2 = binaryReader.ReadByte();
			ushort streamHeaderSize = (ushort)((bigEndian1 << 8) + bigEndian2);

			InnerStreamSize = streamHeaderSize + streamSize;
		}
	}
}
