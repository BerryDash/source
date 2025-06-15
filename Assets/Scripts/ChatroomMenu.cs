using UnityEngine;
using WebSocketSharp;

public class ChatroomMenu : MonoBehaviour
{
    WebSocket websocket;

    void Start()
    {
        websocket = new WebSocket("ws://localhost:8880");

        websocket.OnMessage += (sender, e) =>
        {
            Debug.Log("message: " + e.Data);
        };
    }
}
