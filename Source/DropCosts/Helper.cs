using BattleTech.Framework;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DropCosts
{
    public class Helper
    {
        public static void SaveState(string instanceGUID, DateTime saveTime)
        {
            try
            {
                int unixTimestamp = (int)(saveTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string filePath = $"{DropCosts.ModDirectory}/.savestate/" + instanceGUID + "-" + unixTimestamp + ".json";
                (new FileInfo(filePath)).Directory.Create();
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    SaveFields fields = new SaveFields(Fields.DropCost, Fields.LanceTonnage, Fields.FormattedDropCost, Fields.FreeTonnageText);
                    string json = JsonConvert.SerializeObject(fields);
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static void LoadState(string instanceGUID, DateTime saveTime)
        {
            try
            {
                int unixTimestamp = (int)(saveTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string filePath = $"{DropCosts.ModDirectory}/.savestate/" + instanceGUID + "-" + unixTimestamp + ".json";
                if (File.Exists(filePath))
                {
                    using (StreamReader r = new StreamReader(filePath))
                    {
                        string json = r.ReadToEnd();
                        SaveFields save = JsonConvert.DeserializeObject<SaveFields>(json);
                        Fields.DropCost = save.DropCost;
                        Fields.LanceTonnage = save.LanceTonnage;
                        Fields.FormattedDropCost = save.FormattedDropCost;
                        Fields.FreeTonnageText = save.FreeTonnageText;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex);
            }
        }



        public static bool MissionRestrictionsActive(ContractOverride contractOverride)
        {
            if (contractOverride.lanceMaxTonnage > 0 || contractOverride.lanceMinTonnage > 0)
            {
                return true;
            }
            foreach (float restriction in contractOverride.mechMaxTonnages)
            {
                if (restriction > 0)
                {
                    return true;
                }
            }
            foreach (float restriction in contractOverride.mechMinTonnages)
            {
                if (restriction > 0)
                {
                    return true;
                }
            }
            return false;
        }



        public static bool ContractOnExcludeList(ContractOverride contractOverride)
        {
            if (DropCosts.Settings.ContractExcludeList.Contains(contractOverride.ID))
            {
                return true;
            }
            return false;
        }
    }
}