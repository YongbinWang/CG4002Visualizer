using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;
using System.Text;

public class NetworkManager : MonoBehaviour
{
    private MqttClient client;
    public string brokerAddress = "e88e03e9ab4243079b29075954dd554d.s1.eu.hivemq.cloud";  // Replace with your broker's address
    public int brokerPort = 1883;  // Default MQTT port
    public string topic = "game/state";  // The topic to subscribe to

    void Start()
    {
        ConnectToBroker();
    }

    private void ConnectToBroker()
    {
        // Create a new MQTT client instance
        client = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);

        // Register to the message received event
        client.MqttMsgPublishReceived += OnMessageReceived;

        // Connect to the broker with a unique client ID
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        // Subscribe to the topic
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

        Debug.Log("Connected to MQTT broker and subscribed to topic: " + topic);
    }

    private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
    {
        // Decode the message payload
        string message = Encoding.UTF8.GetString(e.Message);

        // Log the received message
        Debug.Log("Received message: " + message);

        // Process the JSON message
        ProcessGameState(message);
    }

    private void ProcessGameState(string json)
    {
        // Assuming you have a JSON structure representing game state
        // Use Unity's JsonUtility or any JSON parsing library (e.g., Newtonsoft.Json)
        GameState gameState = JsonUtility.FromJson<GameState>(json);

        // Handle the game state update (e.g., update game objects, UI, etc.)
        UpdateGameState(gameState);
    }

    private void UpdateGameState(GameState gameState)
    {
        // Implement game state handling logic here
        // For example, updating the positions of objects, player stats, etc.
        Debug.Log("Updated game state: " + gameState);
    }

    void OnDestroy()
    {
        // Clean up and disconnect from the MQTT broker
        if (client != null && client.IsConnected)
        {
            client.Disconnect();
            Debug.Log("Disconnected from MQTT broker.");
        }
    }

    // Example GameState class for JSON parsing
    [Serializable]
    public class GameState
    {
        public string playerName;
        public int score;
        public Vector3 playerPosition;
        // Add more fields as per your game's state requirements
    }
}

