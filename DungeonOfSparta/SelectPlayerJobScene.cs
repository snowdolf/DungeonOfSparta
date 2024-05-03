public partial class GameManager
{
    private void SelectPlayerJobScene()
    {
        Console.Clear();

        Console.WriteLine("원하시는 직업을 선택해주세요.");

        Console.WriteLine("");
        Console.WriteLine("1. 전사");
        Console.WriteLine("2. 거지");

        Console.WriteLine("");

        int choice = ConsoleUtility.PromptSceneChoice(1, 2);

        switch (choice)
        {
            case 1:
                player.Job = "전사";
                player.Atk = 10;
                player.Def = 5;
                player.Hp = 100;
                player.Gold = 10000;
                player.Potion = 3;
                break;
            case 2:
                player.Job = "거지";
                player.Atk = 20;
                player.Def = 20;
                player.Hp = 50;
                player.Gold = 0;
                player.Potion = 0;
                break;
        }
        MainScene();
    }
}