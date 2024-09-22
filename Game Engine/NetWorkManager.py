import paho.mqtt.client as mqtt
import json
# from flask import Flask, request, jsonify
from threading import Thread

topic = "game/players"  

class Network:
    def __init__(self) -> None:
        self.client = mqtt.Client()
        self.client.on_connect =self.on_connect
        self.client.on_message = self.on_message
        self.broker_address = "e88e03e9ab4243079b29075954dd554d.s1.eu.hivemq.cloud"  
        self.broker_port = 1883  

        self.local_sever_port = 8888
        #self.local_server = Flask(__name__)
        self.local_server.route('/update', methods = ['POST'])(self.handle_local_update)
    
    def connect(self):
        self.client.connect(self.broker_address,self.broker_port, 60)
    
    def on_connect(self,client,userdata,flag,rc):
        self.client.subscribe("game/players/#")

    def on_message(self,client,userdata,msg):
        print(f"Received {msg.topic}: {msg.payload}")

    def publish_player_state(self,player):
        topic = f"game/players/{player.player_id}"
        payload =json.dumps(player.to_dict())
        self.client.publish(topic,payload)

    def start_mqtt(self):
        self.client.loop_start()

    def start_local_server(self):
        self.local_server.run(host = "0.0.0.0", port = self.local_server_port)

    def start(self):
        self.connect()
        self.start_mqtt()
        Thread(target = self.start_local_server).start()

    def stop(self):
        self.client.loop_stop()
    


