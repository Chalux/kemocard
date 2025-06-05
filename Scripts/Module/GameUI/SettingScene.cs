using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scripts.Module.GameUI;

public partial class SettingScene : BaseView
{
    [Export] private Button _bgmMute;
    [Export] private HSlider _bgmProg;
    [Export] private Button _closeButton;
    [Export] private Button _masterMute;
    [Export] private HSlider _masterProg;
    [Export] private Texture2D _muteIcon;
    [Export] private Button _sfxMute;
    [Export] private HSlider _sfxProg;
    [Export] private Texture2D _unmuteIcon;

    public override void DoShow(params object[] args)
    {
        _closeButton.Pressed += OnCloseButtonOnPressed;
        _masterMute.Pressed += OnMasterMuteOnPressed;
        _bgmMute.Pressed += OnBGMMuteOnPressed;
        _sfxMute.Pressed += OnSFXMuteOnPressed;
        _masterProg.ValueChanged += OnMasterProgOnValueChanged;
        _bgmProg.ValueChanged += OnBGMProgOnValueChanged;
        _sfxProg.ValueChanged += OnSFXProgOnValueChanged;
        _masterProg.DragEnded += DragEnd;
        _bgmProg.DragEnded += DragEnd;
        _sfxProg.DragEnded += DragEnd;

        UpdateScene();
    }

    private void OnCloseButtonOnPressed()
    {
        GameCore.ViewMgr.CloseView(ViewId);
    }

    private void OnMasterMuteOnPressed()
    {
        GameCore.SoundMgr.SoundMuteFlag ^= SoundBus.Master;
        UpdateScene();
    }

    private void OnSFXMuteOnPressed()
    {
        GameCore.SoundMgr.SoundMuteFlag ^= SoundBus.SFX;
        UpdateScene();
    }

    private void OnBGMMuteOnPressed()
    {
        GameCore.SoundMgr.SoundMuteFlag ^= SoundBus.BGM;
        UpdateScene();
    }

    private void OnSFXProgOnValueChanged(double value)
    {
        GameCore.SoundMgr.SetBusVolume(SoundBus.SFX, (uint)value, false);
    }

    private void OnBGMProgOnValueChanged(double value)
    {
        GameCore.SoundMgr.SetBusVolume(SoundBus.BGM, (uint)value, false);
    }

    private void OnMasterProgOnValueChanged(double value)
    {
        GameCore.SoundMgr.SetBusVolume(SoundBus.Master, (uint)value, false);
    }

    private void DragEnd(bool valueChanged)
    {
        if (valueChanged) GameCore.SoundMgr.SaveSetting();
    }

    private void UpdateScene()
    {
        for (var i = 0; i < SoundBus.Max; i++)
            _masterMute.SetButtonIcon((GameCore.SoundMgr.SoundMuteFlag & (1 << i)) > 0 ? _muteIcon : _unmuteIcon);

        _masterProg.SetValueNoSignal(GameCore.SoundMgr.GetBusVolume(SoundBus.Master));
        _bgmProg.SetValueNoSignal(GameCore.SoundMgr.GetBusVolume(SoundBus.BGM));
        _sfxProg.SetValueNoSignal(GameCore.SoundMgr.GetBusVolume(SoundBus.SFX));
    }

    public override void DoClose(params object[] args)
    {
        base.DoClose(args);
        GameCore.SoundMgr.SaveSetting();

        _closeButton.Pressed -= OnCloseButtonOnPressed;
        _masterMute.Pressed -= OnMasterMuteOnPressed;
        _bgmMute.Pressed -= OnBGMMuteOnPressed;
        _sfxMute.Pressed -= OnSFXMuteOnPressed;
        _masterProg.ValueChanged -= OnMasterProgOnValueChanged;
        _bgmProg.ValueChanged -= OnBGMProgOnValueChanged;
        _sfxProg.ValueChanged -= OnSFXProgOnValueChanged;
        _masterProg.DragEnded -= DragEnd;
        _bgmProg.DragEnded -= DragEnd;
        _sfxProg.DragEnded -= DragEnd;
    }
}