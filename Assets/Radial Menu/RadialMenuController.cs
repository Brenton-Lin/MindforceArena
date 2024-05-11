using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private RadialMenu radialMenu;

    [SerializeField] private InputActionReference activateRadialMenuButton;
    [SerializeField] private InputActionReference setJostickAngleAxis; // This one is Vector2

    private bool isEnabled = false;

    private void Start()
    {
        HideRadialMenu();
    }

    private void ShowRadialMenu()
    {
        radialMenu.transform.parent.gameObject.SetActive(true);
        isEnabled = true;
    }

    private void HideRadialMenu()
    {
        radialMenu.transform.parent.gameObject.SetActive(false);
        isEnabled = false;
    }

    private void ToggleRadialMenu()
    {
        if (isEnabled)
        {
            HideRadialMenu();
        }
        else
        {
            ShowRadialMenu();
        }
    }

    private void SetRadialMenuAngle()
    {
        if (isEnabled)
        {
            radialMenu.isSelecting = true;
            Vector2 angle2 = setJostickAngleAxis.action.ReadValue<Vector2>();

            if (angle2.y == 0 && angle2.x == 0)
            {
                radialMenu.isSelecting = false;
                return;
            }

            print($"angle2: Y: {angle2.y}, X: {angle2.x}");

            // Adjust the angle calculation:
            float angle = Mathf.Atan2(angle2.x, angle2.y) * Mathf.Rad2Deg;
            if (angle < 0)
            {
                angle += 360; // Normalize angle to be within 0-360 degrees range
            }
            radialMenu.angle = angle;
        }
    }

    private void Update()
    {
        if (activateRadialMenuButton.action.WasReleasedThisFrame())
        {
            ToggleRadialMenu();
            Debug.Log("Radial Menu Toggled");
        }

        SetRadialMenuAngle();
    }
}