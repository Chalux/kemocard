using System.Collections.Generic;
using System.Linq;
using Godot;
using kemocard.Scripts.Common;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.MVC.View;

namespace kemocard.Scripts.MVC;

public class ViewManager
{
    private readonly Dictionary<int, IBaseView> _openedViews = new();
    private readonly Dictionary<int, IBaseView> _viewCache = new();
    private readonly Dictionary<int, ViewInfo> _viewInfo = new();

    public void Register(int key, ViewInfo viewInfo)
    {
        _viewInfo.TryAdd(key, viewInfo);
    }

    public void Register(ViewType key, ViewInfo viewInfo)
    {
        Register((int)key, viewInfo);
    }

    public void Unregister(int key)
    {
        _viewInfo.Remove(key);
    }

    public void RemoveView(int key)
    {
        Unregister(key);
        _viewCache.Remove(key);
        _openedViews.Remove(key);
    }

    public void RemoveViewByController(BaseController inController)
    {
        foreach (var keyValuePair in _viewInfo.Where(keyValuePair => keyValuePair.Value.Controller == inController))
            RemoveView(keyValuePair.Key);
    }

    public bool IsOpen(int key)
    {
        return _openedViews.ContainsKey(key);
    }

    public IBaseView GetView(int key)
    {
        if (_openedViews.TryGetValue(key, out var view)) return view;

        return _viewCache.GetValueOrDefault(key);
    }

    public T GetView<T>(int key) where T : class, IBaseView
    {
        return GetView(key) as T;
    }

    public void DestroyView(int key)
    {
        var view = GetView(key);
        if (view == null) return;
        Unregister(key);
        view.DestroyView();
        _viewCache.Remove(key);
    }

    public void CloseView(ViewType viewType, params object[] args)
    {
        CloseView((int)viewType, args);
    }

    public void CloseView(int key, params object[] args)
    {
        if (!IsOpen(key)) return;
        var view = GetView(key);
        if (view == null) return;
        _openedViews.Remove(key);
        view.DoClose(args);
        view.Controller.CloseView(view);
    }

    public void OpenView(ViewType viewType, params object[] args)
    {
        OpenView((int)viewType, args);
    }

    public void OpenView(int key, params object[] args)
    {
        var view = GetView(key);
        var info = _viewInfo[key];
        if (view == null)
        {
            var type = ((ViewType)key).ToString();
            var scene = GD.Load<PackedScene>(info.ResPath);
            if (scene == null) return;

            var node = scene.Instantiate<Control>();
            // ReSharper disable once SuspiciousTypeConversion.Global
            view = node as IBaseView;
            if (view != null)
            {
                GameCore.Root.ParentCanvas.AddChild(node);
                view.ViewId = key;
                view.Controller = info.Controller;
                _viewCache.Add(key, view);
                info.Controller.OnLoadView(view);
            }
            else
            {
                return;
            }
        }

        if (!_openedViews.TryAdd(key, view)) return;

        if (view.IsInit())
        {
            view.SetVisibility(true);
            view.DoShow(args);
            info.Controller.OpenView(view);
        }
        else
        {
            view.InitUI();
            view.InitData();
            view.DoShow(args);
            info.Controller.OpenView(view);
        }
    }
}

public enum ViewType
{
    MainRoot,
    MenuScene,
    SettingScene,
    AlertView,
    LoadingScene,
    GameScene,
    MapSelectScene,
    BattleView,
    TeamEditView,
    DeckEditView,
    CompendiumScene,
    
}

public struct ViewInfo
{
    public string ViewName;
    public string ResPath;
    public BaseController Controller;
    public ViewType ViewType;
}