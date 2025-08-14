extern alias UnityEngine_CoreModule;
using System.Collections.Generic;
using BetterSuccubus;
using HarmonyLib;
public class ConCharm : BadCondition
{
  public override void SetOwner(Chara _owner, bool onDeserialize = false)
  {
    owner = _owner;
    //base.SetOwner(_owner, false);
    elements = new ElementContainer();
    elements.SetParent(owner);
    //owner.renderer?.RefreshSprite();
    try
    {
      //GoalBattle  确保魅惑后不会攻击
      if (owner.OriginalHostility <= Hostility.Enemy)
        owner.c_originalHostility = Hostility.Neutral;

      owner.hostility += 2;
      owner.enemy = null;

      owner.SetAI(new AI_Goto(pc.pos, 1));
      //owner.ShowEmo(Emo.love, 0f, false);
    }
    catch { return; }
  }
  public override Emo2 EmoIcon => Emo2.speeing;
  public override string GetPhaseStr() => BetterSuccubus.Texts.ConCharmPhaseStr.Lang();

  public override void Tick()
  {
    //owner.ai = new NoGoal();
    if (owner.ai == null || owner.ai is GoalCombat) // 接近
      owner.SetAI(new AI_Goto(pc.pos, 1));
    else owner.ai.DoGoto(pc.pos, 1, false, null);

    owner.enemy = null;
    //owner.ShowEmo(Emo.love, 1f, false);

    if (pc.enemy == owner.Chara) //玩家仇恨
      pc.enemy = null;

    foreach (Chara chara in pc.party.members) //同伴仇恨
      if (chara.enemy == owner.Chara && chara.ai is GoalCombat)
      {
        chara.enemy = null;
        chara.SetAI(new NoGoal());
      }

    if (pc.ai is AI_Fuck)
      Mod(5);
    Mod(-1);
  }
  //public override void PlayEffect()
  //{
  //}
  //public override void PlayEndEffect()
  //{ }
  public override void OnRemoved()
  {
    if (owner.isDead) return;
    owner.AddCondition<ConCalmDown>(10, true);
    Texts.Say(Texts.ConCharmKilled.RandGet(), Msg.colors.Ono, pc.Name, owner.Name);
    owner.c_originalHostility = owner.OriginalHostility;
  }
  public new void Mod(int a, bool force = false)
  {
    value += a;
    SetPhase();
    OnValueChanged();
  }
}

namespace BetterSuccubus
{
  public static partial class Texts
  {
    public readonly static List<LangGame.Row> ConCharmKilled = [
        new LangGame.Row() { text_JP = "#2の心を包んでいた甘い霧が消えた。", text = "The sweet mist that was hovering over #2's mind faded.", text_L = "笼罩在#2心头的甜雾消去了。"},
            new LangGame.Row() { text_JP = "「ぼ...僕は一体...」", text = "「I...I am...」", text_L = "「我...我这是在...」"},
            new LangGame.Row() { text_JP = "#2首を振り回した。「一体...」", text = "#2 shakes his head. 「What... What am I doing...?」", text_L = "#2晃了晃脑袋。「我...我在做什么...」"},
            new LangGame.Row() { text_JP = "#2は冷や汗をかいて息を荒げている。", text = "#2 is in a cold sweat, breathing heavily.「Ha... Ha...」", text_L = "#2一身冷汗, 大口的喘着粗气「哈...哈...」"},
            new LangGame.Row() { text_JP = "#2が首を振り、驚いて#1を見た。", text = "#2 shakes his head, looking at #1 in surprise.", text_L = "#2摇了摇头, 一脸惊讶的看着#1。"},
        ];
    public readonly static LangGame.Row ConCharmPhaseStr = new() { text = "Enchanted", text_L = "被魅惑", text_JP = "魅了" };

  }

  [HarmonyPatch(typeof(Chara), nameof(Chara.Kick), [typeof(Chara), typeof(bool), typeof(bool), typeof(bool)])]
  internal static class Kick_Patch
  {
    public static void Postfix(Chara __instance, Chara t, bool ignoreSelf = false, bool karmaLoss = true)
    {
      t.RemoveCondition<ConCharm>();
    }
  }

  [HarmonyPatch(typeof(Chara), nameof(Chara.DoHostileAction))]
  internal static class DoHostileAction_Patch
  {
    static bool Prefix(Chara __instance, Card _tg, bool immediate = false)
    {
      if (_tg.HasCondition<ConCharm>())
      {
        if (Settings.AboutHostileAction)
          _tg.Chara.RemoveCondition<ConCharm>();
        return false;
      }
      else return true;
    }
  }

  [HarmonyPatch(typeof(Chara), nameof(Chara.MoveZone), [typeof(Zone), typeof(ZoneTransition)])]
  internal static class MoveZonePatch//离开时删除buff
  {
    static void Postfix(Chara __instance)
    {
      try
      {
        foreach (Chara chara in EClass._map.charas)
          if (chara.HasCondition<ConCharm>())
            chara.RemoveCondition<ConCharm>();
      }
      catch { return; }
    }
  }
  //   [HarmonyPatch(typeof(Chara), nameof(Chara.FindNewEnemy))]
}


