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
            if (args.Length >= 0)
            {
                string mml = args[0];
                XElement xml = XElement.Load(@"template.xml");
                XElement part = (
                        from e in xml.Elements("part")
                        select e).Last();

                XElement notes = MMLParser(part, mml);
                string filename = Path.GetFullPath("output.musicxml");
                xml.Save(filename);

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
                Console.WriteLine("Playing...");
                System.Media.SoundPlayer player = new System.Media.SoundPlayer("output.wav");
                player.PlaySync();
            }
        }
        static XElement MMLParser(XElement part, string mml)
        {
            int m = 2;
            int octave = 4;
            int length = 4;
            int[] duration = new int[] { 0, 8, 4, 3, 2, 0, 0, 0, 1 }; // L1=8.L2=4,L3=3,L4=2,L5=0,L6=0,L7=0,L8=1 

            int mDuration = 0;
            XElement measure = new XElement("measure");
            measure.SetAttributeValue("number", m++);

            mml = mml.Replace(" ", "").ToUpper();
            for(int i=0; i < mml.Length; i++)
            {
                string c = mml[i].ToString();
                if(c == "O")
                {
                    i++;
                    (var j,var val) = GetNumber(i, mml);
                    if(i != j)
                    {
                        octave = val;
                        i = j;
                    }
                    i--;
                }
                else if(c == "L")
                {
                    i++;
                    (var j, var val) = GetNumber(i, mml);
                    if (i != j)
                    {
                        length = val;
                        i = j;
                    }
                    i--;
                }
                else if(c == ">")
                {
                    octave++;
                }else if(c == "<")
                {
                    octave--;
                }else if ("CDEFGAB".IndexOf(c) >= 0)
                {
                    int len = length;
                    i++;
                    int alter = 0;
                    if(mml[i].ToString()=="+" || mml[i].ToString() == "#")
                    {
                        alter = 1;
                        i++;
                    }
                    else if (mml[i].ToString() == "-")
                    {
                        alter = -1;
                        i++;
                    }

                    (var j, var val) = GetNumber(i, mml);
                    if (i != j)
                    {
                        len = val;
                    }
                    string lyric = mml[j].ToString();
                    while(j<mml.Length-1 && "CDEFGABOLTR<>+-#.".IndexOf(mml[j+1]) < 0)
                    {
                        j++;
                        lyric += mml[j];
                    }
                    if(mDuration >= 8)
                    {
                        part.Add(measure);
                        measure = new XElement("measure");
                        measure.SetAttributeValue("number", m++);
                        mDuration = 0;
                    }
                    measure.Add(CreateNote(c.ToString(), alter, octave, duration[len], lyric));
                    mDuration += duration[len];
                }
                else if (c == "R")
                {
                    int len = length;
                    i++;
                    (var j, var val) = GetNumber(i, mml);
                    if (i != j)
                    {
                        len = val;
                    }
                    if (mDuration >= 8)
                    {
                        part.Add(measure);
                        measure = new XElement("measure");
                        measure.SetAttributeValue("number", m++);
                        mDuration = 0;
                    }
                    measure.Add(CreateRest(duration[len]));
                    mDuration += duration[len];
                }
            }
            if (mDuration < 4)
            {
                for(int i=0; i < 4 - mDuration; i++)
                {
                    measure.Add(CreateRest(1));
                }
            }
            part.Add(measure);

            return part;
        }
        static (int prt,int val) GetNumber(int ptr, string mml)
        {
            int val = 0;
            while (ptr < mml.Length)
            {
                int n = "0123456789".IndexOf(mml[ptr]);
                if (n < 0)
                {
                    break;
                }
                val = val * 100 + n;
                ptr++;
            }
            return (ptr,val);
        }
        static XElement CreateNote(string step, int alter, int octave, int duration, string lyric)
        {
            string[] t = new string[]{ "-", "eighth", "quater", "-", "half", "-", "-", "-", "whole" };

            XElement lyricElement = new XElement("lyric",
                new XElement("sylabic", "single"),
                new XElement("text", lyric));
            lyricElement.SetAttributeValue("number", 1);

            XElement note = null;
            switch (alter)
            {
                case 0:
                    note = new XElement("note",
                        new XElement("pitch",
                            new XElement("step", step),
                            new XElement("octave", octave)
                            ),
                        new XElement("duration", duration),
                        new XElement("type", t[duration]),
                        lyricElement);
                    break;
                case -1:
                    note = new XElement("note",
                        new XElement("pitch",
                            new XElement("step", step),
                            new XElement("alter", -1),
                            new XElement("octave", octave)
                            ),
                        new XElement("duration", duration),
                        new XElement("type", t[duration]),
                        new XElement("accidental", "flat"),
                        lyricElement);
                    break;
                case 1:
                    note = new XElement("note",
                        new XElement("pitch",
                            new XElement("step", step),
                            new XElement("alter", 1),
                            new XElement("octave", octave)
                            ),
                        new XElement("duration", duration),
                        new XElement("type", t[duration]),
                        new XElement("accidental", "sharp"),
                        lyricElement);
                    break;
                default:
                    break;
            }

            return note;
        }
        static XElement CreateRest(int duration)
        {
            string[] t = new string[] { "-", "quater", "half", "-", "whole" };

            XElement note = new XElement("note",
                new XElement("rest"),
                new XElement("duration", duration),
                new XElement("type", t[duration])
            );

            return note;
        }

    }

}
