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

    // BattleScene에서 player 정보 출력할 때 사용
    public void PrintPlayerDescription(int bonusHp = 0)
    {
        Console.WriteLine("[내정보]");
        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), $" {Name} ( {Job} )");
        ConsoleUtility.PrintTextHighlights("HP ", Hp.ToString(), "", false);
        ConsoleUtility.PrintTextHighlights("/", (100 + bonusHp).ToString());
    }

    // BattleScene에서 player 정보 변화를 출력할 때 사용
    public void PrintPlayerChangeDescription(int initialHp, int damage = 0)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            IsDead = true;
            Hp = 0;
        }

        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), $" {Name}");
        ConsoleUtility.PrintTextHighlights("HP ", initialHp.ToString(), " -> ", false);
        if (IsDead)
        {
            Console.WriteLine("Dead");
        }
        else
        {
            ConsoleUtility.PrintTextHighlights("", Hp.ToString());
        }
    }

    // BattleScene에서 player가 monster를 공격할 때 사용
    public void PrintAttackDescription(Monster monster, int damage)
    {
        Console.WriteLine($"{Name} 의 공격!");
        ConsoleUtility.PrintTextHighlights("Lv.", monster.Level.ToString(), "", false);
        Console.Write($" {monster.Name} 을(를) 맞췄습니다. ");
        ConsoleUtility.PrintTextHighlights("[데미지 : ", damage.ToString(), "]");
    }
}