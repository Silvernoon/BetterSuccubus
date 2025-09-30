extern alias UnityEngine_CoreModule;
extern alias UnityEngine_Origin;
using System.Collections.Generic;
using BetterSuccubus;
using UnityEngine_CoreModule.UnityEngine;

public class ActCharm : Ability
{
  public override Sprite GetSprite() => TexManager.SpriteMap.TryGetValue(this.GetType().Name, EClass.core.refs.icons.defaultAbility);

  public override bool Perform()
  {
    if (TC.Chara.HasCondition<ConCharm>() && CC.ai is not AI_Fuck) Texts.Say("「...」", Msg.colors.Ono);
    else if (TC.Chara.HasCondition<ConCalmDown>()) Texts.Say(Texts.ConCalmDownToFail.RandGet(), Msg.colors.Ono, ref2: TC.Chara.Name);
    else if (TC.Chara == pc) Texts.Say(Texts.Self.RandGet(), Msg.colors.Ono);
    else
    {
      //魅力+技能等级+好感度—对象意志=成功率
      if (EClass.rnd(100) < CC.elements.ValueWithoutLink("CHA") + Value + (TC.Chara.affinity.value > 20 ? TC.Chara.affinity.value / 2 : TC.Chara.affinity.value - 10) - TC.elements.ValueWithoutLink("WIL"))
      {// Succuss
        Effect effectSuccess = Effect.Get("Element/ball_Fire");
        effectSuccess.sprites = TexManager.FrameMap["Charm_Success"];
        effectSuccess.duration = 1.5f;
        effectSuccess.Play(TC.pos);
        Texts.Say(Texts.ActCharmSuccess.RandGet(), Msg.colors.Ono, CC.Name, TC.Name);
        TC.Chara.AddCondition<ConCharm>(50, true);

        foreach (Chara chara in pc.party.members) //同伴仇恨
          if (chara.enemy == owner.Chara && chara.ai is GoalCombat)
          {
            chara.enemy = null;
            chara.SetAI(new NoGoal());
          }
      }
      else
      {// Failure
        Effect effectFailure = Effect.Get("Element/ball_Fire");
        effectFailure.sprites = TexManager.FrameMap["Charm_Failure"];
        effectFailure.duration = 1.5f;
        effectFailure.Play(TC.pos);
        if (TC.Chara.HasCondition<ConCharm>())
          TC.Chara.RemoveCondition<ConCharm>();
        else
        {
          Texts.Say(Texts.ActCharmFailure.RandGet(), Msg.colors.Ono, CC.Name, TC.Name);
          TC.Chara.AddCondition<ConCalmDown>(10, true);
        }

        if (Settings.FailedCharmMakeHostile)
        {
          TC.ShowEmo(Emo.angry, 0f, false);
          TC.Chara.SetEnemy(CC);
        }
        else TC.Chara.ModAffinity(CC, -10);
      }
    }
    TC.PlaySound("pick_thing");
    return true;
  }
  public override Cost GetCost(Chara c)
  {
    Cost result2 = default;
    result2.type = CostType.MP;
    int num = EClass.curve(Value, 50, 10);
    result2.cost = (int)(source.cost[0] * ((100 + num) / 100f));
    return result2;
  }
}

namespace BetterSuccubus
{
  public static partial class Texts
  {
    public readonly static List<LangGame.Row> ActCharmSuccess = [
        new LangGame.Row() { text_JP = "#2は呆けている。", text = "#2 is mesmerized.", text_L = "#2被迷倒了。" },
            new LangGame.Row() { text_JP = "#2は恍惚とした表情であなたを見ている。", text = "#2 looks at you in a trance.", text_L = "#2神情恍惚的看着你。" },
            new LangGame.Row() { text_JP = "「あ...美しい...」", text = "「Ah…Such a beauty…」", text_L = "「啊…真美…」" },
            new LangGame.Row() { text_JP = "抑えきれない愛欲が溢れてくる。", text = "A wave of love that could not be suppressed rushed up to #2's heart.", text_L = "一股无法压抑的爱意冲上了#2的心头。" },
            new LangGame.Row() { text_JP = "「僕、僕はあなたが...」", text = "「I'm . .I'm so...」", text_L = "「我…我对你…」" }
    ];
    public readonly static List<LangGame.Row> ActCharmFailure = [
        new LangGame.Row() { text_JP = "#2は#1を突き放した。", text = "#2 pushed #1 away", text_L = "#2一把推开了#1。"},
            new LangGame.Row() { text_JP = "「なにを！」", text = "「What's wrong with you?」", text_L = "「你做什么！」"},
            new LangGame.Row() { text_JP = "#2は激怒した。「君のこと、見損なった!」", text = "#2 looks at #1 angrily.「I was wrong about you!」", text_L = "#2愤怒的看着#1。「我看错你了!」"},
            new LangGame.Row() { text_JP = "#1は差し伸ばした手を#2に振り払われた。", text = "#2 fends off #1's outstretched hand.", text_L = "#2挡开了#1伸过来的手。"},
            new LangGame.Row() { text_JP = "「近寄らないで！」", text = "「Don't come near me!」", text_L = "「不要靠近我！」"},
        ];
    public readonly static List<LangGame.Row> Self = [
        new LangGame.Row() { text_JP = "#1は可愛らしく髪を撫でつけた。", text = "You stroke your hair coquettishly into the air.", text_L = "你对着空气搔首弄姿。"},
            new LangGame.Row() { text_JP = "あなたは自分の美しさにメロメロだ。", text = "You're about to charm yourself.", text_L = "你快要把自己迷住了。"}
    ];

    public static string Lang(this LangGame.Row row)
    {
      return global::Lang.langCode switch
      {
        "CN" => row.text_L,
        "JP" => row.text_JP,
        _ => row.text,
      };
    }
    public static string RandGet(this List<LangGame.Row> rows)
    {
      return rows[EClass.rnd(rows.Count - 1)].Lang();
    }
    public static void Say(string word, UnityEngine_CoreModule.UnityEngine.Color color, string ref1 = null, string ref2 = null)
    {
      Msg.currentColor = color;
      Msg.Say(word, ref1, ref2);
      Msg.SetColor();
    }
  }
}

namespace BetterSuccubus
{
  public static partial class Data
  {
    public static readonly SourceElement.Row ActCharm = new()
    {
      id = 60030,
      alias = "ActCharm",
      name_JP = "チャーム",
      name = "Charm",
      name_L = "魅惑",
      aliasParent = "CHA",
      parentFactor = 20f,
      lvFactor = 100,
      encFactor = 100,
      mtp = 1,
      LV = 1,
      chance = 1000,
      cost = [10],
      target = "Chara",
      proc = ["Debuff", "ConCharm"],
      type = "ActCharm",
      group = "ABILITY",
      category = "ability",
      categorySub = "ability",
      eleP = 50,
      cooldown = 0,
      charge = 10,
      radius = 5,
      max = 2,
      textExtra_JP = "対象が接近する,エナジードレインが使える",
      textExtra = "The target will approach you,Allow predation",
      textExtra_L = "赋予魅惑效果50回合,目标的意志使效果降低了,目标会靠近你,允许捕食",
      detail = "Unleash your charm to make target fall under your spell, but failure could make it an enemy.",
      detail_JP = "対象を誘惑して虜にする。また失敗すれば敵対する。",
      detail_L = "释放魅力让目标成为你的俘虏任你摆布，但是失败的话则会成为敌人。"
    };
  }
}

