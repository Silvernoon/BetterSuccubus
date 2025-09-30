using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
namespace BetterSuccubus;

[HarmonyPatch(typeof(Feat), nameof(Feat.Apply))]
static class SuccubusRace_Patch
{
  static void Postfix(ref Feat __instance, int a, ElementContainer owner, bool hint = false)
  {
    MethodInfo methodInfo = typeof(Feat).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name.Contains("ModBase"));
    Type type = typeof(Feat).GetNestedTypes(BindingFlags.NonPublic).FirstOrDefault(t => t.Name.Contains("DisplayClass"));
    object obj = Activator.CreateInstance(type);
    type.GetField("owner").SetValue(obj, owner);
    type.GetField("hint").SetValue(obj, hint);
    if (__instance.id == 1216 && __instance.owner == EClass.pc.elements)
      methodInfo.Invoke(__instance, [Data.ActCharm.id, a, false, obj]);
  }
}

[HarmonyPatch(typeof(Player), nameof(Player.OnLoad))]
static class AddAbility
{
  static void Postfix()
  {
    if (!Settings.DebugAddAbility)
      return;

    if (!EClass.pc.HasElement(Data.ActCharm.id, 1) && EClass.pc.IsSuccubus())
    {
      var method = typeof(Chara).GetMethod("GainAbility");
      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length == 2)
        method.Invoke(EClass.pc, [Data.ActCharm.id, 1]);
      else if (parameters.Length == 3)
        method.Invoke(EClass.pc, [Data.ActCharm.id, 1, null]);

      BetterSuccubus.Logger.LogInfo("Add Ability Successfully");
    }
    else BetterSuccubus.Logger.LogInfo("PC already have ability");
  }
}


