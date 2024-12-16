using System;
using System.IO;
using HarmonyLib;
using BepInEx.Logging;
namespace BetterSuccubus;
extern alias UnityEngine_Origin;
extern alias UnityEngine_CoreModule;

using BepInEx.Core.Logging.Interpolation;
using UnityEngine;
using UnityEngine_CoreModule.UnityEngine;
[HarmonyPatch(typeof(SourceElement.Row), nameof(SourceElement.Row.GetSprite))]
internal static class GetSprite_Patch
{
    public static void Postfix(SourceElement.Row __instance, ref Sprite __result)
    {
        if (__result == EClass.core.refs.icons.defaultAbility && Ability.Contains(__instance.type))
            __result = TextureToSprite(LoadTextureByIO(__instance.type)) ?? EClass.core.refs.icons.defaultAbility;
    }
    private static Sprite TextureToSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    private static Texture2D LoadTextureByIO(string type)
    {
        
        FileStream fs = new(BetterSuccubus.Path + "/Texture/" + type + ".png", FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);//游标
        byte[] bytes = new byte[fs.Length];//生命字节，用来存储读取到的图片字节
        fs.Read(bytes, 0, bytes.Length);//开始读取

        fs.Close();//切记关闭
        int width = 64;
        int height = 64;
        Texture2D texture = new(width, height);
        if (texture.LoadImage(bytes))
        {
            //BetterSuccubus.Logger.LogInfo("Readed " + type);
            return texture;
        }
        else
        {
            BetterSuccubus.Logger.LogError("Cant read " + type);
            return null;
        }
    }

    public static string[] Ability = ["ActCharm"];
}
