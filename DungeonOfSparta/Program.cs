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
        storeInventory.Add(new Item("무쇠갑옷", "튼튼한 갑옷", ItemType.ARMOR, 0, 5, 0, 500));
        storeInventory.Add(new Item("낡은 검", "낡은 검", ItemType.WEAPON, 2, 0, 0, 1000));
        storeInventory.Add(new Item("골든 헬름", "희귀한 투구", ItemType.ARMOR, 0, 9, 0, 2000));

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