using System;
using kemocard.Scripts.MVC;
using kemocard.Scripts.MVC.Controller;
using kemocard.Scripts.MVC.Model;

namespace kemocard.Scripts.Module.Loading;

public class LoadingModel(BaseController controller) : BaseModel(inController: controller)
{
    public string SceneName;
    public ViewType ViewType;
    public Action LoadingCallback;
}