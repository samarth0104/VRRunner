using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinText; // Reference to the UI text element

    private int coinCount = 0;

    private void Start()
    {
        UpdateCoinText();
    }

    public void CollectCoin()
    {
        coinCount++;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        coinText.text = coinCount.ToString();
    }
}
