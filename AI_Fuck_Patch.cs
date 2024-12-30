extern alias UnityEngine_CoreModule;
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BetterSuccubus;

[HarmonyPatch(typeof(AI_Fuck), nameof(AI_Fuck.Finish))]
static class AI_Fuck_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        //Affinity
        //chara2.ModAffinity(chara, flag ? 10 : (-5));
        //flag = chara.IsSuccubus() || chara2.IsSuccubus() || EClass.rnd(2) == 0;
        return new CodeMatcher(instructions).MatchEndForward(new(OpCodes.Ldc_I4_2), new(OpCodes.Call, AccessTools.Method(typeof(EClass), nameof(EClass.rnd))), new(OpCodes.Ldc_I4_0), new(OpCodes.Ceq), new(OpCodes.Stloc_2))
            .Advance(1)
            .InsertAndAdvance(
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldloc_1),
                Transpilers.EmitDelegate((Chara chara, Chara chara2) => { return (Settings.AffinityIncrease && (chara.IsSuccubus() || chara2.IsSuccubus())) || EClass.rnd(2) == 0; }),
                new(OpCodes.Stloc_2)
                )
        .MatchStartForward(new CodeMatch(o => o.opcode == OpCodes.Call && o.operand.ToString().Contains("<Finish>g__SuccubusExp")))
            .Advance(1)
            .InsertAndAdvance(new(OpCodes.Ldloc_0), new(OpCodes.Ldloc_1), Transpilers.EmitDelegate(Damage))
            .InsertAndAdvance(new(OpCodes.Ldloc_0), new(OpCodes.Ldloc_1), Transpilers.EmitDelegate(SuccubusSkillExp))
            .InsertAndAdvance(new(OpCodes.Ldloc_1), new(OpCodes.Ldloc_0), Transpilers.EmitDelegate(SuccubusSkillExp))
        .InstructionEnumeration();
    }
    static void Damage(Chara chara, Chara chara2)
    {
        if (chara2.HasCondition<ConCharm>())
        {
            int damage = chara.MaxHP / 2;
            chara2.DamageHP(damage, AttackSource.Finish, chara);
            chara.UseAbility("ActCharm", chara2);
        }
    }
    static void SuccubusSkillExp(Chara c, Chara tg)
    {
        if (!c.IsSuccubus())
            return;
        foreach (Element item in tg.elements.ListBestSkills())
            if (c.elements.ValueWithoutLink(item.id) != 0 && c.elements.ValueWithoutLink(item.id) < item.ValueWithoutLink)
            {
                if (Settings.SkillImprovementPotential)
                {
                    c.elements.ModTempPotential(item.id, 1 + EClass.rnd(item.ValueWithoutLink - c.elements.ValueWithoutLink(item.id) / 5 + 1));
                    c.Say("succubus_exp", c, item.Name.ToLower());
                }
                if (Settings.SkillImprovement)
                    c.ModExp(item.id, Settings.SkillImprovementValue);
                break;
            }
    }
}

[HarmonyPatch(typeof(Chara), nameof(Chara.Die))]
static class OnDie
{
    static bool Prefix(Chara __instance, Card origin, AttackSource attackSource)
    {
        if (__instance.HasCondition<ConCharm>() && origin == EClass.pc && attackSource == AttackSource.Finish)
        {
            int damage = origin.MaxHP / 2;
            if (Settings.KillRecoverHP)
                origin.HealHP(damage / 2);
            if (Settings.KillRecoverMP)
                origin.Chara.mana.Mod(damage / 2);
            if (Settings.KillRecoverSP)
                origin.Chara.stamina.Mod(damage / 2);
            foreach (Element item in __instance.elements.ListBestAttributes())
            {
                int value = item.ValueWithoutLink / 20;
                origin.ModExp(item.id, value * Settings.KillBoostAttributes);
                __instance.elements.ModBase(item.id, -value);
            }
        }
        return true;
    }
}

[HarmonyPatch("AI_Fuck+<Run>d__25", "MoveNext")]
static class AI_Fuck_Run_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher codeMatcher = new(instructions);
        var ccttc = codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), "LookAt", [typeof(Card)]))).InstructionsWithOffsets(-5, -1);
        var tctcc = codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Card), "LookAt", [typeof(Card)]))).InstructionsWithOffsets(-5, -1);

        codeMatcher.Start().MatchEndForward(new(OpCodes.Ldfld), new(OpCodes.Callvirt, AccessTools.Method(typeof(CardRenderer), "PlayAnime", [typeof(AnimeID), typeof(Card)])))
            .Advance(1).InsertAndAdvance(ccttc).InsertAndAdvance(Transpilers.EmitDelegate(StatMod));

        codeMatcher.Start().MatchEndForward(new(OpCodes.Ldc_I4_0), new(OpCodes.Callvirt, AccessTools.Method(typeof(CardRenderer), "PlayAnime", [typeof(AnimeID), typeof(UnityEngine_CoreModule.UnityEngine.Vector3), typeof(bool)])))
            .Advance(1).InsertAndAdvance(tctcc).InsertAndAdvance(Transpilers.EmitDelegate(StatMod));

        return codeMatcher.InstructionEnumeration();
    }

    static void StatMod(Chara chara, Chara chara2)
    {
        if (chara.IsPCParty || chara2.IsPCParty)
        {
            int num = -1 - EClass.rnd(chara.stamina.max / 10 / 25);
            if (!chara.IsSuccubus())
                chara.stamina.Mod(num);
            else
            {
                int i = 1 + EClass.rnd((int)(chara.stamina.max * Settings.DrainScale) / 10);
                chara.stamina.Mod(Settings.EnableSTRecovery ? (i + chara2.LV / 10) : num);
                if (Settings.EnableNoHunger) chara.hunger.Mod(-Settings.HungerValue);
                if (Settings.EnableHPDrain)
                {
                    chara.HealHP(i);
                    chara2.DamageHP(i, AttackSource.Finish, chara);
                }
                if (Settings.EnableMPDrain)
                {
                    chara.mana.Mod(i);
                    chara2.mana.Mod(-i);
                }
            }
        }
    }
}

[HarmonyPatch(typeof(ActPlan), nameof(ActPlan._Update))]
static class ActPlan_Patch
{
    static void Postfix(ActPlan __instance)
    {
        Point _pos = new(__instance.pos);
        try
        {
            List<Card> cards = _pos?.ListCards(false);
            if (_pos == EClass.pc.pos)
                return;
            cards?.ForeachReverse(delegate (Card _c)
            {
                Chara chara = _c.Chara;
                if (chara == null || !EClass.pc.CanSee(chara))
                {
                    return;
                }
                int num = chara.Dist(EClass.pc);
                if (num > 1 && EClass.pc.isBlind)
                {
                    return;
                }
                bool isKey = __instance.input == ActInput.Key;
                if (!EClass.pc.isBlind && __instance.input == ActInput.AllAction && (__instance.input == ActInput.AllAction || !chara.IsNeutral() || chara.quest != null || EClass.game.quests.IsDeliverTarget(chara)) && chara.isSynced && num <= 2 && ((!chara.HasCondition<ConSuspend>() && (!chara.isRestrained || !chara.IsPCFaction)) || __instance.altAction) && EClass.pc.IsSuccubus())
                {
                    if (Settings.NGPN)
                        global::Lang.General.map["AI_Fuck"].text_L = "姦！";
                    else if (chara.HasCondition<ConCharm>())
                    {
                        global::Lang.General.map["AI_Fuck"].text_L = "捕食";
                        global::Lang.General.map["AI_Fuck"].text_JP = "エナジードレイン";
                        global::Lang.General.map["AI_Fuck"].text = "Predation";
                    }
                    else
                    {
                        global::Lang.General.map["AI_Fuck"].text_L = "干坏坏的事";
                        global::Lang.General.map["AI_Fuck"].text = "Seduce";
                        global::Lang.General.map["AI_Fuck"].text_JP = "悪さをする";
                    }
                    if (!chara.HasCondition<ConSleep>() && chara.HasCondition<ConCharm>())
                        DOIT();
                    if (!chara.IsHostile())
                    {
                        if (!chara.HasCondition<ConSleep>() && Settings.SexNoNeed)
                            DOIT();
                    }
                    else
                        if (chara.HasCondition<ConSleep>() || Settings.SexNoNeed)
                        DOIT();
                    void DOIT() =>
                        __instance.TrySetAct(new AI_Fuck
                        {
                            target = chara,
                            succubus = true
                        }, chara);
                }
            });
        }
        catch { return; }
    }
}

public static class CharaExtension
{
    public static bool IsSuccubus(this Chara chara)
    {
        return chara.HasElement(1216);
    }
}