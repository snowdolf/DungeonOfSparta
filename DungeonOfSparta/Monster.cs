using System.Numerics;

internal class Monster
{
    public string Name { get; }
    public int Level { get; }
    public int Hp { get; }
    public int Atk { get; }
    public bool IsDead { get; private set; }

    public Monster(string name, int level, int hp, int atk, bool isDead = false)
    {
        Name = name;
        Level = level;
        Hp = hp;
        Atk = atk;
        IsDead = isDead;
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
}