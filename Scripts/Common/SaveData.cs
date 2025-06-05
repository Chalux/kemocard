using Godot;

namespace kemocard.Scripts.Common;

public partial class SaveData : GodotObject
{
    public Godot.Collections.Dictionary<string, bool> GlobalBoolSave { get; set; } = new();
    public Godot.Collections.Dictionary<string, double> GlobalNumberSave { get; set; } = new();
    public Godot.Collections.Dictionary<string, string> GlobalStringSave { get; set; } = new();
}

public static class SaveMgr
{
    private const string SaveFilePath = "user://save.sav";
    public static SaveData CurrentSave;

    public static void SaveGame()
    {
        FileAccess file = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Write);
        file.StoreString(Json.Stringify(CurrentSave));
    }

    public static void LoadGame()
    {
        FileAccess file = FileAccess.Open(SaveFilePath, FileAccess.ModeFlags.Read);
        CurrentSave = file == null ? new SaveData() : Json.ParseString(file.GetAsText()).As<SaveData>();
    }
}