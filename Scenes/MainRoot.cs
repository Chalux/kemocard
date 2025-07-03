using System.IO;
using cfg;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC;
using Luban;

namespace kemocard.Scenes;

public partial class MainRoot : Control
{
    [Export] private AudioStreamPlayer _mainAudioPlayer;
    [Export] public CanvasItem ParentCanvas;
    [Export] public CanvasItem WorldCanvas;
    [Export] public PanelContainer HintPanel;
    [Export] public RichTextLabel HintLabel;
    [Export] public VBoxContainer Banner;

    public override void _EnterTree()
    {
        base._EnterTree();
        GameCore.Instance.Init();

        if (_mainAudioPlayer == null)
        {
            _mainAudioPlayer = new AudioStreamPlayer();
            AddChild(_mainAudioPlayer);
        }

        GameCore.SoundMgr.SetBGMAudioPlayer(_mainAudioPlayer);
        GameCore.SoundMgr.LoadSetting();
        // GameCore.SoundMgr.PlayBgm("menu");
        GameCore.Root = this;

        // 初始化配置表
        const string gameConfDir = "Config/output";
        GameCore.Tables = new Tables(file => new ByteBuf(File.ReadAllBytes($"{gameConfDir}/{file}.bytes")));
        GD.Print("=======加载配置表成功=======");

        //从模块工厂注册模块
        ModuleFactory.RegisterModule();
        
        //工厂准备好后初始化模块
        ModuleFactory.InitModule();

        SaveMgr.LoadGame();

        //准备好后打开菜单界面
        GameCore.ControllerMgr.SendUpdate(ControllerType.GameUIController, CommonEvent.OpenMenuScene);
        StaticUtil.HideHint();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        GameCore.Instance.Update(delta);
        if (HintPanel.Visible)
        {
            var targetGlobalPosition = GetGlobalMousePosition() + new Vector2(15, 15);
            bool flipX = false, flipY = false;
            if ((targetGlobalPosition + HintPanel.Size).X > GetViewportRect().Size.X)
            {
                flipX = true;
            }

            if ((targetGlobalPosition + HintPanel.Size).Y > GetViewportRect().Size.Y)
            {
                flipY = true;
            }

            if (flipX)
            {
                targetGlobalPosition.X = GetViewportRect().Size.X - HintPanel.Size.X;
            }

            if (flipY)
            {
                targetGlobalPosition.Y = GetViewportRect().Size.Y - HintPanel.Size.Y;
            }

            HintPanel.GlobalPosition = targetGlobalPosition;
        }
    }
}