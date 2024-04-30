using System.Numerics;

internal class Monster
{
    public string Name { get; }
    public int Level { get; }
    public int Hp { get; private set;  }
    public int Atk { get; }
    public bool IsDead { get; private set; }

    public int Loot { get; private set; }

    public Monster(string name, int level, int hp, int atk, int loot, bool isDead = false)
    {
        Name = name;
        Level = level;
        Hp = hp;
        Atk = atk;
        IsDead = isDead;
        Loot = loot;
    }

    public void PrintMonsterDescription(bool withNumber = false, int idx = 0)
    {
        if (withNumber)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"{idx} ");
            Console.ResetColor();
        }
        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), " ", false);
        Console.Write($"{Name} ");
        ConsoleUtility.PrintTextHighlights("HP ", Hp.ToString());
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
    }
}