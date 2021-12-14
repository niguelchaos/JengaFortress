using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUI : MonoBehaviour
{

    public TMP_Text currentPlayerText;
    //private Player player;


    // Start is called before the first frame update
    void Start()
    {
        UpdateText();
    }

    // Update is called once per frame
    void UpdateText()
    {
        currentPlayerText.text = GameManager.Instance.CurrentPlayer.name;
    }

    public void ChangePlayer()
    {
        GameManager.instance.ChangePlayer();
        UpdateText();
    }
}
