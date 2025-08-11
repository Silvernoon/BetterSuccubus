extern alias UnityEngine_CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine_CoreModule.UnityEngine;

namespace BetterSuccubus;

// I truly cant understand why it call "emo2"
// emo is divided into two types: emo and emo2
// emo2 hangs on the side of the head


[HarmonyPatch(typeof(TCOrbitChara), nameof(TCOrbitChara.RefreshAll))]
static class ShowEmo2_Patch
{
  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
  {
    var codes = instructions.ToList();
    int i = 0;
    Label label = new();

    // find brfalse (191
    for (; i < codes.Count; i++)
      if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand as FieldInfo == typeof(TCOrbitChara).GetField("showIcon", BindingFlags.NonPublic | BindingFlags.Instance))
        if (codes[i + 1].opcode == OpCodes.Brfalse || codes[i + 1].opcode == OpCodes.Brfalse_S)
        {
          label = (Label)codes[i + 1].operand;
          break;
        }

    // find label (200
    for (; i < codes.Count; i++)
      if (codes[i].labels.Contains(label))
        break;

    codes.InsertRange(i, [
      new(OpCodes.Ldarg_0),
      Transpilers.EmitDelegate((TCOrbitChara __instance) => {
        if (__instance.owner.HasCondition<ConCharm>())
          __instance.iconStatus.sprite = TexManager.SpriteMap["EmoCharm"];
        else if (__instance.owner.HasCondition<ConCalmDown>())
          __instance.iconStatus.sprite = TexManager.SpriteMap["EmoCalmDown"];}),
      ]);

    return codes.AsEnumerable();
  }
}
