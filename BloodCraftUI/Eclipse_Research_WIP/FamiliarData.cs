using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BloodCraftUI.Eclipse
{
    internal class FamiliarData(string percent, string level, string prestige, string familiarName, string familiarStats)
    {
        public float Progress { get; set; } = float.Parse(percent, CultureInfo.InvariantCulture) / 100f;
        public int Level { get; set; } = int.TryParse(level, out int parsedLevel) && parsedLevel > 0 ? parsedLevel : 1;
        public int Prestige { get; set; } = int.Parse(prestige);
        public string FamiliarName { get; set; } = !string.IsNullOrEmpty(familiarName) ? familiarName : "Familiar";
        public List<string> FamiliarStats { get; set; } = !string.IsNullOrEmpty(familiarStats) ? [.. new List<string> { familiarStats[..4], familiarStats[4..7], familiarStats[7..] }.Select(stat => int.Parse(stat).ToString())] : ["", "", ""];
    }
}
