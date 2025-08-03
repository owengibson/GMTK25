using UnityEngine;

namespace GMTK25
{
    public interface ICircleable
    {
        void OnCircled();
        Vector2 GetPosition();
        void SetLoopMode(bool isLooping);
    }
}
