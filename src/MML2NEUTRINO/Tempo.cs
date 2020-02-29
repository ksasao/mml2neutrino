using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    public class Tempo : IElement
    {
        /// <summary>
        /// Tempo
        /// </summary>
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Tempo tempo &&
                   Value == tempo.Value;
        }
        /// <summary>
        /// MML
        /// </summary>
        public string MML { get; set; }
        public override int GetHashCode()
        {
            int hashCode = -1937169414;
            hashCode = Value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MML);
            return hashCode;
        }
    }
}
