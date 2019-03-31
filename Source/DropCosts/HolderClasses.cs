namespace DropCosts {
    public class Settings {
        public float percentageOfMechCost = 0.0025f;

        public bool CostByTons = true;
        public int cbillsPerTon = 5000;
        public bool someFreeTonnage = true;
        public int freeTonnageAmount = 0;
        public bool useFreeTonnageByDifficulty = true;
        public int[] freeTonnageByDifficulty = new int[] { 140, 160, 180, 200, 220, 260, 280, 300, 320, 340 };
        public bool useFreeTonnagePriorityBonus = true;
        public int freeTonnagePriorityBonus = 20;
    }

    public class Fields {
        public static float DropCost = 0;
        public static int LanceTonnage = 0;
        public static string FormattedDropCost = string.Empty;
        public static string FreeTonnageText = string.Empty;
    }
}