internal class Monster
{
    public string Name { get; protected set; }
    public int Level { get; protected set; }
    public int Hp { get; set; }
    public int Atk { get; }
    public bool IsDead { get; set; }
    public bool DeathRattle { get; set; }
    public MonsterType MonstersType { get; }

    public Monster(string name, int level, int hp, int atk, MonsterType monstersType, bool deathRattle = false)
    {
        Name = name;
        Level = level;
        Hp = hp;
        Atk = atk;
        IsDead = false;
        MonstersType = monstersType;
        DeathRattle = deathRattle;
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
    public void PrintAttackDescription(Player player, int damage)
    {
        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), "", false);
        Console.WriteLine($" {Name} 의 공격!");

        Console.WriteLine("");
        Console.Write($"{player.Name} 을(를) 맞췄습니다. ");
        ConsoleUtility.PrintTextHighlights("[데미지 : ", damage.ToString(), "]");
    }

    public virtual void DeathRattleActive(List<Monster> monsters) // 특수 몬스터 전용
    {
    }
}

class AncientKrug : Monster
{
    public AncientKrug() : base("고대돌거북", 5, 60, 20, MonsterType.Krugs, true)
    {
    } 
    public override void DeathRattleActive(List<Monster> monsters)
    {
        Console.WriteLine();
        ConsoleUtility.PrintTextHighlights("", "고대 돌거북이 둘로 쪼개집니다!");
        ConsoleUtility.PrintTextHighlights("->", "Lv. 5 돌거북 Lv. 5 돌거북");
        monsters.Add(new Krug());
        monsters.Add(new Krug());
        monsters.Remove(this);
    }
}

class Krug : Monster
{
    public Krug() : base("돌거북", 5, 30, 10, MonsterType.Krugs, true)
    {
    }
    public override void DeathRattleActive(List<Monster> monsters)
    {
        Console.WriteLine();
        ConsoleUtility.PrintTextHighlights("", "돌거북이 둘로 쪼개집니다!");
        ConsoleUtility.PrintTextHighlights("->", "Lv. 5 작은돌거북 Lv. 5 작은돌거북");
        monsters.Add(new Monster("작은돌거북", 5, 15, 5, MonsterType.Krugs));
        monsters.Add(new Monster("작은돌거북", 5, 15, 5, MonsterType.Krugs));
        monsters.Remove(this);
    }
}