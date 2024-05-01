public partial class GameManager
{
    private void PotionScene(string? prompt = null)
    {
        if (prompt != null)
        {
            // 1초간 메시지를 띄운 다음에 다시 진행
            Console.Clear();
            ConsoleUtility.ShowTitle(prompt);
            Thread.Sleep(300);
        }

        Console.Clear();

        ConsoleUtility.ShowTitle("■ 회복 ■");
        Console.Write("포션을 사용하면 체력을 30 회복할 수 있습니다. ");
        ConsoleUtility.PrintTextHighlights("(남은 포션 : ", player.Potion.ToString(), " )");

        Console.WriteLine("");
        Console.WriteLine("1. 사용하기");
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");

        int bonusHp = inventory.Select(item => item.IsEquipped ? item.Hp : 0).Sum();

        switch (ConsoleUtility.PromptSceneChoice(0, 1))
        {
            case 0:
                MainScene();
                break;
            case 1:
                // 보유 포션이 충분하다면
                if (player.Potion > 0)
                {
                    player.Hp += 30;
                    if (player.Hp > (100 + bonusHp))
                    {
                        player.Hp = 100 + bonusHp;
                    }
                    player.Potion--;
                    PotionScene("회복을 완료했습니다.");
                }
                // 보유 포션이 부족하다면
                else
                {
                    PotionScene("포션이 부족합니다.");
                }
                break;
        }
    }
}
