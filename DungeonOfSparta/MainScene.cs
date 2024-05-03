﻿public partial class GameManager
{
    private void MainScene(string? prompt = null)
    {
        if (prompt != null)
        {
            // 1초간 메시지를 띄운 다음에 다시 진행
            Console.Clear();
            ConsoleUtility.ShowTitle(prompt);
            Thread.Sleep(300);
        }

        // 0. 화면 정리
        Console.Clear();

        // 1. 선택 멘트를 줌
        Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
        Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
        Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
        Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
        Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
        Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
        Console.WriteLine("");

        Console.WriteLine("1. 상태 보기");
        Console.WriteLine("2. 인벤 토리");
        Console.WriteLine("3. 상     점");
        Console.WriteLine("4. 전투 시작");
        Console.WriteLine("5. 회복 아이템");
        Console.WriteLine("6. 퀘스트");
        Console.WriteLine("");

        Console.WriteLine("");
        Console.WriteLine("0. 저장 및 나가기");
        Console.WriteLine("");

        int choice = ConsoleUtility.PromptSceneChoice(0, 6);

        // 3. 선택한 결과에 따라 보내줌
        switch (choice)
        {
            case 0:
                // 저장 기능 구현하기
                SaveScene();
                return;
            case 1:
                StatusScene();
                break;
            case 2:
                InventoryScene();
                break;
            case 3:
                StoreScene();
                break;
            case 4:
                BattleScene();
                break;
            case 5:
                PotionScene();
                break;
            case 6:
                QuestSelectScene();
                break;
        }
    }
}