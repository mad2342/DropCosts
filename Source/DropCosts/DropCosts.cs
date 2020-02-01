using Harmony;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace DropCosts
{
    public class DropCosts
    {
        internal static string LogPath;
        internal static string ModDirectory;
        internal static Settings Settings;
        // BEN: DebugLevel (0: nothing, 1: error, 2: debug, 3: info)
        internal static int DebugLevel = 1;

        public static void Init(string directory, string settings)
        {
            ModDirectory = directory;
            LogPath = Path.Combine(ModDirectory, "DropCosts.log");

            Logger.Initialize(LogPath, DebugLevel, ModDirectory, nameof(DropCosts));

            try
            {
                Settings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            catch (Exception e)
            {
                Settings = new Settings();
                Logger.Error(e);
            }

            HarmonyInstance harmony = HarmonyInstance.Create("de.mad.DropCosts");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
