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
        /// C, D, E, F, E, ...
        /// </summary>
        public string Step { get; set; }
        /// <summary>
        /// Sharp: 1, Flat: -1, Natural: 0
        /// </summary>
        public int Alter { get; set; }
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
        public string Lyric { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Note note &&
                   Step == note.Step &&
                   Alter == note.Alter &&
                   Octave == note.Octave &&
                   HasDot == note.HasDot &&
                   Length == note.Length &&
                   Lyric == note.Lyric;
        }

        public override int GetHashCode()
        {
            var hashCode = -1788234954;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Step);
            hashCode = hashCode * -1521134295 + Alter.GetHashCode();
            hashCode = hashCode * -1521134295 + Octave.GetHashCode();
            hashCode = hashCode * -1521134295 + HasDot.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Lyric);
            return hashCode;
        }
    }
}
