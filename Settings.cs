using System;

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
    //强而有力口牙！
    public static bool NGPN { get; set; } = false;
}
