using System.Collections.Generic;

namespace DropCosts
{
    public class Settings
    {
        public int CbillsPerTon = 5000;
        public int[] FreeTonnageByDifficulty = new int[] { 140, 160, 180, 200, 220, 260, 280, 300, 320, 340 };
        public bool UseFreeTonnagePriorityBonus = true;
        public int FreeTonnagePriorityBonus = 20;

        public List<string> ContractExcludeList = new List<string>()
        {
            "tournament_b1_3wayBattle"
        };
    }
}