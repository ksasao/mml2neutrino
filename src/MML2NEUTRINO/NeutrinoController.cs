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
    public class NeutrinoController
    {
        public string CachePath { get; set; } = "./cache";
        public NeutrinoController()
        {

        }
        public IElement[] Parse(string mml)
        {
            MMLParser parser = new MMLParser();
            return parser.Parse(mml);
        }
        public XElement ConvertToMusicXML(IElement[] elements)
        {
            var xmlGen = new MusicXMLGenerator();
            return xmlGen.GenerateFromElements(elements);
        }
            
        public bool IsCached(string hash)
        {
            string file = Path.Combine(CachePath, hash + ".wav");
            return File.Exists(file);
        }
        public void CreateCache(string hash, string src)
        {
            Directory.CreateDirectory(CachePath);
            string dest = Path.Combine(CachePath, hash + ".wav");
            File.Copy(src, dest);
        }
        public string GetHash(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
                byte[] bs = sha1.ComputeHash(fs);
                sha1.Clear();
                return BitConverter.ToString(bs).ToLower().Replace("-", "");
            }
        }
        public string RunNeutrino(string musicXmlFile)
        {
            string outputWav = Path.Combine(Path.GetDirectoryName(musicXmlFile), Path.GetFileNameWithoutExtension(musicXmlFile) + ".wav");
            string hash = GetHash(musicXmlFile);
            if (!IsCached(hash))
            {
                RunBatch(musicXmlFile);
                CreateCache(hash, outputWav);
            }
            else
            {
                Console.WriteLine("Using cached file.");
            }
            return Path.Combine(CachePath, hash + ".wav");
        }

        private void RunBatch(string musicXmlFile)
        {
            string neutrino = @"input.bat";
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = neutrino;
            pi.Arguments = musicXmlFile;
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;

            Process p = Process.Start(pi);
            while (!p.StandardOutput.EndOfStream)
            {
                Console.WriteLine(p.StandardOutput.ReadLine());
            }
        }
    }
}
