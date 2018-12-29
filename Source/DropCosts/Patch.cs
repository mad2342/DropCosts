﻿using BattleTech;
using BattleTech.Framework;
using BattleTech.Save;
using BattleTech.Save.SaveGameStructure;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



namespace DropCosts {
    [HarmonyPatch(typeof(GameInstanceSave), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(GameInstance), typeof(SaveReason) })]
    public static class GameInstanceSave_Constructor_Patch {
        static void Postfix(GameInstanceSave __instance) {
            Helper.SaveState(__instance.InstanceGUID, __instance.SaveTime);
        }
    }

    [HarmonyPatch(typeof(GameInstance), "Load")]
    public static class GameInstance_Load_Patch {
        static void Prefix(GameInstanceSave save) {
            Helper.LoadState(save.InstanceGUID, save.SaveTime);
        }
    }

    [HarmonyPatch(typeof(AAR_ContractObjectivesWidget), "FillInObjectives")]
    public static class AAR_ContractObjectivesWidget_FillInObjectives {

        static void Postfix(AAR_ContractObjectivesWidget __instance) {
            try {
                Settings settings = Helper.LoadSettings();

                if (Fields.DropCost > 0)
                {
                    string missionObjectiveResultString = $"DROP COSTS DEDUCTED: ¢{Fields.FormattedDropCost}";
                    MissionObjectiveResult missionObjectiveResult = new MissionObjectiveResult(missionObjectiveResultString, "7facf07a-626d-4a3b-a1ec-b29a35ff1ac0", false, true, ObjectiveStatus.Succeeded, false);
                    ReflectionHelper.InvokePrivateMethode(__instance, "AddObjective", new object[] { missionObjectiveResult });
                }
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(Contract), "CompleteContract")]
    public static class Contract_CompleteContract {

        static void Postfix(Contract __instance) {
            try {
                int newMoneyResults = Mathf.FloorToInt(__instance.MoneyResults - Fields.DropCost);
                ReflectionHelper.InvokePrivateMethode(__instance, "set_MoneyResults", new object[] { newMoneyResults });
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(LanceHeaderWidget), "RefreshLanceInfo")]
    public static class LanceHeaderWidget_RefreshLanceInfo {    
        static void Postfix(LanceHeaderWidget __instance, List<MechDef> mechs) {
            try {
                Settings settings = Helper.LoadSettings();
                LanceConfiguratorPanel LC = (LanceConfiguratorPanel)ReflectionHelper.GetPrivateField(__instance, "LC");

                if (LC.IsSimGame) {
                    int lanceTonnage = 0;
                    float dropCost = 0f;

                    string freeTonnageText = "";
                    int freeTonnageAmount = settings.freeTonnageAmount;

                    if (settings.useFreeTonnageByDifficulty)
                    {
                        int difficulty = LC.activeContract.Difficulty;
                        if (difficulty < 1)
                        {
                            difficulty = 1;
                        }
                        if (difficulty > 10)
                        {
                            difficulty = 10;
                        }
                        freeTonnageAmount = settings.freeTonnageByDifficulty[difficulty - 1];
                    }

                    if (LC.activeContract.IsPriorityContract)
                    {
                        if (settings.useFreeTonnagePriorityBonus)
                        {
                            freeTonnageAmount += settings.freeTonnagePriorityBonus;
                        }
                    }

                    if(freeTonnageAmount > 400) {
                        freeTonnageAmount = 400;
                    }

                    if (settings.CostByTons) {
                        foreach (MechDef def in mechs) {
                            dropCost += (def.Chassis.Tonnage * settings.cbillsPerTon);
                            lanceTonnage += (int)def.Chassis.Tonnage;
                        }
                    } else {
                        foreach (MechDef def in mechs) {
                            dropCost += (Helper.CalculateCBillValue(def) * settings.percentageOfMechCost);
                            lanceTonnage += (int)def.Chassis.Tonnage;
                        }
                    }
                    if (settings.CostByTons && settings.someFreeTonnage) {
                        freeTonnageText = $"{freeTonnageAmount} TONS";
                        dropCost = Math.Max(0f, (lanceTonnage - freeTonnageAmount) * settings.cbillsPerTon);
                    }

                    string formattedDropCost = string.Format("{0:n0}", dropCost);
                    Fields.DropCost = dropCost;
                    Fields.LanceTonnage = lanceTonnage;
                    Fields.FormattedDropCost = formattedDropCost;
                    Fields.FreeTonnageText = freeTonnageText;

                    TextMeshProUGUI simLanceTonnageText = (TextMeshProUGUI)ReflectionHelper.GetPrivateField(__instance, "simLanceTonnageText");
                    // Fix for FP
                    simLanceTonnageText.enableWordWrapping = false;
                    // longer strings interfere with messages about incorrect lance configurations
                    simLanceTonnageText.text = $"DROP COST: ¢{Fields.FormattedDropCost}   LANCE WEIGHT: {Fields.LanceTonnage}/{Fields.FreeTonnageText}";
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }
}