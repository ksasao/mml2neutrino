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

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }
    }
}
