using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace MML2NEUTRINO
{
    class Program
    {
        const string cachePath = "./cache";
        static void Main(string[] args)
        {
            string outputMusicXML = "output.musicxml";
            string outputWav = "output.wav";

            if (args.Length > 0)
            {
                string mml = args[0];

                MMLParser parser = new MMLParser();

                IElement[] elements = null;
                try
                {
                    elements = parser.Parse(mml);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                var xmlGen = new MusicXMLGenerator();
                XElement xml = null;
                try
                {
                    xml = xmlGen.GenerateFromElements(elements);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                string filename = Path.GetFullPath(outputMusicXML);
                xml.Save(filename);
                string hash = GetHash(filename);

                if (!IsCached(hash))
                {
                    RunNeutrino(filename);
                    CreateCache(hash, outputWav);
                }
                else
                {
                    Console.WriteLine("Using cached file.");
                    outputWav = Path.Combine(cachePath, hash + ".wav");
                }

                // Play wave file
                Console.WriteLine("Playing...");
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(outputWav);
                player.PlaySync();
            }
        }

        private static void RunNeutrino(string filename)
        {
            string neutrino = @"input.bat";
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = neutrino;
            pi.Arguments = filename;
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            Process p = Process.Start(pi);
            while (!p.StandardOutput.EndOfStream)
            {
                Console.WriteLine(p.StandardOutput.ReadLine());
            }
        }

        static private bool IsCached(string hash)
        {
            string file = Path.Combine(cachePath, hash + ".wav");
            return File.Exists(file);
        }
        static private void CreateCache(string hash, string src)
        {
            Directory.CreateDirectory(cachePath);
            string dest = Path.Combine(cachePath, hash + ".wav");
            File.Copy(src, dest);
        }
        static private string GetHash(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
                byte[] bs = sha1.ComputeHash(fs);
                sha1.Clear();
                return BitConverter.ToString(bs).ToLower().Replace("-", "");
            }
        }
    }

}
