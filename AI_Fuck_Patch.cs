using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using System.Reflection.Emit;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Net.NetworkInformation;

namespace BetterSuccubus;

[HarmonyPatch(typeof(AI_Fuck), nameof(AI_Fuck.Finish))]
internal static class AI_Fuck_Patch
{
    static bool Prefix(AI_Fuck __instance)
    {
        Chara chara = __instance.sell ? __instance.target : __instance.owner;
        Chara chara2 = __instance.sell ? __instance.owner : __instance.target;
        if (chara.isDead || chara2.isDead)
            return false;

        bool flag = EClass.rnd(2) == 0;
        switch (__instance.Type)
        {
            case AI_Fuck.FuckType.fuck:
                {
                    for (int i = 0; i < 2; i++)
                    {
                        mofet(chara);
                        mofet(chara2);

                        static void mofet(Chara chara3)
                        {
                            chara3.RemoveCondition<ConDrunk>();
                            chara3.ModExp(77, 250);
                            chara3.ModExp(71, 250);
                            chara3.ModExp(75, 250);

                            chara3.SAN.Mod((Settings.SexNoSleepy && chara3.IsSuccubus()) ? -Settings.SexNoSleepyValue : 10);
                            if (!chara3.IsSuccubus() && EClass.rnd(15) == 0)
                                chara3.AddCondition<ConDisease>(200);
                        }
                    }
                    if (!chara2.IsSuccubus())
                    {
                        if (EClass.rnd(5) == 0)
                            chara2.AddCondition<ConParalyze>(500);

                        if (EClass.rnd(3) == 0)
                            chara2.AddCondition<ConInsane>(100 + EClass.rnd(100));
                    }

                    chara.Talk("tail_after");
                    if (__instance.succubus)
                    {
                        chara.ShowEmo(Emo.love);
                        chara2.ShowEmo(Emo.love);
                        EClass.player.forceTalk = true;
                        chara2.Talk("seduced");
                    }
                    else if (chara != EClass.pc)
                    {
                        int num = CalcMoney.Whore(chara2);
                        if (__instance.bitch)
                            (chara2, chara) = (chara, chara2);

                        if (!chara.IsPCParty && chara2 == EClass.pc && EClass.rnd(4) != 0)
                        {
                            if (Settings.GreaterReward)
                                num += EClass.pc.elements.GetElement(77).ValueWithoutLink * Settings.GreaterRewardFactor;
                            num = num / 5 + 1;
                            chara.ModCurrency(num);
                        }

                        if (chara.GetCurrency() >= num)
                        {
                            chara.Talk("tail_pay");
                        }
                        else
                        {
                            chara.Talk("tail_nomoney");
                            num = chara.GetCurrency();
                            chara2.Say("angry", chara2);
                            chara2.Talk("angry");
                            flag = __instance.sell;
                            if (EClass.rnd(20) == 0)
                                chara2.DoHostileAction(chara);
                        }

                        chara.ModCurrency(-num);
                        if (chara2 == EClass.pc)
                        {
                            if (num > 0)
                            {
                                EClass.player.DropReward(ThingGen.Create("money").SetNum(num));
                                EClass.player.ModKarma(-1);
                            }
                        }
                        else
                        {
                            chara2.ModCurrency(num);
                        }

                        if (__instance.bitch)
                            (chara2, chara) = (chara, chara2);
                    }

                    SuccubusExp(chara, chara2);
                    SuccubusExp(chara2, chara);

                    if (chara2.HasCondition<ConCharm>())
                    {
                        #region 伤害判定
                        int damage = chara.MaxHP / 2;
                        /*
                                                if (Sss.trytodo && chara2.hp <= damage)
                                                {
                                                    Sss.trytodo = false;
                                                    Dialog.TryWarn("warn_mana", () =>
                                                    {
                                                        Sss.trytodo = true;
                                                        EClass.pc.SetAI(new AI_Fuck
                                                        {
                                                            target = chara2,
                                                            succubus = true
                                                        });
                                                    }, true);
                                                }
                                                if (!Sss.trytodo)
                                                {
                                                    Sss.trytodo = true;
                                                    yield return __instance.Cancel();
                                                    yield break;
                                                }
                        */
                        chara2.DamageHP(damage, AttackSource.Finish, chara);
                        #endregion
                        #region 结束
                        chara.UseAbility("ActCharm", chara2);
                        #endregion
                    }
                    break;
                }
            case AI_Fuck.FuckType.tame:
                if (__instance.CanTame())
                    if (flag)
                    {
                        chara.Say("tame_success", __instance.owner, __instance.target);
                        chara2.MakeAlly();
                        chara.elements.ModExp(237, 200);
                    }
                    else
                        chara.Say("tame_fail", chara, chara2);
                else
                    chara.Say("tame_invalid", chara2);
                break;
        }

        chara2.ModAffinity(chara, (chara.IsSuccubus() || chara2.IsSuccubus() || flag) ? 10 : (-5));
        static void SuccubusExp(Chara c, Chara tg)
        {
            if (!c.IsSuccubus())
                return;

            foreach (Element item in tg.elements.ListBestAttributes())
                if (c.elements.ValueWithoutLink(item.id) < item.ValueWithoutLink)
                {
                    c.elements.ModTempPotential(item.id, 1 + EClass.rnd(item.ValueWithoutLink - c.elements.ValueWithoutLink(item.id) / 5 + 1));
                    c.Say("succubus_exp", c, item.Name.ToLower());
                    break;
                }

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
        return false;
    }

}

[HarmonyPatch(typeof(Chara), nameof(Chara.Die))]
internal static class OnDie
{
    static bool Prefix(Chara __instance, Card origin, AttackSource attackSource)
    {
        if (__instance.HasCondition<ConCharm>() && origin != null && attackSource == AttackSource.Finish)
        {
            //BetterSuccubus.Logger.LogError("WRYYYYYYYYYYYYYY");
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

[HarmonyPatch(typeof(AI_Fuck), nameof(AI_Fuck.Run))]
internal static class AI_Fuck_Run_Patch
{/*
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {//188, 5 是tc和cc
        var methodInfo = AccessTools.Method(typeof(AI_Fuck_Run_Patch), nameof(StatMod));
        List<CodeInstruction> codes = instructions.ToList();
        foreach(var item in codes)
        {
            yield return item;
            if(item.opcode == OpCodes.Callvirt && item.OperandIs(AccessTools.Method(typeof(CardRenderer), nameof(CardRenderer.PlayAnime), [typeof(AnimeID), typeof(Card)])))
            {
                BetterSuccubus.Logger.LogInfo("Success Patch");
                foreach (var k in codes.GetRange(188, 5))
                    yield return k;
                yield return new(OpCodes.Callvirt, methodInfo);
            }
        }
    }*/
    static bool Prefix(AI_Fuck __instance) => false;

    static IEnumerable<AIAct.Status> Postfix(IEnumerable<AIAct.Status> values, AI_Fuck __instance)
    {
        if (__instance.target == null)
        {
            foreach (Chara chara in EClass._map.charas)
            {
                if (!chara.IsHomeMember() && !chara.IsDeadOrSleeping && chara.Dist(__instance.owner) <= 5)
                {
                    __instance.target = chara;
                    break;
                }
            }
        }
        if (__instance.target == null)
        {
            yield return __instance.Cancel();
        }
        Chara cc = (__instance.sell ? __instance.target : __instance.owner);
        Chara tc = (__instance.sell ? __instance.owner : __instance.target);
        cc.Say(__instance.Type.ToString() + "_start", cc, tc, null, null);
        __instance.isFail = () => !tc.IsAliveInCurrentZone || tc.Dist(__instance.owner) > 3;
        if (__instance.Type == AI_Fuck.FuckType.tame)
        {
            cc.SetTempHand(1104, -1);
        }
        int destDist = ((__instance.Type == AI_Fuck.FuckType.fuck) ? 1 : 1);
        __instance.maxProgress = 25;
        if (__instance.succubus)
        {
            cc.Talk("seduce", null, null, false);
        }
        int num;
        for (int i = 0; i < __instance.maxProgress; i = num + 1)
        {
            __instance.progress = i;
            yield return __instance.DoGoto(__instance.target.pos, destDist, false, null);
            AI_Fuck.FuckType type = __instance.Type;
            if (type != AI_Fuck.FuckType.fuck)
            {
                if (type == AI_Fuck.FuckType.tame)
                {
                    if (EClass.rnd(8) == 0)
                    {
                        tc.AddCondition<ConFear>(50, false);
                    }
                    if (i == 0 || i == 10)
                    {
                        cc.Talk("goodBoy", null, null, false);
                    }
                    cc.elements.ModExp(237, 10, false);
                }
            }
            else
            {
                cc.LookAt(tc);
                tc.LookAt(cc);
                num = i % 4;
                if (num != 0)
                {
                    if (num == 2)
                    {
                        StatMod(tc, cc);
                        tc.renderer.PlayAnime(AnimeID.Shiver, default, false);
                        if (EClass.rnd(3) == 0)
                        {
                            tc.Talk("tailed", null, null, false);
                        }
                    }
                }
                else
                {
                    StatMod(cc, tc);
                    cc.renderer.PlayAnime(AnimeID.Attack, tc);
                    if (EClass.rnd(3) == 0 || __instance.sell)
                    {
                        cc.Talk("tail", null, null, false);
                    }
                }
                if (EClass.rnd(3) == 0 || __instance.sell)
                {
                    __instance.target.AddCondition<ConWait>(50, true);
                }
            }
            num = i;
        }
        __instance.Finish();
        yield break;
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
                    chara2.DamageHP(i);
                }
                if (Settings.EnableMPDrain)
                {
                    chara.mana.Mod(i);
                    chara2.mana.Mod(-i);
                }
                //chara2.AddCondition<Con>
            }
        }
    }


}
/*
[HarmonyPatch(typeof(AI_Fuck), MethodType.Constructor)]
internal static class AI_Fuck_Constuct
{
    static void Postfix(AI_Fuck __instance)
    {
        if (Settings.NGPN)
            global::Lang.General.map["AI_Fuck"].text_L = "姦！";
        else if (__instance.target.HasCondition<ConCharm>())
            global::Lang.General.map["AI_Fuck"].text_L = "捕食";
        else global::Lang.General.map["AI_Fuck"].text_L = "干坏坏的事";
    }
}
*/
[HarmonyPatch(typeof(ActPlan), nameof(ActPlan._Update))]
internal static class ActPlan_Patch
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