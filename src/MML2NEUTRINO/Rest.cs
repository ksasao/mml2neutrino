using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MML2NEUTRINO
{
    public class Rest : IElement
    {
        /// <summary>
        /// whether note has a dot (such as 'dotted quarter note')
        /// </summary>
        public bool HasDot { get; set; }
        /// <summary>
        /// L2C -> Length = 2 / D16 -> Length = 16 / C4. -> Length = 4 (dot is ignored)
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// MML
        /// </summary>
        public string MML { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Rest rest &&
                   HasDot == rest.HasDot &&
                   Length == rest.Length &&
                   MML == rest.MML;
        }

        public override int GetHashCode()
        {
            var hashCode = 1560665283;
            hashCode = hashCode * -1521134295 + HasDot.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MML);
            return hashCode;
        }
    }
}
