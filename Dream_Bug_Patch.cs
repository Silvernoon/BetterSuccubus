using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using System.Reflection.Emit;
using System;
using System.Linq;

namespace BetterSuccubus;

[HarmonyPatch(typeof(ConSleep), nameof(ConSleep.SuccubusVisit))]
static class SuccubusVisit_Patch
{
    static bool Prefix(ConSleep __instance, Chara tg)
    {
        return Settings.DreamBugTeleport;
    }
}

[HarmonyPatch(typeof(ConSleep), nameof(ConSleep.SuccubusSleep))]
static class SuccubusSleep_Patch
{
    static bool Prefix(ConSleep __instance, Chara tg)
    {
        return Settings.DreamBugTeleport;
    }
}

[HarmonyPatch(typeof(ActDreamBug), nameof(ActDreamBug.Perform))]
static class ActDreamBug_Perform_Patch
{
    static void Postfix(ActDreamBug __instance)
    {
        if (Settings.DreamBugMakeSleep && Act.TC != Act.CC)
            Act.TC.Chara.AddCondition<ConSleep>(1000 + __instance.GetPower(Act.CC) * 10, true);
    }
}

[HarmonyPatch(typeof(Element), nameof(Element.GetCost))]
static class ActDreamBug_Cost_Patch
{
    static void Postfix(Element __instance, ref Act.Cost __result)
    {
        if (__instance.id == 6020)
            __result.cost = (int)(__result.cost * Settings.DreamBugCostScale);
    }
}

[HarmonyPatch(typeof(TraitDreamBug), nameof(TraitDreamBug.CanStack), MethodType.Getter)]
static class DreamBug_Stackable
{
    static bool Prefix(ref bool __result)
    {
        __result = Settings.DreamBugStackable;
        return false;
    }
}
/*
[HarmonyPatch(typeof(Trait), nameof(TraitDreamBug.CanBeStolen), MethodType.Getter)]
internal static class DreamBug_Stolen
{
    static bool Prefix(ref bool __result)
    {
        __result = Settings.DreamBugCanBeStolen;
        return false;
    }
}*/