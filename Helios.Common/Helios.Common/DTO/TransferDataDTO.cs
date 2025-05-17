using Helios.Common.Enums;

namespace Helios.Common.DTO
{
    public class TransferDataDTO
    {
        public Int64 Id { get; set; }
        public VisitStatu Type { get; set; }
        public TransferChangeType Statu { get; set; }
    }
}
