using System.Collections.Generic;

namespace AssetManagement.Business.DTOs
{
    public class DashboardDTO
    {
        public int TotalAssets { get; set; }
        public int AssignedAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int UnderRepairAssets { get; set; }
        public int RetiredAssets { get; set; }
        public int SpareAssets { get; set; }
        public int ActiveEmployees { get; set; }
        public Dictionary<string, int> AssetsByType { get; set; } = new Dictionary<string, int>();
    }
}
