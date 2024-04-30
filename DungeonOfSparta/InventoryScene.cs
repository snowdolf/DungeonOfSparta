public partial class GameManager
{
    private void InventoryScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ 인벤토리 ■");
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.WriteLine("");
        Console.WriteLine("[아이템 목록]");

        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].PrintItemStatDescription();
        }

        Console.WriteLine("");
        Console.WriteLine("1. 장착관리");
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 1))
        {
            case 0:
                MainScene();
                break;
            case 1:
                EquipScene();
                break;
        }
    }

    private void EquipScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ 인벤토리 - 장착 관리 ■");
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.WriteLine("");
        Console.WriteLine("[아이템 목록]");
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].PrintItemStatDescription(true, i + 1); // 나가기가 0번 고정, 나머지가 1번부터 배정
        }
        Console.WriteLine("");
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");

        int KeyInput = ConsoleUtility.PromptSceneChoice(0, inventory.Count);

        switch (KeyInput)
        {
            case 0:
                InventoryScene();
                break;
            default:
                if(inventory[KeyInput - 1].IsEquipped == false)
                {
                    for(int i = 0; i < inventory.Count; i++)
                    {
                        if (inventory[i].Type == inventory[KeyInput - 1].Type && inventory[i].IsEquipped == true)
                        {
                            inventory[i].ToggleEquipStatus();
                        }
                    }
                }
                inventory[KeyInput - 1].ToggleEquipStatus();
                EquipScene();
                break;
        }
    }
}