extern alias UnityEngine_CoreModule;
extern alias UnityEngine_Origin;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine_CoreModule.UnityEngine;
using Newtonsoft.Json;

namespace BetterSuccubus;

public class Frame
{
    public int Count { get; set; }
}

public static class TexManager
{
    public static Dictionary<string, Sprite[]> FrameMap = [];
    public static Dictionary<string, Sprite> SpriteMap = [];

    static string texPath;

    static Sprite TextureToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
    static Texture2D LoadTextureByIO(string name)
    {
        byte[] image = File.ReadAllBytes(texPath + name + ".png");

        Texture2D texture = new(0, 0);
        if (texture.LoadImage(image))
            return texture;
        else
        {
            BetterSuccubus.Logger.LogError("Cant read " + name);
            return null;
        }
    }
    static Sprite[] SplitFrames(Texture2D tex, int frameCount)
    {
        List<Sprite> sprites = new List<Sprite>();

        int frameWidth = tex.width / frameCount;

        for (int x = 0; x < frameCount; x++)
        {
            Rect frameRect = new Rect(x * frameWidth, 0, frameWidth, tex.height);
            Sprite frame = Sprite.Create(tex, frameRect, new Vector2(0.5f, 0.5f));
            sprites.Add(frame);
        }

        return sprites.ToArray();
    }
    public static void Add(string name)
    {
        Texture2D tex = LoadTextureByIO(name);

        string jsonPath = texPath + name + ".json";
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(texPath + name + ".json");
            Frame size = JsonConvert.DeserializeObject<Frame>(json);
            Sprite[] frames = SplitFrames(tex, size.Count);
            FrameMap.Add(name, frames);
        }
        else
            SpriteMap.Add(name, TextureToSprite(tex));

        BetterSuccubus.Logger.LogInfo("Add new Sprite :" + name);
    }
    public static void Load()
    {
        texPath = BetterSuccubus.Path + "/Texture/";
        string[] files = Directory.GetFiles(texPath, "*.png", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            Add(name);
        }
    }
}