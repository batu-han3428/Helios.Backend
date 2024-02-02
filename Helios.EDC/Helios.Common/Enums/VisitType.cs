using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Common.Enums
{
    public enum VisitType
    {
        /// <summary>
        /// Her katlımcı için 1 kere doldurulabilir.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Her katılımcı için birden fazla doldurulabilir.
        /// Optinal- adverse olay tipleri
        /// </summary>
        Multi = 2,

        /// <summary>
        /// AE-SAE formlar
        /// </summary>
        AdversEvent = 3,
        SeriousAdversEvent = 4
    }

    public enum VisitStatu
    {
        visit = 1,
        page = 2,
        module = 3
    }
}
