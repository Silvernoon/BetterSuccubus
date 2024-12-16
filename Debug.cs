using System;
using ReflexCLI.Attributes;
namespace BetterSuccubus;

[ConsoleCommandClassCustomizer("")]
public static class BetterSuccubusDebug
{
    [ConsoleCommand("")]
    public static string BetterSuccubusAddAbility()
    {
        EClass.pc.SetFeat(6030, 1, true);
        return "YES";
    }
}
