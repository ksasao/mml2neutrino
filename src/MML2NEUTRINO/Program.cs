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
            if (args.Length > 0)
            {
                if (args[0] == "-i")
                {
                    Translate(File.ReadAllText(args[1]);
                }
                else
                {
                    Translate(args[0]);
                }
            }
        }

        static void Translate(string mml)
        {
            NeutrinoController nc = new NeutrinoController();
            string outputMusicXML = "output.musicxml";

            IElement[] elements = null;
            try
            {
                elements = nc.Parse(mml);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            XElement xml = null;
            try
            {
                xml = nc.ConvertToMusicXML(elements);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string musicXmlFile = "";
            try
            {
                musicXmlFile = Path.GetFullPath(outputMusicXML);
                xml.Save(musicXmlFile);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string outputWav = nc.RunNeutrino(musicXmlFile);

            // Play wave file
            Console.WriteLine($"Playing {outputWav}");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(outputWav);
            player.PlaySync();
        }
    }
}
