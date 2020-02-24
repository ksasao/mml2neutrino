using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    public class Note : IElement
    {
        /// <summary>
        /// C, D, E, F, C+ (sharp), C# (sharp), D- (flat)
        /// </summary>
        public string Step { get; set; }
        /// <summary>
        /// Octave
        /// </summary>
        public int Octave { get; set; }
        /// <summary>
        /// whether note has a dot (such as 'dotted quarter note')
        /// </summary>
        public bool HasDot { get; set; }
        /// <summary>
        /// L2C -> Length = 2 / D16 -> Length = 16 / C4. -> Length = 4 (dot is ignored)
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// Support Japanese Hiragana/Katakana letters
        /// </summary>
        public string Lylic { get; set; }
    }
}
