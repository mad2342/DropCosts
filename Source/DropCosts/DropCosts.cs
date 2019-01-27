using Harmony;
using System.Reflection;



namespace DropCosts
{
    public class DropCosts
    {
        internal static string ModDirectory;

        // BEN: Debug (0: nothing, 1: errors, 2:all)
        internal static int DebugLevel = 1;

        public static void Init(string directory, string settingsJSON) {
            var harmony = HarmonyInstance.Create("de.mad.DropCosts");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            ModDirectory = directory;
        }
    }
}
