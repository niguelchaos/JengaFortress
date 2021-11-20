using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerNum {P1, P2}

// contains data relative to each player
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerNum playerNum;

    public PlayerNum GetPlayerNum() { return playerNum;}
    public void SetPlayerNum(PlayerNum newPlayer) { playerNum = newPlayer;}
}