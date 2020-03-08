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

        [Option('k', "keyshift", Required = false, HelpText = "MML のキーを変更します。1 で半音高くなります。省略時は 0 です。")]
        public int KeyShift { get; set; } = 0;

        [Option('p', "pitchshift", Required = false, HelpText = "音声出力時のピッチを変更します。+1.0 で半音高くなります。省略時は 0.0 です。")]
        public float PitchShift { get; set; } = 0f;
        [Option('f', "formantshift", Required = false, HelpText = "フォルマントを変更します。大きくする(1.05など)と声が子供っぽくなります。省略時は 1.0 です。")]
        public float FormantShift { get; set; } = 1.0f;

        [Option('t', "threads", Required = false, HelpText = "使用するスレッド数(省略時は最大スレッド数-1)。")]
        public int NumberOfThread { get; set; } = -1;
        [Option('s', "silent", Required = false, HelpText = "自動的にファイルを再生しません。")]
        public bool Silent { get; set; } = false;
        [Option('r', "reverse", Required = false, HelpText = ">, < の役割を逆にします。")]
        public bool Reverse { get; set; } = false;

        [Value(1, MetaName = "remaining")]
        public IEnumerable<string> Remaining { get; set; }
    }
}
