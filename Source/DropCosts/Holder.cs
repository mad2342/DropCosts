namespace DropCosts
{
    public class Fields
    {
        public static float DropCost = 0;
        public static int LanceTonnage = 0;
        public static string FormattedDropCost = string.Empty;
        public static string FreeTonnageText = string.Empty;
    }

    public class SaveFields
    {
        public float DropCost = 0;
        public int LanceTonnage = 0;
        public string FormattedDropCost;
        public string FreeTonnageText;

        public SaveFields(float dropCost, int lanceTonnage, string formattedDropCost, string freeTonnageText)
        {
            DropCost = dropCost;
            LanceTonnage = lanceTonnage;
            FormattedDropCost = formattedDropCost;
            FreeTonnageText = freeTonnageText;
        }
    }
}