public partial class GameManager
{
    private Player player;
    private List<Item> inventory;

    private List<Item> storeInventory;

    private Skill skills = new Skill(); // 스킬 기능 추가!

    public GameManager()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        player = new Player("Jiwon", "Programmer", 1, 10, 5, 100, 15000, 3);

        inventory = new List<Item>();

        storeInventory = new List<Item>();

        storeInventory.Add(new Item("낡은 검", "낡은 검", ItemType.WEAPON, 5, 0, 0, 2000));
        storeInventory.Add(new Item("전사의 검", "전설 속 전사의 검", ItemType.WEAPON, 15, 0, 0, 10000));

        storeInventory.Add(new Item("수련자 투구", "쓸만한 투구", ItemType.HELMET, 0, 5, 0, 1000));
        storeInventory.Add(new Item("무쇠 투구", "튼튼한 투구", ItemType.HELMET, 0, 10, 5, 2000));

        storeInventory.Add(new Item("수련자 갑옷", "쓸만한 갑옷", ItemType.BODYARMOUR, 0, 10, 10, 2000));
        storeInventory.Add(new Item("무쇠 갑옷", "튼튼한 갑옷", ItemType.BODYARMOUR, 0, 20, 20, 4000));

        storeInventory.Add(new Item("수련자 장갑", "쓸만한 장갑", ItemType.GLOVE, 0, 5, 0, 1000));
        storeInventory.Add(new Item("무쇠 장갑", "튼튼한 장갑", ItemType.GLOVE, 3, 10, 0, 2000));

        storeInventory.Add(new Item("수련자 장화", "쓸만한 장화", ItemType.BOOT, 0, 5, 0, 1000));
        storeInventory.Add(new Item("무쇠 장화", "튼튼한 장화", ItemType.BOOT, 3, 10, 0, 2000));

        skills.SkillEarn(Skill.SkillName.DoubleAttack);  // 기본 스킬입니다.
    }

    public void StartGame()
    {
        Console.Clear();
        ConsoleUtility.PrintGameHeader();
        MainScene();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        GameManager gameManager = new GameManager();
        gameManager.StartGame();
    }
}