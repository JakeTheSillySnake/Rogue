namespace Domain;
using System.Collections.Generic;

public class Entity {
  public int x, y, hp, hp_max, str, agl, color;
  public string symbol;
  public Entity() {}
  public void assign(int x, int y, int hp, int hp_max, int str, int agl, string s, int c) {
    this.x = x;
    this.y = y;
    this.hp = hp;
    this.hp_max = hp_max;
    this.str = str;
    this.agl = agl;
    this.symbol = s;
    this.color = c;
  }

  // attack() -- calc damage & chance of damage
  // move() -- patterns of movements
}

public class Player : Entity {
    public int lvl = 1;
  public Player() {
    assign(40, 10, 40, 40, 10, 10, ":P", 5);
  }

  // eat food
  // drink elixir
  // read scroll
  // change weapon
  // take inventory
}

public class Zombie : Entity {
  public int enmity = 3;
  public Zombie() {
    assign(8, 6, 40, 40, 7, 5, "Z", 1);
  }
}

public class Vampire : Entity {
  public int enmity = 5;
  public Vampire() {
    assign(10, 10, 40, 40, 10, 7, "V", 2);
  }
}

public class Ogre : Entity {
  public int enmity = 3;
  public Ogre() {
    assign(15, 6, 40, 40, 10, 5, "O", 3);
  }
}

public class Ghost : Entity {
  public int enmity = 2;
  public Ghost() {
    assign(8, 15, 20, 20, 5, 10, "G", 0);
  }
}

public class Snake : Entity {
  public int enmity = 5;
  public Snake() {
    assign(15, 10, 30, 30, 7, 10, "S", 0);
  }
}

/*public class Backpack {
    var potions = new Dictionary<String, Int>(9);
    var scrolls = new Dictionary<String, Int>(9);
    var food = new Dictionary<String, Int>(9);
    public Backpack() {}

    // list inventory
}*/