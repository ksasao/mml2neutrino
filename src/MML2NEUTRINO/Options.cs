using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    class Options
    {
        [Option('i', "input", Required = false, HelpText = "MMLが記載されたテキストファイル(UTF-8)")]
        public string InputFileName { get; set; }
        [Option('o', "output", Required = false, HelpText = "出力ファイル名(*.wav)")]
        public string OutputFileName { get; set; } = "output.wav";
        [Option('m', "model", Required = false, HelpText = "モデル名(省略時は KIRITAN)")]
        public string Model { get; set; } = "KIRITAN";

        [Option('p', "pitchshift", Required = false, HelpText = "ピッチシフト(WORLD)")]
        public float PitchShift { get; set; } = 1.0f;
        [Option('f', "formantshift", Required = false, HelpText = "フォルマントシフト(WORLD)")]
        public float FormantShift { get; set; } = 1.0f;

        [Option('t', "threads", Required = false, HelpText = "使用するスレッド数(省略時は最大スレッド数-1)")]
        public int NumberOfThread { get; set; } = -1;
        [Option('s', "silent", Required = false, HelpText = "自動的にファイルを再生しません")]
        public bool Silent { get; set; } = false;

        [Value(1, MetaName = "remaining")]
        public IEnumerable<string> Remaining { get; set; }
    }
}
