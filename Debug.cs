using System;
using ReflexCLI.Attributes;
namespace BetterSuccubus;

[ConsoleCommandClassCustomizer("")]
public static class BetterSuccubusDebug
{
    [ConsoleCommand("")]
    public static string BetterSuccubusAddAbility()
    {
        EClass.pc.SetFeat(Data.ActCharm.id, 1, true);
        return "YES";
    }
}
