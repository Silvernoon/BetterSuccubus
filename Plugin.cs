using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.Net.NetworkInformation;
using ReflexCLI;
using System.Reflection;
using System.Linq;
//using EvilMask.Elin.ModOptions;

namespace BetterSuccubus;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class BetterSuccubus : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private void Awake()
    {
        Logger = base.Logger;
        var harmony = new Harmony("BetterSuccubus");

        LoadConfiguration();
        /*
        if (Settings.DreamBugTeleport)
        {
            harmony.Unpatch(typeof(ConSleep).GetMethod(nameof(ConSleep.SuccubusVisit)), HarmonyPatchType.All, "BetterSuccubus");
            harmony.Unpatch(typeof(ConSleep).GetMethod(nameof(ConSleep.SuccubusSleep)), HarmonyPatchType.All, "BetterSuccubus");
        }*/

        //var controller = ModOptionController.Register(MyPluginInfo.PLUGIN_GUID, "sivn.bettersuccubus");

        harmony.PatchAll();
        CommandRegistry.assemblies.Add(typeof(BetterSuccubus).Assembly);
    }
    public void OnStartCore()
    {
        Path = System.IO.Path.GetDirectoryName(Info.Location);
        SourceManager sources = Core.Instance.sources;
        //sources.elements.rows.Add(AddSource.ActCharm);
        BetterSuccubus.Logger.LogError("NM");
        //ModUtil.ImportExcel(Path + "/Element.xlsx", "Element", sources.elements);
        //ModUtil.ImportExcel(Path + "/stats.xlsx", "stats", sources.stats);

        sources.elements.rows.Add(new SourceElement.Row()
        {
            id = 6030,
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
            proc = ["Debuff","ConCharm"],
            type = "ActCharm",
            group = "ABILITY",
            category = "ability",
            categorySub = "ability",
            eleP = 50,
            cooldown = 100,
            charge = 10,
            radius = 5,
            max = 2,
            textExtra_JP = "対象が接近する,エナジードレインが使える",
            textExtra = "The target will approach you,Allow predation",
            textExtra_L = "赋予魅惑效果50回合,目标的意志使效果降低了,目标会靠近你,允许捕食",
            detail = "Unleash your charm to make target fall under your spell, but failure could make it an enemy.",
            detail_JP = "対象を誘惑して虜にする。また失敗すれば敵対する。",
            detail_L = "释放魅力让目标成为你的俘虏任你摆布，但是失败的话则会成为敌人。",
            langAct = []
        });

        sources.stats.rows.Add(new SourceStat.Row()
        {
            id = 1069,
            alias = "ConCharm",
            name_L = "魅惑",
            name_JP = "魅了",
            name = "Enchanted",
            group = "Bad",
            curse = "ConCharm",
            duration = "50",
            hexPower = 0,
            defenseAttb = [],
            resistance = [],
            gainRes = 0,
            phase = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
            colors = "sleep",
            nullify = [],
            effect = [],
            strPhase_JP = [],
            strPhase = [],
            //textPhase_JP = "#1は恐怖に襲われた。",
            //textPhase = "#1 are/is struck with terror.",
            invert = false
        });
        sources.stats.rows.Add(new SourceStat.Row()
        {
            id = 1070,
            alias = "ConCalmDown",
            name_JP = "賢者ターム",
            name = "Calm Down",
            group = "Bad",
            curse = "ConCalmDown",
            duration = "10",
            hexPower = 0,
            defenseAttb = [],
            resistance = [],
            gainRes = 0,
            phase = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9],
            colors = "sleep",
            nullify = [],
            effect = [],
            strPhase_JP = [],
            strPhase = [],
            invert = false
        });
    }
    private void LoadConfiguration()
    {
        ConfigFile configFile = new(System.IO.Path.Combine(Paths.ConfigPath, "BetterSuccubus.cfg"), true);
        Settings.EnableHPDrain = configFile.Bind("Drain", "EnableHPDrain", true, "Enable HP Drain.\n启用吸取生命").Value;
        Settings.EnableMPDrain = configFile.Bind("Drain", "EnableMPDrain", true, "Enable MP Drain.\n启用吸取魔力").Value;
        Settings.EnableSTRecover = configFile.Bind("Drain", "EnableSTRecovery", true, "Enable Stamina Recover.\n启用耐力回复").Value;
        Settings.DrainScale = configFile.Bind("Drain", "DrainScale", 0.1f, "The maximum of drain of HP and MP equal to Max Stamina * the scale.\nSet to 0.1 means One tenth of Max Stamina.\n吸取倍率，吸取量（随机数）的最大值等于 自己最大耐力*这个值\n0.1就表示最大耐力的十分之一").Value;
        Settings.EnableNoHunger = configFile.Bind("Hunger", "EnableNoHunger", true, "Sex would reduct Hunger.\n超人会减少饥饿").Value;
        Settings.HungerValue = configFile.Bind("Hunger", "HungerValue", 20, "The Value reduct Hunger.\nEvery 50 of Hunger is a stage.\n减少饥饿的量，每50是一个阶段").Value;
        Settings.SexNoNeed = configFile.Bind("Ability", "SexNoNeed", false, "Able to sex even without precondition.\n超人无需前置条件").Value;
        //Settings.SexNoSleepy = configFile.Bind("Ability", "SexNoSleepy", true, "Sex no longer make you sleepy\n超人不再困倦").Value;
        Settings.SkillImprovement = configFile.Bind("Ability", "SkillImprovement", true, "Improve Skill (not Potential) after sex.\n超人提高技能（非潜力）").Value;
        Settings.SkillImprovementValue = configFile.Bind("Ability", "SkillImprovementValue", 250, "The value of Skill improvement (not Potential) after sex.\n超人提高技能（非潜力）的数值").Value;
        Settings.SkillImprovementPotential = configFile.Bind("Ability", "SkillImprovementPotential", true, "Improve Skill Potential after sex.\n超人提高技能潜力").Value;
        Settings.AffinityIncrease = configFile.Bind("Ability", "AffinityIncrease", true, "Affinity is bound to Increase after sex.\n超人必定增加好感").Value;
        //Settings.GreaterReward = configFile.Bind("TailReward", "GreaterReward", true, "Make Tail Reward scaled by CHA\n启用爽一爽回报受魅力影响").Value;
        //Settings.GreaterRewardFactor = configFile.Bind("TailReward", "DreamBugCostFactor", 50, "The reward ends up adding CHA * Factor\n启用爽一爽受魅力影响的倍率 魅力*该值").Value;
        //DreamBug
        Settings.DreamBugTeleport = configFile.Bind("DreamBug", "DreamBugTeleport", true, "Enable DreamBug Teleport.\n启用梦虫传送").Value;
        Settings.DreamBugMakeSleep = configFile.Bind("DreamBug", "DreamBugMakeSleep", false, "DreamBug would put a target to sleep.\n启用梦虫催眠").Value;
        Settings.DreamBugStackable = configFile.Bind("DreamBug", "DreamBugStackable", true, "Make DreamBug Stackable\n梦虫可堆叠").Value;
        //Settings.DreamBugCanBeStolen = configFile.Bind("DreamBug", "DreamBugCanBeStolen", false, "Is DreamBug Can Be Stolen?\n梦虫能否被偷窃").Value;
        Settings.DreamBugCostScale = configFile.Bind("DreamBug", "DreamBugCostScale", 0.5f, "DreamBug Cost Scale.\nSet to 0.5 means half the cost of origin\n梦虫耐力消耗倍率").Value;
        //ActCharm
        Settings.FailedCharmMakeHostile = configFile.Bind("ActCharm", "FailedCharmMakeHostile", true, "Failed Charm would make target hostile.\n魅惑失败使对方敌对").Value;
        //ConCharm
        Settings.AboutHostileAction = configFile.Bind("ConCharm", "AboutHostileAction", true, "if Do Hostile Action break Charm\n敌对行为（攻击等）中断魅惑效果").Value;
        //Debug
        Settings.DebugAddAbility = configFile.Bind("Debug", "AddAbility", false, "For old save, enable the option to add ability to succubus.\n启用该选项将给旧存档添加新能力").Value;
        //Kill
        Settings.KillRecoverHP = configFile.Bind("Kill", "KillRecoverHP", true, "Kill target being charmed would recover HP.\n超死被魅惑的敌人会回复血量").Value;
        Settings.KillRecoverMP = configFile.Bind("Kill", "KillRecoverMP", true, "Kill target being charmed would recover MP.\n超死被魅惑的敌人会回复蓝量").Value;
        Settings.KillRecoverSP = configFile.Bind("Kill", "KillRecoverSP", true, "Kill target being charmed would recover SP.\n超死被魅惑的敌人会回复耐力").Value;
        Settings.KillBoostAttributes = configFile.Bind("Kill", "KillBoostAttributes", 500, "Kill target being charmed would improve attributes.\n超死被魅惑的敌人提升属性的倍率").Value;
    }
    public static string Path { get; private set; }
}