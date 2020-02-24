using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    public class MMLParser
    {
        // for parse
        List<IElement> list = new List<IElement>();
        int length = 0;
        int octave = 0;
        int tempo = 0;
        public MMLParser() { 
        }
        private void InitializeParameter()
        {
            list.Clear();
            length = 4;
            octave = 4;
            tempo = 80;
        }
        public IElement[] Parse(string mml)
        {
            InitializeParameter();
            mml = mml.Replace(" ", "").ToUpper();
            int p = 0;
            while(p < mml.Length)
            {
                int n;
                n = ParseAsNote(mml, p);
                if (n > 0) { p += n; continue; }
                n = ParseAsRest(mml, p);
                if (n > 0) { p += n; continue; }
                n = ParseAsOctave(mml, p);
                if (n > 0) { p += n; continue; }
                n = ParseAsOctaveShift(mml, p);
                if (n > 0) { p += n; continue; }
                n = ParseAsLength(mml, p);
                if (n > 0) { p += n; continue; }
            }
            return list.ToArray();
        }
        private int ParseAsNote(string mml, int p)
        {
            int n = 0;
            string c = mml[p].ToString();
            string step = "";
            int alter = 0;
            int len = length;
            if ("CDEFGAB".IndexOf(c) >= 0)
            {
                n++;
                step = c;
                if (mml[p + n].ToString() == "+" || mml[p + n].ToString() == "#")
                {
                    alter = 1;
                    n++;
                }
                else if (mml[p + n].ToString() == "-")
                {
                    alter = -1;
                    n++;
                }
                int j = ParseAsNumber(mml, p + n);
                if (j > 0)
                {
                    len = Convert.ToInt32(mml.Substring(p + n, j));
                    n += j;
                }
                bool hasDot = false;
                if(p + n < mml.Length && mml[p+n] == '.')
                {
                    n++;
                    hasDot = true;
                }
                string lyric = "";
                while (p + n < mml.Length && "CDEFGABOLTR<>+-#.".IndexOf(mml[p + n]) < 0)
                {
                    lyric += mml[p+n];
                    n++;
                }
                if (lyric.Length == 0)
                {
                    throw new FormatException(mml.Substring(0,p+n) + " の後に歌詞がありません。");
                }
                list.Add(new Note {
                    Octave = octave,
                    Step = step,
                    Alter = alter,
                    Length = len,
                    HasDot = hasDot,
                    Lyric = lyric});
            }
            return n;
        }
        private int ParseAsRest(string mml, int p)
        {
            int n = 0;
            string c = mml[p].ToString();
            int len = length;
            if (c == "R")
            {
                n++;
                int j = ParseAsNumber(mml, p + n);
                if (j > 0)
                {
                    len = Convert.ToInt32(mml.Substring(p + n, j));
                    n += j;
                }
                bool hasDot = false;
                if (p + n < mml.Length && mml[p + n] == '.')
                {
                    n++;
                    hasDot = true;
                }
                list.Add(new Rest
                {
                    Length = len,
                    HasDot = hasDot
                });
            }
            return n;
        }
        private int ParseAsOctave(string mml, int p)
        {
            int n = 0;
            string c = mml[p].ToString();
            if (c == "O")
            {
                n++;
                int j = ParseAsNumber(mml, p + n);
                if (j > 0)
                {
                    octave = Convert.ToInt32(mml.Substring(p + n, j));
                    n += j;
                    if(octave < 1 || octave > 8)
                    {
                        throw new FormatException(mml.Substring(0, p + n) + " オクターブの範囲は1～8です。");
                    }
                }
            }
            return n;
        }
        private int ParseAsLength(string mml, int p)
        {
            int n = 0;
            string c = mml[p].ToString();
            if (c == "L")
            {
                n++;
                int j = ParseAsNumber(mml, p + n);
                if (j > 0)
                {
                    length = Convert.ToInt32(mml.Substring(p + n, j));
                    n += j;
                }
            }
            return n;
        }
        private int ParseAsOctaveShift(string mml, int p)
        {
            int n = 0;
            string c = mml[p].ToString();
            if (c == ">")
            {
                n++;
                octave++;
            }else if (c == "<")
            {
                n++;
                octave--;
            }
            if (octave < 1 || octave > 8)
            {
                throw new FormatException(mml.Substring(0, p + n) + " オクターブの範囲は1～8です。");
            }
            return n;
        }
        private int ParseAsNumber(string mml, int p)
        {
            int n = 0;
            while (p + n < mml.Length)
            {
                int num = "0123456789".IndexOf(mml[p + n]);
                if (num < 0)
                {
                    break;
                }
                n++;
            }
            return n;
        }
    }
}
