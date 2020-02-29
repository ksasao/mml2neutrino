using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    public class Octave : IElement
    {
        /// <summary>
        /// Octave
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// MML
        /// </summary>
        public string MML { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Octave octave &&
                   Value == octave.Value &&
                   MML == octave.MML;
        }

        public override int GetHashCode()
        {
            var hashCode = -1663560141;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MML);
            return hashCode;
        }
    }
}
