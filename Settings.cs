using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;

namespace BetterSuccubus;

public static class Settings
{
  // Fuck
  public static bool EnableHPDrain { get; set; }
  public static bool EnableMPDrain { get; set; }
  public static bool EnableSTRecover { get; set; }
  public static float DrainScale { get; set; }
  public static bool EnableNoHunger { get; set; }
  public static int HungerValue { get; set; }
  public static bool SexNoNeed { get; set; }
  //public static bool SexNoSleepy { get; set; }
  public static bool SkillImprovement { get; set; }
  public static bool SkillImprovementPotential { get; set; }
  public static int SkillImprovementValue { get; set; }
  public static bool AffinityIncrease { get; set; }
  //public static bool GreaterReward { get; set; }
  //public static int GreaterRewardFactor { get; set; }
  // DreamBug
  public static bool DreamBugTeleport { get; set; }
  public static bool DreamBugMakeSleep { get; set; }
  public static float DreamBugCostScale { get; set; }
  public static bool DreamBugStackable { get; set; }
  //public static bool DreamBugCanBeStolen { get; set; }
  // ActCharm
  public static bool FailedCharmMakeHostile { get; set; }
  // Debug
  public static bool DebugAddAbility { get; set; }
  public static bool AboutHostileAction { get; set; }
  // Kill
  public static bool KillRecoverHP { get; set; }
  public static bool KillRecoverMP { get; set; }
  public static bool KillRecoverSP { get; set; }
  public static int KillBoostAttributes { get; set; }

  // Milk & Egg

  public static bool EnableMNE { get; set;}
  public static Milk MNE_Male_Milk { get; set; } = new();
  public static Egg MNE_Male_Egg { get; set; } = new();
  public static Milk MNE_Female_Milk { get; set; } = new();
  public static Egg MNE_Female_Egg { get; set; } = new();
  public static Milk MNE_NonBin_Milk { get; set; } = new();
  public static Egg MNE_NonBin_Egg { get; set; } = new();

  public static bool MNE_Player_Patch { get; set; }
  public static Milk MNE_Player_Milk { get; set; } = new();
  public static Egg MNE_Player_Egg { get; set; } = new();

  //强而有力口牙！
  public static bool NGPN { get; set; } = false;

  public static ConfigFile LoadConfiguration()
  {
    ConfigFile configFile = new(System.IO.Path.Combine(Paths.ConfigPath, "BetterSuccubus.cfg"), true);

    EnableHPDrain = configFile.Bind("Drain", "EnableHPDrain", true, "Enable HP Drain.\n启用吸取生命").Value;
    EnableMPDrain = configFile.Bind("Drain", "EnableMPDrain", true, "Enable MP Drain.\n启用吸取魔力").Value;
    EnableSTRecover = configFile.Bind("Drain", "EnableSTRecovery", true, "Enable Stamina Recover.\n启用耐力回复").Value;
    DrainScale = configFile.Bind("Drain", "DrainScale", 0.1f, "The maximum of drain of HP and MP equal to Max Stamina * the scale.\nSet to 0.1 means One tenth of Max Stamina.\n吸取倍率，吸取量（随机数）的最大值等于 自己最大耐力*这个值\n0.1就表示最大耐力的十分之一").Value;
    EnableNoHunger = configFile.Bind("Hunger", "EnableNoHunger", true, "Sex would reduct Hunger.\n超人会减少饥饿").Value;
    HungerValue = configFile.Bind("Hunger", "HungerValue", 20, "The Value reduct Hunger.\nEvery 50 of Hunger is a stage.\n减少饥饿的量，每50是一个阶段").Value;
    SexNoNeed = configFile.Bind("Ability", "SexNoNeed", false, "Able to sex even without precondition (Sleep).\n超人无需前置条件（睡眠）").Value;
    //SexNoSleepy = configFile.Bind("Ability", "SexNoSleepy", true, "Sex no longer make you sleepy\n超人不再困倦").Value;
    SkillImprovement = configFile.Bind("Ability", "SkillImprovement", true, "Improve Skill (not Potential) after sex.\n超人提高技能（非潜力）").Value;
    SkillImprovementValue = configFile.Bind("Ability", "SkillImprovementValue", 250, "The value of Skill improvement (not Potential) after sex.\n超人提高技能（非潜力）的数值").Value;
    SkillImprovementPotential = configFile.Bind("Ability", "SkillImprovementPotential", true, "Improve Skill Potential after sex.\n超人提高技能潜力").Value;
    AffinityIncrease = configFile.Bind("Ability", "AffinityIncrease", true, "Affinity is bound to Increase after sex.\n超人必定增加好感").Value;
    //GreaterReward = configFile.Bind("TailReward", "GreaterReward", true, "Make Tail Reward scaled by CHA\n启用爽一爽回报受魅力影响").Value;
    //GreaterRewardFactor = configFile.Bind("TailReward", "DreamBugCostFactor", 50, "The reward ends up adding CHA * Factor\n启用爽一爽受魅力影响的倍率 魅力*该值").Value;

    // DreamBug
    DreamBugTeleport = configFile.Bind("DreamBug", "DreamBugTeleport", true, "Enable DreamBug Teleport.\n启用梦虫传送").Value;
    DreamBugMakeSleep = configFile.Bind("DreamBug", "DreamBugMakeSleep", false, "DreamBug would put a target to sleep.\n启用梦虫催眠").Value;
    DreamBugStackable = configFile.Bind("DreamBug", "DreamBugStackable", true, "Make DreamBug Stackable\n梦虫可堆叠").Value;
    //DreamBugCanBeStolen = configFile.Bind("DreamBug", "DreamBugCanBeStolen", false, "Is DreamBug Can Be Stolen?\n梦虫能否被偷窃").Value;
    DreamBugCostScale = configFile.Bind("DreamBug", "DreamBugCostScale", 0.5f, "DreamBug Cost Scale.\nSet to 0.5 means half the cost of origin\n梦虫耐力消耗倍率").Value;
    // ActCharm
    FailedCharmMakeHostile = configFile.Bind("ActCharm", "FailedCharmMakeHostile", true, "Failed Charm would make target hostile.\n魅惑失败使对方敌对").Value;
    // ConCharm
    AboutHostileAction = configFile.Bind("ConCharm", "AboutHostileAction", true, "if Do Hostile Action break Charm\n敌对行为（攻击等）中断魅惑效果").Value;
    // Debug
    DebugAddAbility = configFile.Bind("Debug", "AddAbility", false, "For old save, enable the option to add ability to succubus.\n启用该选项将给旧存档添加新能力").Value;
    // Kill
    KillRecoverHP = configFile.Bind("Kill", "KillRecoverHP", true, "Kill target being charmed would recover HP.\n超死被魅惑的敌人会回复血量").Value;
    KillRecoverMP = configFile.Bind("Kill", "KillRecoverMP", true, "Kill target being charmed would recover MP.\n超死被魅惑的敌人会回复蓝量").Value;
    KillRecoverSP = configFile.Bind("Kill", "KillRecoverSP", true, "Kill target being charmed would recover SP.\n超死被魅惑的敌人会回复耐力").Value;
    KillBoostAttributes = configFile.Bind("Kill", "KillBoostAttributes", 500, "Kill target being charmed would improve attributes.\n超死被魅惑的敌人提升属性的倍率").Value;

    // Milk & Egg
    
    EnableMNE = configFile.Bind("Milk & Egg", "EnableMNE", false, "Enable Milk & Egg Patch.\n启用产乳和生蛋功能").Value;

    MNE_Male_Milk.chance = configFile.Bind("Milk & Egg", "MaleMilkChance", 0, "Chance of Males producing Milk after sex.\nset to 0 means Males never produce Milk.\nset to 20 means 1/20 chance to produce eggs\n雄性产乳几率。\n设置为 0 意味着禁用雄性产乳。\n设置为 20 意味着20分之1几率雄性产乳。").Value;
    MNE_Male_Egg.chance = configFile.Bind("Milk & Egg", "MaleEggChance", 0, "Chance of Males laying Eggs after sex.\nset to 0 means Males never lay Eggs.\n雄性生蛋几率。\n设置为 0 意味着禁用雄性生蛋。").Value;
    MNE_Male_Egg.fertChance = configFile.Bind("Milk & Egg", "MaleFertEggChance", 20, "Chance of Males' Eggs is Fertilizied.\nset to 0 means Eggs are almost never Fertilizied.\n雄性受精卵几率。\n设置为 0 意味着几乎不会是受精卵。").Value;

    MNE_Female_Milk.chance = configFile.Bind("Milk & Egg", "FemaleMilkChance", 10, "Chance of Females producing Milk after sex.\nset to 0 means Females never produce Milk.\n雌性产乳几率。\n设置为 0 意味着禁用雌性产乳。").Value;
    MNE_Female_Egg.chance = configFile.Bind("Milk & Egg", "FemaleEggChance", 10, "Chance of Females laying Eggs after sex.\nset to 0 means Females never lay Eggs.\n雌性生蛋几率。\n设置为 0 意味着禁用雌性生蛋。").Value;
    MNE_Female_Egg.fertChance = configFile.Bind("Milk & Egg", "FemaleFertEggChance", 20, "Chance of Females' Eggs is Fertilizied.\nset to 0 means Eggs are almost never Fertilizied.\n雌性受精卵几率。\n设置为 0 意味着几乎不会是受精卵。").Value;

    MNE_NonBin_Milk.chance = configFile.Bind("Milk & Egg", "NonBinMilkChance", 0, "Chance of ??? producing Milk after sex.\nset to 0 means ??? never produce Milk.\n？？？性别产乳几率。\n设置为 0 意味着禁用？？？性别产乳。").Value;
    MNE_NonBin_Egg.chance = configFile.Bind("Milk & Egg", "NonBinEggChance", 0, "Chance of ??? laying Eggs after sex.\nset to 0 means ??? never lay Eggs.\n？？？性别生蛋几率。\n设置为 0 意味着禁用？？？性别生蛋。").Value;
    MNE_NonBin_Egg.fertChance = configFile.Bind("Milk & Egg", "NonBinFertEggChance", 20, "Chance of ???'s Eggs is Fertilizied.\nset to 0 means Eggs are almost never Fertilizied.\n？？？性别受精卵几率。\n设置为 0 意味着几乎不会是受精卵。").Value;

    MNE_Player_Patch = configFile.Bind("Milk & Egg", "PlayerMNEOverride", false, "Enable Player Milk & Egg Override.\nset to true means Player ignoring the Above settings.\n启用玩家设置覆盖。\n设置为 true 意味着玩家无视上面的配置。").Value;
    MNE_Player_Milk.chance = configFile.Bind("Milk & Egg", "PlayerMilkChance", 0, "Chance of Player producing Milk after sex.\nset to 0 means Player never produces Milk.\n玩家产乳几率。\n设置为 0 意味着禁用玩家产乳。").Value;
    MNE_Player_Egg.chance = configFile.Bind("Milk & Egg", "PlayerEggChance", 0, "Chance of Player laying Eggs after sex.\nset to 0 means Player never lays Eggs.\n玩家生蛋几率。\n设置为 0 意味着禁用玩家生蛋。").Value;
    MNE_Player_Egg.fertChance = configFile.Bind("Milk & Egg", "PlayerFertEggChance", 20, "Chance of Player's Eggs is Fertilizied.\nset to 0 means Eggs are almost never Fertilizied.\n玩家受精卵几率。\n设置为 0 意味着几乎不会是受精卵。").Value;

    return configFile;
  }

  public class Milk
  {
    public int chance;
  }
  public class Egg
  {
    public int chance;
    public int fertChance;
  }
}
