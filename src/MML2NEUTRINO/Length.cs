using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    public class Length : IElement
    {
        /// <summary>
        /// Length
        /// </summary>
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Length length &&
                   Value == length.Value;
        }

        /// <summary>
        /// MML
        /// </summary>
        public string MML { get; set; }
        public override int GetHashCode()
        {
            var hashCode = -1663560141;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MML);
            return hashCode;
        }
    }
}
