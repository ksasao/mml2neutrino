using CommandLine;
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
            NeutrinoController nc = new NeutrinoController();

            string outputMusicXML = "output.musicxml";
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opt =>
                {

                    nc.FormantShift = opt.FormantShift;
                    nc.PitchShift = opt.PitchShift;
                    nc.Model = opt.Model;
                    if (opt.NumberOfThread != -1)
                    {
                        nc.NumberOfThreads = opt.NumberOfThread;
                    }
                    nc.OutputFileName = opt.OutputFileName;

                    // set MML
                    string mml = "";
                    if (opt.Remaining != null && opt.Remaining.ToArray().Length > 0)
                    {
                        mml = string.Join(" ", opt.Remaining.ToArray());
                    }
                    if (opt.InputFileName != null)
                    {
                        mml = File.ReadAllText(opt.InputFileName);
                    }
                    if(mml == "")
                    {
                        Console.WriteLine("ヘルプを表示するには --help を入力してください");
                        return;
                    }

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
                    if (!opt.Silent)
                    {
                        Console.WriteLine($"Playing...");
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(outputWav);
                        player.PlaySync();
                    }
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }


    }

}
