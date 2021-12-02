from  flask import Flask, render_template, request, jsonify
import json, logging, os, atexit
from CarModel import *

app = Flask(__name__, static_url_path='')
model = CarModel(15, 20, 20)

def get_key (elem: tuple) -> int:
    return elem[0]

def get_second(arr: list) -> list:
    return [elem[1] for elem in arr]

port = int(os.getenv('PORT', 8000))

@app.route('/')
def root():
    return ("¡Bienvenidos a nuestro servidor del reto!")

@app.route('/CarAgentsPositions', methods=["GET","POST"])
def getCarPositions():
    cars, car_pos = [], []
    for (agents, x, z) in model.grid.coord_iter():
        if len(agents) > 0:
            for a in agents:
                if a.agent_type == 0:
                    cars.append((a.unique_id, {"x": x, "z": z, "y": 0}))
    cars.sort(key=get_key)
    car_pos = get_second(cars)
    model.step()
    return json.dumps(car_pos)

@app.route('/TrafficLightsAgentsStates', methods=["GET","POST"])
def getLightsStates():
    lights, light_state = [], []
    for (agents, _, _) in model.grid.coord_iter():
        if len(agents) > 0:
            for a in agents:
                if a.agent_type == 1:
                    lights.append((a.unique_id, {"x": int(a.light),"y":0,"z":0}))
    lights.sort(key=get_key)
    light_state = get_second(lights)
    model.step()
    return json.dumps(light_state)

if __name__ == '__main__':
    app.run(host="0.0.0.0", port=port, debug=True)