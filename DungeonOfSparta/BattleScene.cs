enum MonsterType
{
    Minion,
    VoidSwarm,
    SiegeMinion
}

public partial class GameManager
{
    private List<Monster> monsters;

    Random random = new Random();
    int randomNumber;

    private void BattleScene()
    {
        monsters = new List<Monster>();

        randomNumber = random.Next(1, 5);
        for (int i = 0; i < randomNumber; i++)
        {
            // Enum.GetNames(typeof(MonsterType)).Length = 3
            // random.Next(3) = 0 ~ 2
            MonsterType monsterType = (MonsterType)random.Next(Enum.GetNames(typeof(MonsterType)).Length);

            switch (monsterType)
            {
                case MonsterType.Minion:
                    monsters.Add(new Monster("미니언", 2, 15, 5));
                    break;
                case MonsterType.VoidSwarm:
                    monsters.Add(new Monster("공허충", 3, 10, 9));
                    break;
                case MonsterType.SiegeMinion:
                    monsters.Add(new Monster("대포미니언", 5, 25, 8));
                    break;
            }
        }

        BattleStartScene();
    }

    private void BattleStartScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].PrintMonsterDescription();
        }

        Console.WriteLine("");
        Console.WriteLine("[내정보]");
        ConsoleUtility.PrintTextHighlights("Lv.", player.Level.ToString(), $" {player.Name} ( {player.Job} )");
        ConsoleUtility.PrintTextHighlights("HP ", player.Hp.ToString(), "", false);
        ConsoleUtility.PrintTextHighlights("/", "100");

        Console.WriteLine("");
        Console.WriteLine("1. 공격");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(1, 1))
        {
            case 1:
                MyBattlePhaseScene();
                break;
            default:
                break;
        }
    }

    private void MyBattlePhaseScene(string? prompt = null)
    {
        if (prompt != null)
        {
            // 1초간 메시지를 띄운 다음에 다시 진행
            Console.Clear();
            ConsoleUtility.ShowTitle(prompt);
            Thread.Sleep(300);
        }

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].PrintMonsterDescription(true, i + 1);
        }

        Console.WriteLine("");
        Console.WriteLine("[내정보]");
        ConsoleUtility.PrintTextHighlights("Lv.", player.Level.ToString(), $" {player.Name} ( {player.Job} )");
        ConsoleUtility.PrintTextHighlights("HP ", player.Hp.ToString(), "", false);
        ConsoleUtility.PrintTextHighlights("/", "100");

        Console.WriteLine("");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");

        int keyInput = ConsoleUtility.PromptSceneChoice(0, monsters.Count);

        switch (keyInput)
        {
            case 0:
                BattleStartScene();
                break;
            default:
                if (monsters[keyInput - 1].IsDead)
                {
                    MyBattlePhaseScene("이미 죽은 몬스터입니다.");
                }
                else
                {
                    int bonusAtk = inventory.Select(item => item.IsEquipped ? item.Atk : 0).Sum();
                    int damage = player.Atk + bonusAtk;
                    int damageMargin = (int)Math.Ceiling(damage * 0.1f);
                    
                    MyBattleResultScene(keyInput - 1, random.Next(damage - damageMargin, damage + damageMargin + 1));
                }
                break;
        }
    }

    private void MyBattleResultScene(int monsterIdx, int damage)
    {
        Monster target = monsters[monsterIdx];

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        Console.WriteLine($"{player.Name} 의 공격!");
        ConsoleUtility.PrintTextHighlights("Lv.", target.Level.ToString(), "", false);
        Console.WriteLine($" {target.Name} 을(를) 맞췄습니다. ");
        ConsoleUtility.PrintTextHighlights("[데미지 : ", damage.ToString(), "]");

        Console.WriteLine("");
        ConsoleUtility.PrintTextHighlights("Lv.", target.Level.ToString(), $" {target.Name}");
        ConsoleUtility.PrintTextHighlights("HP ", target.Hp.ToString(), " ", false);
        target.Hp -= damage;
        if (target.Hp <= 0)
        {
            target.IsDead = true;
        }
        ConsoleUtility.PrintTextHighlights("-> ", target.IsDead ? "Dead" : target.Hp.ToString(), "");

        Console.WriteLine("");
        Console.WriteLine("0. 다음");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                MyBattlePhaseScene();
                break;
            default:
                break;
        }
    }
}
