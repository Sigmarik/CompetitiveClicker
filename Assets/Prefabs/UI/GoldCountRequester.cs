using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldCountRequester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        text_ = GetComponent<TextMeshProUGUI>();
    }

    int FindGoldCount()
    {
        // TODO: Request the amount of gold of the player.

        return Random.Range(0, 999);
    }

    void Update()
    {
        text_.text = FindGoldCount().ToString();
    }

    private TextMeshProUGUI text_;
}
