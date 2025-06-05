using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.MVC.View;

public interface IBaseView
{
    int ViewId { get; set; }

    BaseController Controller { get; set; }
    bool IsInit();

    bool IsShow();

    void InitUI();

    void InitData();

    void DoShow(params object[] args);

    void DoClose(params object[] args);

    void DestroyView();

    void UpdateEvent(string eventName, params object[] args);

    void UpdateControllerEvent(int controllerKey, string eventName, params object[] args);

    void SetVisibility(bool isVisible);
}