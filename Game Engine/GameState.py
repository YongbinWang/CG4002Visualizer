import random
import json
import sys

from Helper import Action


class GameState:
    def __init__(self):
        self.player_1 = Player()
        self.player_2 = Player()

    def __str__(self):
        return str(self.get_dict())

    def get_dict(self):
        data = {'p1': self.player_1.get_dict(), 'p2': self.player_2.get_dict()}
        return data
    
    def get_json(self):
        return json.dumps(self.get_dict())

    def perform_action(self, action, player_id, position_1, position_2):
        """use the user sent action to alter the game state"""
        all_actions = {"gun", "shield", "bomb", "reload", "basket", "soccer", "volley", "bowl"}
        if not Action.actions_match(all_actions):
            print("All actions not handled by GameState.perform_action")
            sys.exit(-1)

        if player_id == 1:
            attacker            = self.player_1
            opponent            = self.player_2
            opponent_position   = position_2
        else:
            attacker            = self.player_2
            opponent            = self.player_1
            opponent_position   = position_1

        can_see = self._can_see (position_1, position_2)
    
        attacker.rain_damage(opponent, opponent_position, can_see)

        attacker.update(action, opponent, can_see)

    def update_from_eval(self, updatedState):
        print(f"updated {updatedState}")
        new_state = json.loads(updatedState)
        print(f"new state {new_state}")
        self.player_1.initialize_from_dict(new_state['p1'])
        self.player_2.initialize_from_dict(new_state['p2'])


    def init_players_random(self):
        """ Helper function to randomize the game state"""
        for player_id in [1, 2]:
            hp = random.randint(10, 90)
            bullets_remaining   = random.randint(0, 6)
            bombs_remaining     = random.randint(0, 2)
            shield_health       = random.randint(0, 30)
            num_unused_shield   = random.randint(0, 3)
            num_deaths          = random.randint(0, 3)

            self._init_player(player_id, bullets_remaining, bombs_remaining, hp,
                              num_deaths, num_unused_shield,
                              shield_health)

    def _init_player (self, player_id, bullets_remaining, bombs_remaining, hp,
                      num_deaths, num_unused_shield, shield_health):
        if player_id == 1:
            player = self.player_1
        else:
            player = self.player_2
        player.set_state(bullets_remaining, bombs_remaining, hp, num_deaths,
                         num_unused_shield, shield_health)


    @staticmethod
    def _can_see(position_1, position_2):
        """check if the players can see each other"""
        can_see = True
        # the players cannot see each other only if one is quadrant 4 and other is in any other quadrant
        if position_1 == 4 and position_2 != 4:
            can_see = False
        elif position_1 != 4 and position_2 == 4:
            can_see = False
        return can_see


class Player:
    def __init__(self):
        self.max_bombs          = 2
        self.max_shields        = 3
        self.hp_bullet          = 5     # the hp reduction for bullet
        self.hp_AI              = 10    # the hp reduction for AI action
        self.hp_bomb            = 5
        self.hp_rain            = 5
        self.max_shield_health  = 30
        self.max_bullets        = 6
        self.max_hp             = 100
        self.ai_actions         = {"soccer","volleyball","basketball","bowling" }

        self.num_deaths         = 0

        self.hp             = self.max_hp
        self.num_bullets    = self.max_bullets
        self.num_bombs      = self.max_bombs
        self.hp_shield      = 0
        self.num_shield     = self.max_shields
        self.action         = 'none'
        self.rain_list = []  # list of quadrants where rain has been started by the bomb of this player


    def __str__(self):
        return str(self.get_dict())

    def get_dict(self):
        data = dict()
        data['hp']              = self.hp
        data['bullets']         = self.num_bullets
        data['bombs']           = self.num_bombs
        data['shield_hp']       = self.hp_shield
        data['deaths']          = self.num_deaths
        data['shields']         = self.num_shield
        data['action']          = self.action
        return data


    def set_state(self, bullets_remaining, bombs_remaining, hp, num_deaths, num_unused_shield, shield_health, action):
        self.hp             = hp
        self.num_bullets    = bullets_remaining
        self.num_bombs      = bombs_remaining
        self.hp_shield      = shield_health
        self.num_shield     = num_unused_shield
        self.num_deaths     = num_deaths
        self.action         = action

    def initialize_from_dict(self, player_dict: dict):
        self.hp             = player_dict['hp']
        self.num_bullets    = player_dict['bullets']
        self.num_bombs      = player_dict['bombs']
        self.hp_shield      = player_dict['shield_hp']
        self.num_shield     = player_dict['shields']
        self.num_deaths     = player_dict['deaths']
        self.action         = player_dict['action']

    def isValidAction(self, action_own):
        if action_own == "shoot":
            return self.num_bullets > 0
        elif action_own == "shield":
            return self.num_shield > 0
        elif action_own == "bomb":
            return self.num_bombs > 0
        elif action_own == "reload":
            return self.num_bullets == 0
        return False
    
    def update(self, action_own , opponent , can_see):
        self.action = action_own
        if action_own == "shoot":
            self.shoot(opponent, can_see)
        elif action_own == "shield":
            self.shield()
        elif action_own == "bomb":
            self.bomb(opponent, opponent.position, can_see)
        elif action_own == "reload":
            self.reload()
        elif action_own in self.ai_actions:
            self.harm_AI(opponent, can_see)
        elif action_own == "logout":
            # has no change in game state
            pass
        else:
            # invalid action we do nothing
            pass
        

        if self.hp == 0:
            self.hp             = self.max_hp
            self.num_bullets    = self.max_bullets
            self.num_bombs      = self.max_bombs
            self.hp_shield      = 0
            self.num_shield     = self.max_shields
            self.action         = 'none'
            self.num_deaths     += 1



    def shoot(self, opponent, can_see):
        while True:
            # check the ammo
            if self.num_bullets <= 0:
                break
            self.num_bullets -= 1

            # check if the opponent is visible
            if not can_see:
                break

            opponent.reduce_health(self.hp_bullet)
            break

    def reduce_health(self, hp_reduction):
        # use the shield to protect the player
        if self.hp_shield > 0:
            new_hp_shield  = max (0, self.hp_shield-hp_reduction)
            # how much should we reduce the HP by?
            hp_reduction   = max (0, hp_reduction-self.hp_shield)
            # update the shield HP
            self.hp_shield = new_hp_shield

        # reduce the player HP
        self.hp = max(0, self.hp - hp_reduction)
        if self.hp == 0:
            # if we die, we spawn immediately
            self.num_deaths += 1

            # initialize all the states
            self.hp             = self.max_hp
            self.num_bullets    = self.max_bullets
            self.num_bombs      = self.max_bombs
            self.hp_shield      = 0
            self.num_shield     = self.max_shields

    def shield(self):
        """Activate shield"""
        while True:
            if self.num_shield <= 0:
                # check the number of shields available
                break
            elif self.hp_shield > 0:
                # check if shield is already active
                break
            self.hp_shield = self.max_shield_health
            self.num_shield -= 1

    def bomb(self, opponent, opponent_position, can_see):
        """Throw a bomb at opponent"""
        while True:
            # check the ammo
            if self.num_bombs <= 0:
                break
            self.num_bombs -= 1

            # check if the opponent is visible
            if not can_see:
                # this bomb will not start a rain and hence has no effect with respect to gameplay
                break

            opponent.reduce_health(self.hp_bomb)
            # start a rain in the quadrant of the opponent
            self.rain_list.append(opponent_position)
            break

    def rain_damage(self, opponent, opponent_position, can_see):
        """
        whenever an opponent walks into a quadrant we need to reduce the health
        based on the number of rains
        """
        if can_see:
            for p in self.rain_list:
                if p == opponent_position:
                    opponent.reduce_health(self.hp_rain)

    def harm_AI(self, opponent, can_see):
        """ We can harm am opponent based on our AI action if we can see them"""
        if can_see:
            opponent.reduce_health(self.hp_AI)

    def reload(self):
        """ perform reload only if the magazine is empty"""
        if self.num_bullets <= 0:
            self.num_bullets = self.max_bullets
