using kemocard.Scripts.MVC.Controller;

namespace kemocard.Scripts.MVC.Model;

public class BaseModel(BaseController inController)
{
    public BaseController Controller = inController;

    public virtual void Init()
    {
    }
}