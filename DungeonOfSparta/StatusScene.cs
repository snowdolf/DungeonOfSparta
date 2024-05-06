public partial class GameManager
{
    private void StatusScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ 상태보기 ■");
        Console.WriteLine("캐릭터의 정보가 표기됩니다.");

        Console.WriteLine("");
        ConsoleUtility.PrintTextHighlights("Lv. ", player.Level.ToString("00"));
        Console.WriteLine($"{player.Name} ( {player.Job} )");

        // TODO : 능력치 강화분을 표현하도록 변경

        player.BonusAtk = inventory.Select(item => item.IsEquipped ? item.Atk : 0).Sum();
        player.BonusDef = inventory.Select(item => item.IsEquipped ? item.Def : 0).Sum();
        player.BonusHp = inventory.Select(item => item.IsEquipped ? item.Hp : 0).Sum();

        ConsoleUtility.PrintTextHighlights("공격력 : ", (player.Atk + player.BonusAtk).ToString(), player.BonusAtk > 0 ? $" (+{player.BonusAtk})" : "");
        ConsoleUtility.PrintTextHighlights("방어력 : ", (player.Def + player.BonusDef).ToString(), player.BonusDef > 0 ? $" (+{player.BonusDef})" : "");
        ConsoleUtility.PrintTextHighlights("체 력 : ", player.Hp.ToString(), "", false);
        ConsoleUtility.PrintTextHighlights("/", (100 + player.BonusHp).ToString(), player.BonusHp > 0 ? $" (+{player.BonusHp})" : "");

        ConsoleUtility.PrintTextHighlights("Gold : ", player.Gold.ToString());

        Console.WriteLine("");
        skills.SkillDescription();  // 스킬 목록 보여주기

        Console.WriteLine("");
        Console.WriteLine("0. 뒤로가기");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                MainScene();
                break;
        }
    }
}