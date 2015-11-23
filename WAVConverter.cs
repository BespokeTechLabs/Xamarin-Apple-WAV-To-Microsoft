// WAVConverter class
// Written by Lewis Smallwood
// November 2015
//
// This class converts a WAV with a long header of 4096
// bytes to WAV with a standard header length of 44 bytes.

using System;
using System.IO;

namespace Enrichments.iOS
{
    // Convert WAV with long headers to WAV short headers.
    public class WAVConverter
    {
        private FileStream fs = null;
        private byte[] buffer = null;
        private int bytesRead = 0;

        private string latestConvertedPath = "";

        // The first 4096 bytes are Headers in an Apple encoded file.
        // Microsoft only accept WAVs with Headers of a 44 byte lenght.

        // For this reason, the RAW audio data starts at the position 4060
        // Which is Total File Lenght - Apple Header Length
        private int appleHeaderLength = 4096;
        private int microsoftAcceptedHeaderLength = 44;

        // This takes in a WAV file with a 4096 byte header.
        public WAVConverter(string wavLocation)
        {
            using (fs = new FileStream(wavLocation, FileMode.Open, FileAccess.Read))
            {
                // Extract new header values from the Apple Headers
                BinaryReader br = new BinaryReader(fs);
                var length = (int)fs.Length - (appleHeaderLength - microsoftAcceptedHeaderLength);
                fs.Position = 22;
                var channels = br.ReadInt16();
                fs.Position = 24;
                var samplerate = br.ReadInt32();
                fs.Position = 34;

                var BitsPerSample = br.ReadInt16();
                var DataLength = (int)fs.Length - appleHeaderLength;

                // Create a duplicate file. e.g. filename.wav2 without the stupidly long headers.
                latestConvertedPath = wavLocation + "2";
                FileStream fileStream = new FileStream(latestConvertedPath, FileMode.Create, FileAccess.Write );
                BinaryWriter bw = new BinaryWriter(fileStream);

                bw.Write(new char[4] { 'R', 'I', 'F', 'F' });
                bw.Write(length);
                bw.Write(new char[8] {'W','A','V','E','f','m','t',' '});
                bw.Write((int)16);
                bw.Write((short)1);
                bw.Write(channels);
                bw.Write(samplerate);
                bw.Write((int)(samplerate * ((BitsPerSample * channels) / 8)));
                bw.Write((short )((BitsPerSample * channels) / 8));
                bw.Write(new byte[2] { 0x10, 0x00 });
                bw.Write(new char[4] {'d','a','t','a'});
                bw.Write(DataLength);

                // Read the bytes from the Apple recorded file.
                int i = 0;
                fs.Position = 0;
                buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                {
                    // For every chunk after the Apple header
                    if (i >= 4) {
                        // Passed 4096 bytes so write the RAW Audio Data to the new file.
                        bw.Write(buffer);
                    }

                    i++;
                }

                bw.Close(); // Finished loading the file into the new file.
                fileStream.Close(); // Forgot to close the file stream.
            }
        }

        public string LastConvertedPath() {
            return latestConvertedPath;
        }
    }
}

