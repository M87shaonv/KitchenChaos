using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt, //朝向相机
        CameraForward, //始终向前
    }

    [SerializeField] private Mode mode = Mode.LookAt;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.CameraForward:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}