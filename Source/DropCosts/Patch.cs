using BattleTech;
using BattleTech.Framework;
using BattleTech.Save;
using BattleTech.Save.SaveGameStructure;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DropCosts
{
    [HarmonyPatch(typeof(GameInstanceSave), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(GameInstance), typeof(SaveReason) })]
    public static class GameInstanceSave_Constructor_Patch
    {
        static void Postfix(GameInstanceSave __instance)
        {
            Helper.SaveState(__instance.InstanceGUID, __instance.SaveTime);
        }
    }

    [HarmonyPatch(typeof(GameInstance), "Load")]
    public static class GameInstance_Load_Patch
    {
        static void Prefix(GameInstanceSave save)
        {
            Helper.LoadState(save.InstanceGUID, save.SaveTime);
        }
    }

    [HarmonyPatch(typeof(AAR_ContractObjectivesWidget), "FillInObjectives")]
    public static class AAR_ContractObjectivesWidget_FillInObjectives
    {

        static void Postfix(AAR_ContractObjectivesWidget __instance)
        {
            try
            {
                if (Fields.DropCost > 0)
                {
                    string missionObjectiveResultString = $"DROP COSTS DEDUCTED: ¢{Fields.FormattedDropCost}";
                    MissionObjectiveResult missionObjectiveResult = new MissionObjectiveResult(missionObjectiveResultString, "7facf07a-626d-4a3b-a1ec-b29a35ff1ac0", false, true, ObjectiveStatus.Succeeded, false);
                    ReflectionHelper.InvokePrivateMethode(__instance, "AddObjective", new object[] { missionObjectiveResult });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    [HarmonyPatch(typeof(Contract), "CompleteContract")]
    public static class Contract_CompleteContract
    {
        static void Postfix(Contract __instance)
        {
            try
            {
                int newMoneyResults = Mathf.FloorToInt(__instance.MoneyResults - Fields.DropCost);
                ReflectionHelper.InvokePrivateMethode(__instance, "set_MoneyResults", new object[] { newMoneyResults });
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

    [HarmonyPatch(typeof(LanceHeaderWidget), "RefreshLanceInfo")]
    public static class LanceHeaderWidget_RefreshLanceInfo
    {    
        static void Postfix(LanceHeaderWidget __instance, List<MechDef> mechs) {
            try
            {
                LanceConfiguratorPanel LC = (LanceConfiguratorPanel)ReflectionHelper.GetPrivateField(__instance, "LC");

                // Fix for normal contracts with restrictions set:
                // Only display/set if there are no tonnage restrictions set by the mission itself
                if (Helper.MissionRestrictionsActive(LC.activeContract.Override))
                {
                    // To disable display in mission results this needs to be 0
                    Fields.DropCost = 0;
                    return;
                }

                if (LC.IsSimGame)
                {
                    int lanceTonnage = 0;
                    float dropCost = 0f;

                    string freeTonnageText = "";
                    int freeTonnageAmount = 0;

                    // This can turn out to be too nasty
                    //int difficulty = LC.activeContract.Override.GetUIDifficulty();

                    int difficulty = LC.activeContract.Difficulty;
                    if (difficulty < 1)
                    {
                        difficulty = 1;
                    }
                    if (difficulty > 10)
                    {
                        difficulty = 10;
                    }
                    freeTonnageAmount = DropCosts.Settings.FreeTonnageByDifficulty[difficulty - 1];

                    if (LC.activeContract.IsPriorityContract)
                    {
                        if (DropCosts.Settings.UseFreeTonnagePriorityBonus)
                        {
                            freeTonnageAmount += DropCosts.Settings.FreeTonnagePriorityBonus;
                        }
                    }

                    if(freeTonnageAmount > 400)
                    {
                        freeTonnageAmount = 400;
                    }

                    foreach (MechDef def in mechs)
                    {
                        lanceTonnage += (int)def.Chassis.Tonnage;
                    }
                    dropCost = Math.Max(0f, (lanceTonnage - freeTonnageAmount) * DropCosts.Settings.CbillsPerTon);

                    freeTonnageText = $"{freeTonnageAmount} TONS";

                    string formattedDropCost = string.Format("{0:n0}", dropCost);
                    Fields.DropCost = dropCost;
                    Fields.LanceTonnage = lanceTonnage;
                    Fields.FormattedDropCost = formattedDropCost;
                    Fields.FreeTonnageText = freeTonnageText;

                    TextMeshProUGUI simLanceTonnageText = (TextMeshProUGUI)ReflectionHelper.GetPrivateField(__instance, "simLanceTonnageText");
                    // Fix for FP
                    simLanceTonnageText.enableAutoSizing = false;
                    simLanceTonnageText.enableWordWrapping = false;

                    // Longer strings interfere with messages about incorrect lance configurations
                    simLanceTonnageText.text = $"DROP COST: ¢{Fields.FormattedDropCost}   LANCE WEIGHT: {Fields.LanceTonnage}/{Fields.FreeTonnageText}";
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}