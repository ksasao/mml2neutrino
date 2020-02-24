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
    }
}
