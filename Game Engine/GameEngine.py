import random
import json
import time
import threading
import sys
import paho.mqtt.client as mqtt

from GameState import GameState
from NetWorkManager import Network
from Helper import Action
lock = threading.Lock()
game_state = GameState()
isReceived = False
print = lambda x: sys.stdout.write("%s\n" % x)

BROKER = '47.129.133.165'  
PORT = 1883
TOPIC = 'game/state'      
# Create a new MQTT client instance
client = mqtt.Client()
# Callback function when connected to the broker
def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Connected successfully to broker.")
        # Subscribe to the topic
        client.subscribe(TOPIC)
    else:
        print(f"Connection failed with code {rc}")

# Callback function when a message is received
#def on_message(client, userdata, msg):
    #print(f"Message received: Topic={msg.topic}, Message={msg.payload.decode()}")

# Callback function for publishing a message
def on_publish(client, userdata, mid):
    print(f"Message published with mid={mid}")

# Attach the callback functions
client.on_connect = on_connect
#client.on_message = on_message
client.on_publish = on_publish

client.username_pw_set("yongbinwang", "!Wyb2123425")
# Connect to the MQTT broker
client.connect(BROKER, PORT, 60)

# Start the MQTT client loop in a non-blocking manner
client.loop_start()     

def mqttPublisher():
    global isReceived
    global game_state
    while True:
        if isReceived:
            lock.acquire()
            state = game_state.get_dict()
            complete_state = {
                "player_id" : 1,
                "action" : "action_input",
                "game_state" : state
            }
            result = client.publish(TOPIC, json.dumps(complete_state))
            lock.release()
            isReceived = False



def dataHandler():
    global game_state
    global isReceived
    while True:
        payload = input("Type your action and data like action-data:")
        action_data = payload.split(sep="-")
        lock.acquire()
        if action_data[0] == "action":
            print(game_state.get_dict())
            game_state.perform_action(action_data[1], 1 ,1,1)
            print(game_state.get_dict())
        elif action_data[0] == "eval_update":
            print(game_state.get_dict())
            game_state.update_from_eval(action_data[1])
            print(game_state.get_dict())
        isReceived = True
        lock.release()
publisher = threading.Thread(target=mqttPublisher)
handler = threading.Thread(target=dataHandler)

publisher.start()
handler.start()
   


