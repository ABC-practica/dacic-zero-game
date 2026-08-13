using UnityEngine;

namespace Surface
{
    public enum SurfaceType
    {
        Default,
        Dirt,
        Water,
        Concrete,
        Grass,
        Metal,
        Stone
    }

    /*
     * Special component used on game objects to declare their surface types.
     * Mostly used for audio purposes such as different footstep sounds based on the ground type.
     */
    public class SurfaceTypeComponent : MonoBehaviour
    {
        [SerializeField]
        private SurfaceType surfaceType = SurfaceType.Default;
        public SurfaceType Type => surfaceType;
    }
}

