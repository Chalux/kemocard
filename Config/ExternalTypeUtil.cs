using System.Numerics;
using cfg;

namespace kemocard.Config;

public static class ExternalTypeUtil
{
    public static Vector2 NewVector2(vector2 v)
    {
        return new Vector2(v.X, v.Y);
    }

    public static Vector3 NewVector3(vector3 v)
    {
        return new Vector3(v.X, v.Y, v.Z);
    }

    public static Vector4 NewVector4(vector4 v)
    {
        return new Vector4(v.X, v.Y, v.Z, v.W);
    }
}