print("Xin chào ThingsBoard")
import paho.mqtt.client as mqttclient
import time
import json
import random

BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
THINGS_BOARD_ACCESS_TOKEN = "BQbJDPdUTFAhUXhn3xvX"


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    try:
        jsonobj = json.loads(message.payload)
        if jsonobj['method'] == "setValue":
            temp_data['value'] = jsonobj['params']
            client.publish('v1/devices/me/attributes', json.dumps(temp_data), 1)
    except:
        pass


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
    else:
        print("Connection is failed")


client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

random.seed(69)

temp = round(random.uniform(0.0, 100.0), 2)
humi = round(random.uniform(0.0, 100.0), 2)

longitude = 106.6297
latitude =  10.8231

counter = 0

PUBLISH_INTERVAL = 10

while True:
    collect_data = {
        'temperature': temp,
        'humidity': humi,
        'longitude': longitude,
        'latitude': latitude
    }
    client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
    temp += round(random.uniform(-2.0, 2.0), 2)
    humi += round(random.uniform(-2.0, 2.0), 2)
    #simulate changing in longitude and latitude with offset
    longitude += round(random.uniform(-0.001, 0.001), 4)
    latitude += round(random.uniform(-0.001, 0.001), 4)

    time.sleep(PUBLISH_INTERVAL)