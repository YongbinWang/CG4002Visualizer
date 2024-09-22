import random

DEBUG = True

class Action:
    none        = "none"
    shoot       = "gun"
    shield      = "shield"
    bomb        = "bomb"
    reload      = "reload"
    basket      = "basket"
    soccer      = "soccer"
    volley      = "volley"
    bowl        = "bowl"
    logout      = "logout"
    # all actions except none and logout
    all = {shoot, shield, bomb, reload, basket, soccer, volley, bowl}

    num_shoot_total = 7
    _num_AI         = 2

    # shoot is not AI and logout is AI
    num_AI_total = _num_AI * (len(all) - 1) + 1

    @classmethod
    def init_list(cls, _r):
        if _r > 0:
            ret = [cls.shoot]
        else:
            ret = []
        ret.extend([cls.shoot]  * cls.num_shoot_total)
        ret.extend([cls.shield] * cls._num_AI)
        ret.extend([cls.bomb]   * cls._num_AI)
        ret.extend([cls.reload] * cls._num_AI)
        ret.extend([cls.basket] * cls._num_AI)
        ret.extend([cls.soccer] * cls._num_AI)
        ret.extend([cls.volley] * cls._num_AI)
        ret.extend([cls.bowl]   * cls._num_AI)
        random.shuffle(ret)

        ret.append(cls.logout)
        return ret

    @classmethod
    def get_random_action(cls):
        return random.choice(list(cls.all))

    @classmethod
    def actions_match(cls, all_actions_para):
        """
        check if all actions match the Action class
        """
        return len(cls.all.symmetric_difference(all_actions_para)) == 0
