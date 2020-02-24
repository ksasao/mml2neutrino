using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MML2NEUTRINO
{
    public class MusicXMLGenerator
    {
        string[] t = new string[] { "-", "eighth", "quater", "-", "half", "-", "-", "-", "whole" };
        int[] durations = new int[] { 0, 8, 4, 3, 2, 0, 0, 0, 1 }; // L1=8.L2=4,L3=3,L4=2,L5=0,L6=0,L7=0,L8=1 
        int mDuration = 0;
        XElement xml = null;
        XElement part = null;
        public MusicXMLGenerator(string template)
        {
            xml = XElement.Load(template);
            part = (
                    from e in xml.Elements("part")
                    select e).Last();
        }
        public XElement GenerateFromElements(IElement[] elements)
        {
            int m = 2;
            XElement measure = new XElement("measure");
            measure.SetAttributeValue("number", m++);

            foreach (var e in elements)
            {
                Type t = e.GetType();
                if(t == typeof(Note))
                {
                    var n = CreateNote((Note)e);
                    measure.Add(n);
                    if (mDuration >= 8)
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
                    if (mDuration >= 8)
                    {
                        part.Add(measure);
                        measure = new XElement("measure");
                        measure.SetAttributeValue("number", m++);
                        mDuration = 0;
                    }
                }
            }
            if (mDuration > 0)
            {
                int last = 8 - mDuration;
                for (int i = 0; i < last; i++)
                {
                    measure.Add(CreateRest(new Rest { Length = 8, HasDot = false }));
                }
                part.Add(measure);
            }
            return xml;
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
