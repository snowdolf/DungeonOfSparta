internal class Monster
{
    public string Name { get; }
    public int Level { get; }
    public int Hp { get; set; }
    public int Atk { get; }
    public bool IsDead { get; set; }

    public Monster(string name, int level, int hp, int atk, bool isDead = false)
    {
        Name = name;
        Level = level;
        Hp = hp;
        Atk = atk;
        IsDead = isDead;
    }

    // BattleScene에서 monster 정보 출력할 때 사용
    public void PrintMonsterDescription(bool withNumber = false, int idx = 0)
    {
        if (IsDead)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            if (withNumber)
            {
                Console.Write($"{idx} ");
            }
            Console.WriteLine($"Lv.{Level} {Name} Dead");
            Console.ResetColor();
        }
        else
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

    // BattleScene에서 monster 정보 변화를 출력할 때 사용
    public void PrintMonsterChangeDescription(int initialHp, int damage = 0)
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

    // BattleScene에서 monster가 player를 공격할 때 사용
    public void PrintAttackDescription(Player player)
    {
        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), "", false);
        Console.WriteLine($" {Name} 의 공격!");
        Console.Write($"{player.Name} 을(를) 맞췄습니다. ");
        ConsoleUtility.PrintTextHighlights("[데미지 : ", Atk.ToString(), "]");
    }
}