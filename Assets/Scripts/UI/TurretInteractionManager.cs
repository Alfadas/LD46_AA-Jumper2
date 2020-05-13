using UnityEngine;

public class TurretInteractionManager : MonoBehaviour
{
    [Tooltip("Selection Menu")]
    [SerializeField] GameObject buttonPanel;

    public bool GetUiStatus()
    {
        return buttonPanel.activeSelf;
    }

    public void OpenTurretInteraction()
    {
        buttonPanel.SetActive(true);
    }

    public void CloseTurretInteraction()
    {
        buttonPanel.SetActive(false);
    }
}
