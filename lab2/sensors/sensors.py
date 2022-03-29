import paho.mqtt.client as mqttclient
import time
import json
import random

BROKER_ADDRESS = "mqttserver.tk"
PORT = 1883
USERNAME = "bkiot"
PASSWORD = "12345678"
CLIENTID = "sensors"
STUDENT_ID = "1852161"


def subscribed(client, userdata, mid, granted_qos):
    #print("Subscribed...")
    pass


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


client = mqttclient.Client(CLIENTID)
client.username_pw_set(USERNAME, PASSWORD)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

random.seed(69)

temp = round(random.uniform(-20.0, 100.0), 2)
humi = round(random.uniform(0.0, 100.0), 2)

counter = 0

PUBLISH_INTERVAL = 1

while True:
    env_data = {
        'temperature': temp,
        'humidity': humi
    }
    client.publish('/bkiot/{}/status'.format(STUDENT_ID), json.dumps(env_data), 2, retain=True)
    temp += round(random.uniform(-2.0, 2.0), 2)
    humi += round(random.uniform(-2.0, 2.0), 2)

    time.sleep(PUBLISH_INTERVAL)