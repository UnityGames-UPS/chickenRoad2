using TMPro;
using UnityEngine;

public class HistoryPrefab : MonoBehaviour
{
    [SerializeField] TMP_Text dateText;
    [SerializeField] TMP_Text betText;
    [SerializeField] TMP_Text MultiplyerText;
    [SerializeField] TMP_Text winText;



    internal void SetData(string date, string bet, string multiplyr, string win = "-")
    {
        dateText.text = date;
        betText.text = bet;
        MultiplyerText.text = multiplyr;
        winText.text = win;
    }
}
