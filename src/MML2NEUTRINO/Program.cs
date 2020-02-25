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
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                var xmlGen = new MusicXMLGenerator();
                var xml = xmlGen.GenerateFromElements(elements);

                string filename = Path.GetFullPath(outputMusicXML);
                xml.Save(filename);

                // Run NEUTRINO
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

                // Play wave file
                Console.WriteLine("Playing...");
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(outputWav);
                player.PlaySync();
            }
        }


    }

}
