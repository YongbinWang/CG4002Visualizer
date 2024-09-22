import paho.mqtt.client as mqtt
import json

# Define the MQTT broker settings
BROKER = '47.129.133.165'  # Change to your broker IP or hostname if needed
PORT = 1883           # Default MQTT port
TOPIC = 'game/state'  # Topic to subscribe and publish to
data_to_send = { 
    "player_id": 1, 
    "action": "action_input",
    "game_state": {
      "p1": {
            "hp": 100,
            "bullets": 5,
            "bombs": 3,
            "shield_hp": 30,
            "deaths": 0,
            "shields": 3
        },
        "p2": {
            "hp": 90,
            "bullets": 4,
            "bombs": 2,
            "shield_hp": 20,
            "deaths": 1,
            "shields": 2
        }
    }
}
# Define the message to publish
MESSAGE = "Hello from MQTT test script!"

# Callback function when connected to the broker
def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Connected successfully to broker.")
        # Subscribe to the topic
        client.subscribe(TOPIC)
    else:
        print(f"Connection failed with code {rc}")

# Callback function when a message is received
def on_message(client, userdata, msg):
    print(f"Message received: Topic={msg.topic}, Message={msg.payload.decode()}")

# Callback function for publishing a message
def on_publish(client, userdata, mid):
    print(f"Message published with mid={mid}")

# Create a new MQTT client instance
client = mqtt.Client()

# Attach the callback functions
client.on_connect = on_connect
client.on_message = on_message
client.on_publish = on_publish

client.username_pw_set("yongbinwang", "!Wyb2123425")
# Connect to the MQTT broker
client.connect(BROKER, PORT, 60)

# Start the MQTT client loop in a non-blocking manner
client.loop_start()

# Publish a message to the topic
result = client.publish(TOPIC, json.dumps(data_to_send))
if result.rc == mqtt.MQTT_ERR_SUCCESS:
    print(f"Published message: {json.dumps(data_to_send)}")
else:
    print("Failed to publish message")

# Keep the script running to receive messages
try:
    while True:
        pass
except KeyboardInterrupt:
    print("Disconnecting from broker...")
    client.disconnect()
    client.loop_stop()
