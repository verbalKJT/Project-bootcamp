using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomData : MonoBehaviour
{
    [HideInInspector] 
    public string roomName="";
    [HideInInspector]
    public int connectPlayer = 0;
    [HideInInspector]
    public int maxPlayer = 0;

    public TMP_Text textRoomName; 
    public TMP_Text textConnectInfo; 
    public void DispRoomData()
    {
        textRoomName.text = roomName; 
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayer.ToString() + ")";
    }
}