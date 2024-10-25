using Camera;
using UnityEngine;

namespace Levels
{
    public class BorderSetter : MonoBehaviour
    {
        [SerializeField] private Vector3 min;
        [SerializeField] private Vector3 max;

        private void Start()
        {
            CameraMovement.Instance.SetBorders(min, max);
        }
    }
}