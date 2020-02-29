using System;
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
            "16th","16th","16th","eighth", "-","eighth","-","-","eighth","-",
            "-","quater","-","-","-","quater","-","quater","-","-",
            "-","-","-","half", "-","-","-","-","-","-",
            "-","-","-","-", "-","half","-","-","-","-",
            "-","-","-","-", "-","-","-","whole" };
        int[] durations = new int[] { 0,
            48,24,16,12, 0,0,0,6, 0,0,0,4, 0,0,0,3,
            0,0,0,0, 0,0,0,2, 0,0,0,0, 0,0,0,0,
            0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,1 }; // L1=48.L2=24,L3=16,L4=12,L8=6,L12=4,L16=3.L24=2,L48=1 
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

            foreach (var e in elements)
            {
                Type t = e.GetType();
                if(t == typeof(Note))
                {
                    var n = CreateNote((Note)e);
                    measure.Add(n);
                    if (mDuration >= maxDuration)
                    {
                        part.Add(measure);
                        measure = new XElement("measure");
                        measure.SetAttributeValue("number", m++);
                        mDuration = 0;
                    }
                }
                else if(t == typeof(Rest))
                {
                    var r = CreateRest((Rest)e);
                    measure.Add(r);
                    if (mDuration >= maxDuration)
                    {
                        part.Add(measure);
                        measure = new XElement("measure");
                        measure.SetAttributeValue("number", m++);
                        mDuration = 0;
                    }
                }else if(t == typeof(Tempo))
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
            int duration = durations[n.Length];
            if (n.HasDot)
            {
                duration = duration * 3 / 2;
            }
            mDuration += duration;

            var dot = n.HasDot ? new XElement("dot") : null;
            switch (n.Alter)
            {
                case 0:
                    note = new XElement("note",
                        new XElement("pitch",
                            new XElement("step", n.Step),
                            new XElement("octave", n.Octave)
                            ),
                        new XElement("duration", duration),
                        new XElement("type", t[duration]),
                        dot,
                        lyricElement);
                    break;
                case -1:
                    note = new XElement("note",
                        new XElement("pitch",
                            new XElement("step", n.Step),
                            new XElement("alter", -1),
                            new XElement("octave", n.Octave)
                            ),
                        new XElement("duration", duration),
                        new XElement("type", t[duration]),
                        new XElement("accidental", "flat"),
                        dot,
                        lyricElement);
                    break;
                case 1:
                    note = new XElement("note",
                        new XElement("pitch",
                            new XElement("step", n.Step),
                            new XElement("alter", 1),
                            new XElement("octave", n.Octave)
                            ),
                        new XElement("duration", duration),
                        new XElement("type", t[duration]),
                        new XElement("accidental", "sharp"),
                        dot,
                        lyricElement);
                    break;
                default:
                    break;
            }

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
