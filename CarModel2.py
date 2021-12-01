'''
Autores:Eduardo Rodríguez López         A01749381                                
        Rebeca Rojas Pérez              A01751192                                           
        Jared Abraham Flores Guarneros  A01379868 		       
        Eduardo Aguilar Chías 			A01749375       
'''
from random import random
from mesa.visualization.modules import CanvasGrid
from mesa.visualization.ModularVisualization import ModularServer
from mesa.batchrunner import BatchRunner
from mesa.datacollection import DataCollector
from mesa.space import MultiGrid
from mesa import Agent , Model
from mesa.time import RandomActivation

#Clase para crear a los agentes automóviles
class CarAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.next_cell = None
        self.direction = None
        self.agent_type = 0

    #Función para validar si la posición es válida, en caso de que sea válida regresa True, en caso contrario
    #regresa False.
    def is_valid(self, position):
        if position[0] < self.model.width and position[1] < self.model.height and position[0] >= 0 and position[1] >= 0:
            if not self.model.grid.is_cell_empty(position):
                return True
        return False

    #Función para recibir las posibles celdas a dónde moverse, regresa la posición de la calle.
    def get_poss_cell(self):
        neighborhood = self.model.grid.get_neighborhood(self.pos, moore=False, include_center=False)
        for cell in neighborhood:
            for agent in self.model.grid.get_cell_list_contents(cell):
                if agent.agent_type == 2:
                    next_dir = (self.pos[0] - agent.pos[0], self.pos[1] - agent.pos[1])
                    if next_dir[0] * -1 != self.direction[0] and next_dir[1] * -1 != self.direction[1]:
                        return agent.pos

    #Función para avanzar hacia el frente, regresa el valor de la variable move que son coordenadas.
    def get_nextcell(self):
        move = (self.pos[0] + self.direction[0], self.pos[1] + self.direction[1])
        return move

    #Función para obtener la dirección hacia donde debe moverse el automóvil, regresa la dirección
    # de la calle.
    def get_nextdirect(self, position):
        for agent in self.model.grid.get_cell_list_contents(position):
            if agent.agent_type == 2:
                return agent.direction

    #Función para dar vuelta, regresa la dirección de la calle.
    def turn(self):
        for cell in self.model.grid.get_neighborhood(self.pos, moore=False, include_center=False):
            for agent in self.model.grid.get_cell_list_contents(cell):
                if agent.agent_type == 2:
                    if agent.direction != self.direction:
                        return agent.direction
        return None
    
    #Función para revisar la luz de los semáforos, regresa la luz del semáforo en caso
    # de que el automóvil tenga uno de vecino. En caso contrario regresa True.
    def check_light(self):
        for agent in self.model.grid.get_cell_list_contents(self.next_cell):
            if agent.agent_type == 1:
                return agent.light
        return True

    #Función para checar si hay otro automovil enfrente, regresa un valor booleano.
    def check_car(self):
        for agent in self.model.grid.get_cell_list_contents(self.next_cell):
            if agent.agent_type == 0:
                return False
        return True

     
    def step(self):
        #Variable para guardar el resultado de la función get_nextcell().
        next_cell = self.get_nextcell()
        #Condición, si la siguiente celda es válida, se guarda en el automóvil y se cambia su dirección.
        if self.is_valid(next_cell):
            self.next_cell = next_cell
            self.direction = self.get_nextdirect(self.next_cell)
       #En caso contrario una varible guarda el resultado de la función turn().
        else:
            direction = self.turn()
            #Condición, si la variable direction es verdadera se cambia la dirección del automóvil.
            if direction:
                self.direction = direction
            #En caso contrario una variable guarda el resultado de la función get_poss_cell().
            #La siguiente celda del automóvil cambia al valor de la variable.
            else:
                poss = self.get_poss_cell()
                self.next_cell = poss
        
        if self.check_car():
            if self.check_light():
                self.model.grid.move_agent(self, self.next_cell)

#Clase para crear a los agentes semáforos.        
class TrafficLightAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.agent_type = 1
        self.light = False

    #Función para cambiar la luz de los semáforos.
    def change(self):
        self.light = not self.light

    #Función para contar el número de automóviles que hay en un semáforo,
    # regresa el contador con el número de automóviles.
    def count_cars(self):
        counter = 0
        neighborhood = self.model.grid.get_neighborhood(self.pos, moore=False, include_center=True)
        for cell in neighborhood:
            for agent in self.model.grid.get_cell_list_contents(cell):
                if agent.agent_type == 0:
                    counter += 1
        return counter

        
#Clase para crear a los agentes calle.               
class StreetAgent(Agent):
    def __init__(self, unique_id, model):
        super().__init__(unique_id, model)
        self.direction = None
        self.agent_type = 2

#Clase para crear el modelo.       
class CarModel(Model):
    def __init__(self, N: int, width: int, height: int):
        self.num_agents = N
        self.running = True
        self.grid = MultiGrid(width, height, False)
        self.schedule = RandomActivation(self)
        self.uids = 0
        self.lights_ids = 0
        self.width = width
        self.height = height
        street_pos = []
        self.lights = 4
        
        #Loop para crear la parte interior de las calles, donde está el cruce.
        for row in range(height):
            for col in range(width):
                agent = StreetAgent(self.uids, self)
                self.uids += 1
                flag = True
                if col > width // 2 - 2 and col < width // 2 + 1 and col > 1 and col < height - 1:
                    if row >= height // 2:
                        agent.direction = (0, 1)                       
                    else:
                        agent.direction = (0, -1)        
                elif row > height // 2 - 2 and row < height // 2 + 1 and row > 1 and row < width - 1:
                    if col > width // 2:
                        agent.direction = (-1, 0)
                    else:
                        agent.direction = (1, 0)
                else: 
                    flag = False
                if flag:
                    self.grid.place_agent(agent, (col, row))
                    street_pos.append((col, row))
        
        #Loop para crear la parte exterior de las calles, donde NO está el cruce.
        for row in range(height):
            for col in range(width):
                agent = StreetAgent(self.uids, self)
                self.uids += 1
                flag = True
                if row < 2:
                    if col < width - 2:
                        agent.direction = (1, 0)
                    else:
                        agent.direction = (0, 1)
                elif row >= 2 and row < height - 2:
                    if col < 2:
                        agent.direction = (0, -1)
                    elif col >= width - 2 and col < width:
                        agent.direction = (0, 1)
                    else:
                        flag = False
                elif row >= height -2 and row < height:
                    if col < width - 2:
                        agent.direction = (-1, 0)
                    else:
                        agent.direction = (0, 1)
                else: 
                    flag = False
                if flag:
                    self.grid.place_agent(agent, (col, row))
                    street_pos.append((col, row))

        #Loop para crear los automóviles en posiciones random donde hay calle.
        for i in range(self.num_agents):
            a = CarAgent(self.uids, self)
            self.uids += 1
            pos_index = self.random.randint(0, len(street_pos) - 1)
            pos = street_pos.pop(pos_index)
            a.direction = self.grid.get_cell_list_contents(pos)[0].direction
            self.grid.place_agent(a, pos)
            self.schedule.add(a)

        #Crear los semáforos
        for i in range(self.lights):
            alight = TrafficLightAgent(self.lights_ids, self)
            self.lights_ids += 1
            self.schedule.add(alight)
            x = 8
            y = 9
            if i == 0:
                alight.light = True
                self.grid.place_agent(alight, (x, y))
            elif i == 1:
                x = 8
                y = 10
                alight.light = True
                self.grid.place_agent(alight, (x, y))
            elif i == 2:
                x = 11
                y = 9
                alight.light = False
                self.grid.place_agent(alight, (x, y))
            else:
                x = 11
                y = 10
                alight.light = False
                self.grid.place_agent(alight, (x, y))               
    
    def step(self):
        #Contadores para saber cuáles semáforos tienen más automóviles.
        count_left = 0
        count_right = 0
        
        #Loop para añadir a los contadores la cantidad de automóviles que hay en cada lado. 
        for agent in self.schedule.agents:
            if agent.agent_type == 1:
                if agent.unique_id == 0:
                    count_left += agent.count_cars()
                elif agent.unique_id == 1:
                    count_left += agent.count_cars()
                elif agent.unique_id == 2:
                    count_right += agent.count_cars()
                elif agent.unique_id == 3:
                    count_right += agent.count_cars()
       
        #Condición, si el lado izquierdo tiene más automóviles, los semáforos del lado izquierdo 
        #dan luz verde y los semáforos del lado derecho dan luz roja.
        if count_left >= count_right:    
            for agent in self.schedule.agents:
                if agent.agent_type == 1:
                    if agent.unique_id == 0:
                        agent.light = True
                    elif agent.unique_id == 1:
                        agent.light = True
                    elif agent.unique_id == 2:
                        agent.light = False
                    else:
                        agent.light = False
        
        #En caso contrario los semáforos del lado derecho dan luz verde y los semáforos del lado
        #izquierdo dan luz roja.
        else:
            for agent in self.schedule.agents:
                if agent.agent_type == 1:
                    if agent.unique_id == 0:
                        agent.light = False
                    elif agent.unique_id == 1:
                        agent.light = False
                    elif agent.unique_id == 2:
                        agent.light = True
                    else:
                        agent.light = True
        self.schedule.step()