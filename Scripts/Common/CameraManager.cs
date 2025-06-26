using Godot;

namespace kemocard.Scripts.Common;

public class CameraManager
{
    private KemoCamera _activatedCamera;

    // 上一次的位置
    private Vector2 _prePos;

    public void SetCamera(KemoCamera inCamera)
    {
        _activatedCamera = inCamera;
    }

    public void ResetPos()
    {
        if (_activatedCamera != null)
        {
            _prePos = _activatedCamera.Position;
            _activatedCamera.Position = _activatedCamera.OriginGlobalPos;
        }
    }

    public void SetPos(Vector2 inPos)
    {
        if (_activatedCamera != null) _prePos = _activatedCamera.GetPosition();
        _activatedCamera?.SetPosition(inPos);
    }
}