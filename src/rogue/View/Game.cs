namespace rogue.View;

using rogue.Domain;
using rogue.Domain.LevelMap;
using rogue.Domain.Items;
using rogue.Data;

class Game {
  public bool isOver = false, killEnemy = false;
  private int _difficulty = 1;

  public Level lvl;
  public Player player;
  public Statistics stats = new();
  public SessionData session = new();
  public Messages msg = new();
  public List<int> attackResult = [];

  public Game() {
    lvl = new(_difficulty);
    var playerPos = lvl.GetStartPos();
    player = new(playerPos[1], playerPos[0]);
  }

  public string UpdateGame(int action) {
    msg.Clear();
    attackResult = player.Move(action, lvl, stats);
    // check for level end
    List<int> endPos = lvl.GetEndPos();
    if (endPos[0] == player.PosY && endPos[1] == player.PosX) {
      if (player.Lvl == 21) {
        isOver = true;
        SaveStats();
      } else
        NextLevel();
      return "";
    }
    // damage to enemy
    killEnemy = lvl.ProcessDamage(attackResult, _difficulty, player);
    msg.ProcessItemMessages(lvl, player, stats);

    // damage to player
    var res = msg.ProcessDamageMessages(lvl, player, stats);
    string attacker = res.Item1;
    isOver = res.Item2;
    msg.ProcessAttackMessages(lvl, player, stats, attackResult, killEnemy);
    if (isOver)
      SaveStats();
    return attacker;
  }

  public void NextLevel() {
    player.Lvl++;
    stats.Lvl = player.Lvl;
    _difficulty = (int)Math.Ceiling(player.Lvl / 2.0);
    // adjust difficulty
    if ((float)player.Hp / player.Hp_max <= 0.5)
      _difficulty = _difficulty > 2 ? _difficulty - 2 : 1;
    if (_difficulty > 10)
      _difficulty = 10;

    lvl = new(_difficulty);
    var playerPos = lvl.GetStartPos();
    player.InitCoords(playerPos[1], playerPos[0]);
    player.backpack.keys.Clear();
    lvl.UpdateField();
    session = SessionDataSaver.LoadData(lvl, player, stats);
  }

  public bool UseItem(Item item) {
    bool success = true;
    if (item is Weapon && player.currWeapon.Equipped)
      success = lvl.DropWeapon(player);
    if (item is Key k) {
      success = player.UseKey(k, lvl);
      if (success)
        lvl.UpdateField();
    } else if (success)
      player.UseItem(item, stats);
    lvl.UpdateField();
    return success;
  }

  public void RemoveCurrWeapon() {
    player.RemoveCurrWeapon();
  }

  public bool LoadSession() {
    if (SessionDataSaver.GetSessionData() == null)
      return false;
    session = SessionDataSaver.GetSessionData()!;
    lvl.field = session.Field;
    lvl.enemies = session.Enemies;
    lvl.items = session.Items;
    lvl.doors = session.Doors;
    lvl.rooms = session.Rooms;
    lvl.corridors = session.Corridors;
    player = session.Player;
    stats = session.Stats;
    return true;
  }

  public void SaveStats() {
    var statList = GameOverStatSaver.GetGameOverStatData();
    GameOverStatSaver.AddRunStatistics(statList, stats);
    GameOverStatSaver.LoadData(statList);
  }
}
