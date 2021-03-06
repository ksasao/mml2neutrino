﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    public class MMLParser
    {
        // params
        const int minTempo = 40;
        const int maxTempo = 300;
        const int minOctave = 1;
        const int maxOctave = 8;

        // for parse
        List<IElement> list = new List<IElement>();
        int length = 0;
        int octave = 0;

        bool reverse = false;

        public MMLParser() { 
        }
        private void InitializeParameter()
        {
            list.Clear();
            length = 4;
            octave = 4;
        }

        public IElement[] Parse(string mml)
        {
            return Parse(mml, false);
        }
        public IElement[] Parse(string mml, bool octaveReverse)
        {
            reverse = octaveReverse;
            InitializeParameter();
            mml = Regex.Replace(mml, @"[\t\r\n\s]", "").ToUpper();
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
                n = ParseAsTempo(mml, p);
                if (n > 0) { p += n; continue; }

                // parse error
                throw new FormatException(mml.Substring(0, p) + " の後が解釈できませんでした。");
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
                string m = mml + "???";
                if (m[p + n].ToString() == "+" || m[p + n].ToString() == "#")
                {
                    alter = 1;
                    n++;
                }
                else if (m[p + n].ToString() == "-")
                {
                    alter = -1;
                    n++;
                }
                int j = ParseAsNumber(m, p + n);
                if (j > 0)
                {
                    len = Convert.ToInt32(m.Substring(p + n, j));
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
                    throw new FormatException(m.Substring(0,p+n) + " の後に歌詞がありません。");
                }
                list.Add(new Note {
                    Octave = octave,
                    Step = step,
                    Alter = alter,
                    Length = len,
                    HasDot = hasDot,
                    Lyric = lyric,
                    MML = mml.Substring(p,n)});
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
                    HasDot = hasDot,
                    MML = mml.Substring(p, n)
                });
            }
            return n;
        }
        private int ParseAsTempo(string mml, int p)
        {
            int n = 0;
            string c = mml[p].ToString();
            if (c == "T")
            {
                n++;
                int j = ParseAsNumber(mml, p + n);
                if (j > 0)
                {
                    int tempo = Convert.ToInt32(mml.Substring(p + n, j));
                    n += j;
                    if (tempo < minTempo || tempo > maxTempo)
                    {
                        throw new FormatException(mml.Substring(0, p + n) + $" テンポの範囲は{minTempo}～{maxTempo}です。");
                    }
                    list.Add(new Tempo {
                        Value = tempo,
                        MML = mml.Substring(p, n)
                    });
                }
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
                    CheckOctave(mml, p + n);
                    list.Add(new Octave
                    {
                        Value = octave,
                        MML = mml.Substring(p, n)
                    });
                }
                else
                {
                    throw new FormatException(mml.Substring(0, p + n) + " Oの後には数字が必要です");
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
                octave = octave + (reverse ? -1 : 1);
            }
            else if (c == "<")
            {
                n++;
                octave = octave - (reverse ? -1 : 1);
            }
            if(n != 0)
            {
                CheckOctave(mml, p + n);
                list.Add(new Octave
                {
                    Value = octave,
                    MML = mml.Substring(p, n)
                });
            }
            return n;
        }
        private void CheckOctave(string mml, int pn)
        {
            if (octave < minOctave || octave > maxOctave)
            {
                throw new FormatException(mml.Substring(0, pn) + $" オクターブの範囲は{minOctave}～{maxOctave}です。");
            }
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
                    list.Add(new Length
                    {
                        Value = length,
                        MML = mml.Substring(p, n)
                    });
                }
                else
                {
                    throw new FormatException(mml.Substring(0, p + n) + " Lの後には数字が必要です");
                }

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
