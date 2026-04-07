using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class CoinCounter : MonoBehaviour
{
    [Header("Dynamic")]
    public int coins = 0;
    private TextMeshProUGUI uiText;
    // Start is called before the first frame update
    void Start()
    {
       uiText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        uiText.text = "Coins: " + coins.ToString("#,0");
    }
}
