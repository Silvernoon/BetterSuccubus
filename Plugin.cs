using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ReflexCLI;

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

    var configFile = Settings.LoadConfiguration();
    /*
    if (Settings.DreamBugTeleport)
    {
        harmony.Unpatch(typeof(ConSleep).GetMethod(nameof(ConSleep.SuccubusVisit)), HarmonyPatchType.All, "BetterSuccubus");
        harmony.Unpatch(typeof(ConSleep).GetMethod(nameof(ConSleep.SuccubusSleep)), HarmonyPatchType.All, "BetterSuccubus");
    }*/

    //var controller = ModOptionController.Register(MyPluginInfo.PLUGIN_GUID, "sivn.bettersuccubus");

    // for Mod Config GUI
    base.GetType().BaseType.GetField("<Config>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, configFile);

    harmony.PatchAll();
    CommandRegistry.assemblies.Add(typeof(BetterSuccubus).Assembly);
  }
  public void OnStartCore()
  {
    Path = System.IO.Path.GetDirectoryName(Info.Location);
    SourceManager sources = Core.Instance.sources;
    //sources.elements.rows.Add(AddSource.ActCharm);
    //ModUtil.ImportExcel(Path + "/Element.xlsx", "Element", sources.elements);
    //ModUtil.ImportExcel(Path + "/stats.xlsx", "stats", sources.stats);
    sources.elements.rows.Add(Initer(Data.ActCharm, sources.elements.rows[0]));

    sources.stats.rows.Add(new SourceStat.Row()
    {
      id = 10069,
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
      id = 10070,
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

    sources.elements.initialized = false;
    sources.stats.initialized = false;
    TexManager.Load();
  }

  public static string Path { get; private set; }

  public T Initer<T>(T target, T tempalte)
  {
    Type t = typeof(T);

    var properties = t.GetFields();
    //.Where(prop => prop.CanRead && prop.CanWrite);
    foreach (var prop in properties)
    {
      var value = prop.GetValue(tempalte);
      if (prop.GetValue(target) == null)
        prop.SetValue(target, value);
    }
    return target;
  }
}