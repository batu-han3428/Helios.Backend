using Helios.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.eCRF.Models
{
    public class ModuleModel
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }

    }

}
