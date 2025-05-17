namespace rogue.Domain.Enemies;

using rogue.Domain.LevelMap;

enum Enemies { GHOST = 0, ZOMBIE, SNAKE, MIMIC, VAMPIRE, OGRE }

public abstract class Enemy : Entity {
  public int Hostility { get; set; }
  public bool Follow { get; set; } = false;
  public bool Dead { get; set; } = false;
  public int floor = 0;

  public Enemy() {}

  public abstract void Move(Level lvl);

  public virtual void ChangeSymbol() {}

  public override bool CheckRight(Level lvl, int dist) {
    if (lvl.field[PosY, PosX + dist] < (int)MapCellStates.EXIT ||
        lvl.field[PosY, PosX + dist] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public override bool CheckLeft(Level lvl, int dist) {
    if (lvl.field[PosY, PosX - dist] < (int)MapCellStates.EXIT ||
        lvl.field[PosY, PosX - dist] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public override bool CheckUp(Level lvl, int dist) {
    if (lvl.field[PosY - dist, PosX] < (int)MapCellStates.EXIT ||
        lvl.field[PosY - dist, PosX] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public override bool CheckDown(Level lvl, int dist) {
    if (lvl.field[PosY + dist, PosX] < (int)MapCellStates.EXIT ||
        lvl.field[PosY + dist, PosX] >= Level.itemCode)
      return true;
    else
      return false;
  }

  public virtual int Attack(Player p) {
    int chance;
    Random rnd = new();
    if (Agl < p.Agl)
      chance = 40;
    else if (Agl == p.Agl)
      chance = 60;
    else
      chance = 80;
    int hitOrMiss = rnd.Next(101);
    if (hitOrMiss <= chance)
      return Str;  // hit
    else
      return 0;  // miss
  }

  public void FollowPlayer(Player p, Level lvl) {
    ChangeSymbol();
    int initX = PosX, initY = PosY;
    Follow = true;
    if (p.PosX > PosX && (p.PosY != PosY || p.PosX - 1 != PosX) && CheckRight(lvl, 1))
      PosX++;
    if (p.PosX < PosX && (p.PosY != PosY || p.PosX + 1 != PosX) && CheckLeft(lvl, 1))
      PosX--;
    if (p.PosY > PosY && (p.PosX != PosX || p.PosY - 1 != PosY) && CheckDown(lvl, 1))
      PosY++;
    if (p.PosY < PosY && (p.PosX != PosX || p.PosY + 1 != PosY) && CheckUp(lvl, 1))
      PosY--;
    if (PosX == initX && PosY == initY) {
      // ignore player if no path exists
      Follow = false;
    }
  }

  public virtual int Act(Level lvl, Player p) {
    if (Asleep || Dead) {
      Asleep = false;
      return 0;
    }
    int dist = DistanceToTarget(p.PosX, p.PosY), damage = 0;
    if (dist <= 1) {
      ChangeSymbol();
      Follow = true;
      damage = Attack(p);
    }
    if ((dist <= Hostility || Follow) && dist > 0 && (PosX != p.PosX || PosY != p.PosY))
      FollowPlayer(p, lvl);
    else
      Move(lvl);
    return damage;
  }

  public virtual bool ProcessDamage(int damage) {
    Hp -= damage;
    if (Hp <= 0) {
      Symbol = "";
      Dead = true;
    }
    return Dead;
  }

  public int GenTreasure() {
    Random rnd = new();
    return rnd.Next(Hp_max, Str + Agl + Hostility + Hp_max);
  }
}