using UnityEngine;
using UnityEngine.InputSystem;

public class LockBumper : MonoBehaviour
{
    [Tooltip("잡기(Select)로 바인딩된 InputActionReference를 여기에 드래그하세요.")]
    public InputActionReference selectAction;

    void OnEnable()
    {
        if (selectAction != null && selectAction.action != null)
            selectAction.action.Disable();
    }

    void OnDisable()
    {
        if (selectAction != null && selectAction.action != null)
            selectAction.action.Enable();
    }
}
