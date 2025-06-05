using System;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scripts.Module.GameUI;

public class AlertViewData
{
    public string Message;
    public Action agreeCallback = null;
    public Action cancelCallback = null;
}

public partial class AlertView : BaseView
{
    public static void ShowAlert(AlertViewData data)
    {
        GameCore.ControllerMgr.SendUpdate(ControllerType.GameUIController, CommonEvent.OpenAlertView, data);
    }

    [Export] private Label _messageLabel;
    [Export] private Button _agreeBtn;
    [Export] private Button _cancelBtn;
    [Export] private ColorRect _bgColorRect;

    private AlertViewData _data;

    public override void DoShow(params object[] args)
    {
        base.DoShow(args);
        if (args[0] is not AlertViewData data) return;
        _messageLabel.SetText(data.Message ?? "");
        _bgColorRect.SetSize(new Vector2(_messageLabel.Size.X, _messageLabel.Size.Y));
        _bgColorRect.SetAnchorsAndOffsetsPreset(LayoutPreset.HcenterWide);
        _data = data;
        _agreeBtn.Pressed += AgreeBtnOnPressed;
        _cancelBtn.Pressed += CancelBtnOnPressed;
    }

    private void CancelBtnOnPressed()
    {
        _data?.cancelCallback?.Invoke();
        GameCore.ViewMgr.CloseView(ViewType.AlertView);
    }

    private void AgreeBtnOnPressed()
    {
        _data?.agreeCallback?.Invoke();
    }

    public override void DoClose(params object[] args)
    {
        _agreeBtn.Pressed -= AgreeBtnOnPressed;
        _cancelBtn.Pressed -= CancelBtnOnPressed;
        _data = null;
        base.DoClose(args);
    }
}