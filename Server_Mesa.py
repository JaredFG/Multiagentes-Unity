from CarModel import *
from mesa.visualization.modules import CanvasGrid
from mesa.visualization.ModularVisualization import ModularServer


def agent_portrayal(agent):
    portrayal = {"Shape": "circle",
                 "Filled": "true",
                 "Layer": 0,
                 "r": 0.5}
    if agent.agent_type == 1:
        portrayal["Color"] = "green"
    elif agent.agent_type == 2:
        if agent.direction == (1, 0):
            portrayal["Color"] = "blue"
        elif agent.direction == (-1, 0):
            portrayal["Color"] = "red"
        elif agent.direction == (0, 1):
            portrayal["Color"] = "purple"
        elif agent.direction == (0, -1):
            portrayal["Color"] = "yellow"
        else:
            portrayal["Color"] = "white"
    else:
        portrayal["Color"] = "orange"
    return portrayal

grid = CanvasGrid(agent_portrayal, 20, 20, 500, 500)
server = ModularServer(CarModel,
                       [grid],
                       "CarModel",
                       {"N": 15, "width":20, "height":20})
server.port = 8585 # The default
server.launch()