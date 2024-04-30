internal class Player
{
    public string Name { get; }
    public string Job { get; }
    public int Level { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Hp { get; set; }
    public int Gold { get; set; }
    public int Potion { get; set; }
    public bool IsDead { get; set; }

    public Player(string name, string job, int level, int atk, int def, int hp, int gold, int potion, bool isDead = false)
    {
        Name = name;
        Job = job;
        Level = level;
        Atk = atk;
        Def = def;
        Hp = hp;
        Gold = gold;
        Potion = potion;
        IsDead = isDead;
    }
}