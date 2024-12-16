using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
namespace BetterSuccubus;

[HarmonyPatch(typeof(Feat), nameof(Feat.Apply))]
internal static class SuccubusRace_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref Feat __instance, int a, ElementContainer owner, bool hint = false)
    {
        MethodInfo ModBase = Enumerable.FirstOrDefault<MethodInfo>(typeof(Feat).GetMethods((BindingFlags)52), (MethodInfo m) => m.Name.Contains("ModBase"));

        Type nestedType = typeof(Feat).GetNestedType("<>c__DisplayClass19_0", (BindingFlags)32);
        object displayClassInstance = Activator.CreateInstance(nestedType);
        nestedType.GetField("owner").SetValue(displayClassInstance, owner);
        nestedType.GetField("hint").SetValue(displayClassInstance, hint);
        int id = __instance.id;
        if (id == 1216)
        {
            //ModBase.Invoke(__instance, [6020, a, false, displayClassInstance]);
            ModBase.Invoke(__instance, [6030, a, false, displayClassInstance]);
        }
    }
    public const int ActCharm = 6030;
}

[HarmonyPatch(typeof(Player), nameof(Player.OnLoad))]
internal static class AddAbility
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!Settings.DebugAddAbility)
        {
            return;
        }
        if (!EClass.pc.HasElement(6030, 1) && EClass.pc.IsSuccubus())
        {
            EClass.pc.GainAbility(6030, 1);
            BetterSuccubus.Logger.LogInfo("Add Ability Successfully");
        }
        else BetterSuccubus.Logger.LogInfo("PC already have ability");
    }
}


