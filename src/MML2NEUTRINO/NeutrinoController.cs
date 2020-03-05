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
        private int processors;

        /// <summary>
        /// Cache path
        /// </summary>
        public string CachePath { get; set; } = "./cache";
        /// <summary>
        /// Number of threads for running Neutrino. Default value is (logical processors - 1)
        /// </summary>
        public int NumberOfThreads { get { return processors; } set { SetProcessors(value); } }
        public NeutrinoController()
        {
            Initialize();
        }
        private void Initialize()
        {
            int c = Environment.ProcessorCount;
            processors = c > 1 ? c-1 : c;
        }
        private void SetProcessors(int value)
        {
            int maxProcessors = Environment.ProcessorCount;
            if(value<0 || value > maxProcessors)
            {
                throw new FormatException($"プロセッサ数には 0～{maxProcessors}を指定してください。");
            }
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
                string baseName = Path.GetFileNameWithoutExtension(musicXmlFile);
                string outputPath = Path.GetDirectoryName(musicXmlFile);
                int numberOfThreads = NumberOfThreads;
                string modelDirectory = "KIRITAN";
                float pitchShift = 1.0f;
                float formantShift = 1.0f;

                Run(@"bin\musicXMLtoLabel.exe",
                    $"{musicXmlFile} score\\label\\full\\{baseName}.lab score\\label\\mono\\{baseName}.lab");
                Run(@"bin\NEUTRINO.exe",
                    $"score\\label\\full\\{baseName}.lab score\\label\\timing\\{baseName}.lab output\\{baseName}.f0 output\\{baseName}.mgc output\\{baseName}.bap model\\{modelDirectory}\\ -n {numberOfThreads} -t");
                Run(@"bin\WORLD.exe",
                    $"output\\{baseName}.f0 output\\{baseName}.mgc output\\{baseName}.bap -f {pitchShift} -m {formantShift} -o {outputPath}\\{baseName}.wav -n {numberOfThreads} -t");

                CreateCache(hash, outputWav);
            }
            else
            {
                Console.WriteLine("Using cached file.");
            }
            return Path.Combine(CachePath, hash + ".wav");
        }

        private void Run(string filename, string args)
        {
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = filename;
            pi.Arguments = args;
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
