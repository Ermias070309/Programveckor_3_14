using UnityEngine;
using TMPro;
using System.Collections;

public class PuzzleLock : MonoBehaviour
{
    [Header("Puzzle")]
    [SerializeField] private string puzzleID;

    [Header("Requirements (måste vara klara)")]
    [SerializeField] private string[] requiredPuzzles;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI lockedText;
    [SerializeField] private float messageTime = 2f;

    private Interaction interaction;

    private void Awake()
    {
        interaction = GetComponent<Interaction>();

        if (lockedText != null)
            lockedText.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (IsUnlocked())
        {
            // Puzzle är upplåst → Interaction får fungera
            if (interaction != null)
                interaction.enabled = true;
        }
        else
        {
            // Puzzle är låst → stoppa teleport
            if (interaction != null)
                interaction.enabled = false;
        }
    }

    private bool IsUnlocked()
    {
        foreach (string req in requiredPuzzles)
        {
            if (PlayerPrefs.GetInt(req + "_Completed", 0) == 0)
                return false;
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!IsUnlocked())
        {
            ShowLockedMessage();
        }
    }

    private void ShowLockedMessage()
    {
        if (lockedText == null) return;

        StopAllCoroutines();
        lockedText.gameObject.SetActive(true);
        StartCoroutine(HideMessage());
    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(messageTime);
        lockedText.gameObject.SetActive(false);
    }
}