using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public LayerMask playerLayer;
    private GameUIManager uiManager;
    private bool hasWon = false;

    void Start()
    {
        uiManager = FindObjectOfType<GameUIManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0 && !hasWon)
        {
            hasWon = true;
            uiManager.ShowWinScreen();
        }
    }
}