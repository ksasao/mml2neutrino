﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MML2NEUTRINO
{
    public class MusicXMLGenerator
    {
        // 16分音符と8分音符3連符をサポート → 全音符 = 48
        string[] t = new string[] { "-",
            "16th","16th","16th","eighth", "[5]","eighth","[7]","quater","eighth","[10]",
            "[11]","quater","[13]","[14]","[15]","half","[17]","quater","[19]","[20]",
            "[21]","[22]","[23]","half", "[25]","[26]","[27]","[28]","[29]","[30]",
            "[31]","[32]","[33]","[34]", "[35]","half","[37]","[38]","[39]","[40]",
            "[41]","[42]","[43]","[44]", "[45]","[46]","[47]","whole" };
        int[] durations = new int[] { 0,
            48,24,16,12, 0,8,0,6, 0,0,0,4, 0,0,0,3,
            0,0,0,0, 0,0,0,2, 0,0,0,0, 0,0,0,0,
            0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,1 }; // L1=48.L2=24,L3=16,L4=12,L6=8,L8=6,L12=4,L16=3.L24=2,L48=1 
        int mDuration = 0;
        const int maxDuration = 48;
        XElement xml = null;
        XElement part = null;

        public MusicXMLGenerator()
        {
        }

        private void Initialize()
        {
            // Read template from resource
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string[] resnames = myAssembly.GetManifestResourceNames();
            using (StreamReader sr = new StreamReader(myAssembly.GetManifestResourceStream("MML2NEUTRINO.template.xml"), Encoding.UTF8))
            {
                xml = XElement.Load(sr);
            }
            part = (
                    from e in xml.Elements("part")
                    select e).Last();
        }

        public XElement GenerateFromElements(IElement[] elements)
        {
            Initialize();

            int m = 1;
            XElement measure = new XElement("measure");
            measure.Add(Attribute(maxDuration/4, 0, 4, 4, "G", 2));
            measure.SetAttributeValue("number", m++);

            for(int i=0; i< elements.Length; i++)
            {
                var e = elements[i];
                Type t = e.GetType();
                if(t == typeof(Note))
                {
                    var n = CreateNote((Note)e);
                    measure.Add(n);
                    if (mDuration == maxDuration)
                    {
                        part.Add(measure);
                        measure = new XElement("measure");
                        measure.SetAttributeValue("number", m++);
                        mDuration = 0;
                    }else if (mDuration > maxDuration)
                    {
                        string mml = CreateMML(elements, i);
                        throw new FormatException($"{mml} 小節をまたぐ音符が指定されています。(over: {mDuration - maxDuration}/{maxDuration})");
                    }
                }
                else if(t == typeof(Rest))
                {
                    var r = CreateRest((Rest)e);
                    measure.Add(r);
                    if (mDuration == maxDuration)
                    {
                        part.Add(measure);
                        measure = new XElement("measure");
                        measure.SetAttributeValue("number", m++);
                        mDuration = 0;
                    }
                    else if (mDuration > maxDuration)
                    {
                        string mml = CreateMML(elements, i);
                        throw new FormatException($"{mml} 小節をまたぐ休符が指定されています。(over: {mDuration - maxDuration}/{maxDuration})");
                    }
                }
                else if(t == typeof(Tempo))
                {
                    measure.Add(CreateTempo(((Tempo)e).Value));
                }
            }
            if (mDuration > 0)
            {
                int last = maxDuration - mDuration;
                for (int i = 0; i < last; i++)
                {
                    measure.Add(CreateRest(new Rest { Length = 48, HasDot = false }));
                }
                part.Add(measure);
            }
            return xml;
        }

        private string CreateMML(IElement[] elements, int i)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < elements.Length && j <= i; j++)
            {
                sb.Append(elements[j].MML);
            }
            string mml = sb.ToString();
            return mml;
        }

        public XElement CreateTempo(int tempo)
        {
            XElement sound = new XElement("sound");
            sound.SetAttributeValue("tempo", tempo);
            XElement t = new XElement("direction",
                new XElement("direction-type",
                    new XElement("metronome",
                        new XElement("beat-unit", "quater"),
                        new XElement("per-minute", tempo))),
                sound);
            return t;
        }
        private XElement Attribute(int divisions, int fifths, int beats, int beatType, string sign, int line)
        {
            XElement attribute = new XElement("attributes",
                new XElement("divisions", divisions),
                new XElement("key",
                    new XElement("fifths", fifths)),
                new XElement("time",
                    new XElement("beats", beats),
                    new XElement("beat-type", beatType)),
                new XElement("clef",
                    new XElement("sign", sign),
                    new XElement("line", line))
                );
            return attribute;
        }

        private XElement CreateNote(Note n)
        {
            XElement lyricElement = new XElement("lyric",
                new XElement("sylabic", "single"),
                new XElement("text", n.Lyric));
            lyricElement.SetAttributeValue("number", 1);

            XElement note = null;
            if(n.Length<0 || n.Length>= durations.Length || durations[n.Length] == 0)
            {
                throw new FormatException($"長さ {n.Length} は指定できません。");
            }
            int duration = durations[n.Length];
            if (n.HasDot)
            {
                duration = duration * 3 / 2;
            }
            mDuration += duration;

            var dot = n.HasDot ? new XElement("dot") : null;
            var alter = n.Alter == -1 ? new XElement("alter", -1) : n.Alter == 1 ? new XElement("alter", 1) : null;
            note = new XElement("note",
                new XElement("pitch",
                    new XElement("step", n.Step),
                    alter,
                    new XElement("octave", n.Octave)
                    ),
                new XElement("duration", duration),
                new XElement("type", t[duration]),
                dot,
                lyricElement);
            return note;
        }
        private XElement CreateRest(Rest r)
        {
            int duration = durations[r.Length];
            if (r.HasDot)
            {
                duration = duration * 3 / 2;
            }
            mDuration += duration;

            XElement note = new XElement("note",
                new XElement("rest"),
                new XElement("duration", duration),
                new XElement("type", t[duration])
            );
            return note;
        }
    }
}
