using System.Collections.Generic;
using Godot;

namespace kemocard.Scripts.Common;

public class SoundManager
{
    private readonly Dictionary<string, AudioStream> _soundDictionary = new();
    private AudioStreamPlayer _bgmAudioStreamPlayer;
    private uint _soundMuteFlag;

    private readonly Godot.Collections.Dictionary<int, uint> _volumeDict = new();

    public uint SoundMuteFlag
    {
        get => _soundMuteFlag;
        set
        {
            // var flip = _soundMuteFlag ^ value;
            _soundMuteFlag = value;
            AudioServer.SetBusMute(SoundBus.Master, (_soundMuteFlag & SoundBus.Master) > 0);
            if ((_soundMuteFlag & SoundBus.BGM) > 0)
            {
                _bgmAudioStreamPlayer.SetStreamPaused(true);
                AudioServer.SetBusMute(SoundBus.BGM, true);
            }
            else
            {
                _bgmAudioStreamPlayer.SetStreamPaused(false);
                AudioServer.SetBusMute(SoundBus.BGM, false);
            }

            AudioServer.SetBusMute(SoundBus.SFX, (_soundMuteFlag & SoundBus.SFX) > 0);

            //给翻转了状态的总线做一个渐入渐出的效果,先放着
            // var tween = _bgmAudioStreamPlayer.CreateTween();
            // tween.TweenMethod(Callable.From((float volume) =>
            // {
            //     var flipStr = Convert.ToString(flip, 2);
            //     for (var i = 0; i < flipStr.Length; i++)
            //     {
            //         if (flipStr[i] == '1')
            //         {
            //             AudioServer.SetBusVolumeDb(i, volume);
            //         }
            //         else
            //         {
            //             AudioServer.SetBusVolumeDb(i, 1 - volume);
            //         }
            //     }
            // }), 0, 1, .5f);
        }
    }

    public void SetBusVolume(int busIdx, uint volumePercent, bool saveSetting = true)
    {
        if (!_volumeDict.TryAdd(busIdx, volumePercent)) _volumeDict[busIdx] = volumePercent;

        AudioServer.SetBusVolumeLinear(busIdx, (float)(volumePercent / 100.0));

        if (saveSetting) SaveSetting();
    }

    public uint GetBusVolume(int busIdx)
    {
        return _volumeDict.TryGetValue(busIdx, out var value) ? value : 0;
    }

    public void SetBGMAudioPlayer(AudioStreamPlayer inAudioStreamPlayer)
    {
        _bgmAudioStreamPlayer = inAudioStreamPlayer;
    }

    public void PlayBgm(string res)
    {
        if ((SoundMuteFlag & SoundBus.BGM) > 0) return;

        AudioStream stream;
        if (!_soundDictionary.TryGetValue(res, out var value))
        {
            stream = ResourceLoader.Load<AudioStream>($"res://Assets/Sound/{res}");
            if (stream == null) return;
            _soundDictionary.Add(res, stream);
        }
        else
        {
            stream = value;
        }

        _bgmAudioStreamPlayer.SetStream(stream);
        _bgmAudioStreamPlayer.Play();
    }

    public void SaveSetting()
    {
        using var saveFile = FileAccess.Open("user://VolumeSetting.conf", FileAccess.ModeFlags.Write);

        if (saveFile == null) return;

        var jsonString = Json.Stringify(_volumeDict);

        saveFile.StoreString(jsonString);

        using var muteFile = FileAccess.Open("user://MuteSetting.conf", FileAccess.ModeFlags.Write);

        muteFile?.Store32(SoundMuteFlag);
    }

    public void LoadSetting()
    {
        using var loadFile = FileAccess.Open("user://VolumeSetting.conf", FileAccess.ModeFlags.Read);
        if (loadFile == null)
        {
            SetBusVolume(SoundBus.Master, 100, false);
            SetBusVolume(SoundBus.BGM, 100, false);
            SetBusVolume(SoundBus.SFX, 100, false);
            using var saveFile = FileAccess.Open("user://VolumeSetting.conf", FileAccess.ModeFlags.Write);
            saveFile?.StoreString(Json.Stringify(_volumeDict));
            return;
        }

        var jsonString = loadFile.GetAsText();
        var temp = Json.ParseString(jsonString).AsGodotDictionary<int, uint>();
        foreach (var keyValuePair in temp)
        {
            SetBusVolume(keyValuePair.Key, keyValuePair.Value);
        }

        using var muteFile = FileAccess.Open("user://MuteSetting.conf", FileAccess.ModeFlags.Read);
        SoundMuteFlag = muteFile?.Get32() ?? 0;
    }
}

public static class SoundBus
{
    public const int Master = 0;
    public const int BGM = 1;
    public const int SFX = 2;
    public const int Max = 3;
}