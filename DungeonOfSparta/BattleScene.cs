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

        random = new Random();

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
                MyBattleScene();
                break;
            default:
                break;
        }
    }

    private void MyBattleScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                MainScene();
                break;
            default:
                break;
        }
    }
}
