public partial class GameManager
{
    private void StoreScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ 상점 ■");
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.WriteLine("");
        Console.WriteLine("[보유 골드]");
        ConsoleUtility.PrintTextHighlights("", player.Gold.ToString(), " G");
        Console.WriteLine("");
        Console.WriteLine("[아이템 목록]");
        for (int i = 0; i < storeInventory.Count; i++)
        {
            storeInventory[i].PrintStoreItemDescription();
        }
        Console.WriteLine("");
        Console.WriteLine("1. 아이템 구매");
        Console.WriteLine("2. 아이템 판매");
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");
        switch (ConsoleUtility.PromptSceneChoice(0, 2))
        {
            case 0:
                MainScene();
                break;
            case 1:
                PurchaseScene();
                break;
            case 2:
                SellScene();
                break;
        }
    }

    private void PurchaseScene(string? prompt = null)
    {
        if (prompt != null)
        {
            // 1초간 메시지를 띄운 다음에 다시 진행
            Console.Clear();
            ConsoleUtility.ShowTitle(prompt);
            Thread.Sleep(300);
        }

        Console.Clear();

        ConsoleUtility.ShowTitle("■ 상점 - 아이템 구매 ■");
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.WriteLine("");
        Console.WriteLine("[보유 골드]");
        ConsoleUtility.PrintTextHighlights("", player.Gold.ToString(), " G");
        Console.WriteLine("");
        Console.WriteLine("[아이템 목록]");
        for (int i = 0; i < storeInventory.Count; i++)
        {
            storeInventory[i].PrintStoreItemDescription(true, i + 1);
        }
        Console.WriteLine("");
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");

        int keyInput = ConsoleUtility.PromptSceneChoice(0, storeInventory.Count);

        switch (keyInput)
        {
            case 0:
                StoreScene();
                break;
            default:
                // 1 : 이미 구매한 경우
                if (storeInventory[keyInput - 1].IsPurchased) // index 맞추기
                {
                    PurchaseScene("이미 구매한 아이템입니다.");
                }
                // 2 : 돈이 충분해서 살 수 있는 경우
                else if (player.Gold >= storeInventory[keyInput - 1].Price)
                {
                    player.Gold -= storeInventory[keyInput - 1].Price;
                    storeInventory[keyInput - 1].Purchase();
                    inventory.Add(storeInventory[keyInput - 1]);
                    PurchaseScene("구매를 완료했습니다.");
                }
                // 3 : 돈이 모자라는 경우
                else
                {
                    PurchaseScene("Gold가 부족합니다.");
                }
                break;
        }
    }

    private void SellScene(string? prompt = null)
    {
        if (prompt != null)
        {
            // 1초간 메시지를 띄운 다음에 다시 진행
            Console.Clear();
            ConsoleUtility.ShowTitle(prompt);
            Thread.Sleep(300);
        }
        Console.Clear();

        ConsoleUtility.ShowTitle("■ 상점 - 아이템 판매 ■");
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.WriteLine("");
        Console.WriteLine("[보유 골드]");
        ConsoleUtility.PrintTextHighlights("", player.Gold.ToString(), " G");
        Console.WriteLine("");
        Console.WriteLine("[아이템 목록]");
        for (int i = 0; i < inventory.Count; i++)
        {
           inventory[i].PrintStoreItemDescription(true, i + 1, true);
        }
        Console.WriteLine("");
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");

        int keyInput = ConsoleUtility.PromptSceneChoice(0, inventory.Count);

        switch (keyInput)
        {
            case 0:
                StoreScene();
                break;
            default:
                player.Gold += inventory[keyInput - 1].Price * 85 / 100;
                inventory[keyInput - 1].Sell();
                inventory.Remove(inventory[keyInput - 1]);
                SellScene("판매를 완료했습니다.");
                break;
        }
    }
}