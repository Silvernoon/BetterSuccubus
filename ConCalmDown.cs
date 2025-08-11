using System;
using System.Collections.Generic;
using BetterSuccubus;

public class ConCalmDown : BadCondition
{
  public override void SetOwner(Chara _owner, bool onDeserialize = false)
  {
    owner = _owner;
    elements = new ElementContainer();
    elements.SetParent(owner);
  }
  public override Emo2 EmoIcon => Emo2.speeing;
  public override string GetPhaseStr() => BetterSuccubus.Texts.ConCalmDownPhaseStr.Lang();
  public override void Tick() => Mod(-1);
  public override void OnRemoved() {}
}

namespace BetterSuccubus
{
  public static partial class Texts
  {
    public readonly static LangGame.Row ConCalmDownPhaseStr = new() { text = "Calm Down", text_L = "贤者时间", text_JP = "賢者タイム" };
    public readonly static List<LangGame.Row> ConCalmDownToFail = [new() { text_JP = "#2番は警戒している。", text = "#2 studies you warily.", text_L = "#2警惕地看着你。" }];
  }
}
