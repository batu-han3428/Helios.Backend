using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Helios.Common.Enums
{
    public enum CodingLibrary
    {
        None = 0,

        [Display(Name = "MedDra")]
        Medra = 1,
        //MedraV2 = 5,


        Who = 2,
        Custom = 3,
    }
}
