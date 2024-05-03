using Newtonsoft.Json;
using System.Reflection.Emit;

public partial class GameManager
{
    private void SaveScene()
    {
        SaveDataToJson();
        return;
    }

    private void SaveDataToJson()
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), player.Name);
        if(Directory.Exists(directoryPath))
        {
            Console.WriteLine($"{player.Name} 폴더가 존재합니다.");
        }
        else
        {
            Console.WriteLine($"{player.Name} 폴더가 존재하지 않습니다...");

            Directory.CreateDirectory(directoryPath);

            Console.WriteLine($"{player.Name} 폴더를 만들었습니다.");
        }

        // Player player
        string filePath = Path.Combine(directoryPath, "player.json");
        string json = JsonConvert.SerializeObject(player, Formatting.Indented);
        File.WriteAllText(filePath, json);

        // List<Item> inventory
        filePath = Path.Combine(directoryPath, "inventory.json");
        json = JsonConvert.SerializeObject(inventory, Formatting.Indented);
        File.WriteAllText(filePath, json);

        // List<Item> storeInventory
        filePath = Path.Combine(directoryPath, "storeInventory.json");
        json = JsonConvert.SerializeObject(storeInventory, Formatting.Indented);
        File.WriteAllText(filePath, json);

        // List<Quest> questList
        filePath = Path.Combine(directoryPath, "questList.json");
        json = JsonConvert.SerializeObject(questList, Formatting.Indented);
        File.WriteAllText(filePath, json);

        Console.WriteLine($"{player.Name} 데이터 저장을 완료했습니다");
    }

    private bool LoadDataFromJson()
    {
        string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), player.Name);
        if (Directory.Exists(directoryPath))
        {
            Console.WriteLine($"{player.Name} 폴더가 존재합니다.");
            Thread.Sleep(500);
        }
        else
        {
            Console.WriteLine($"{player.Name} 폴더가 존재하지 않습니다...");
            Thread.Sleep(500);
            return false;
        }

        // Player player
        string filePath = Path.Combine(directoryPath, "player.json");
        string json = File.ReadAllText(filePath);
        player = JsonConvert.DeserializeObject<Player>(json);

        // List<Item> inventory
        filePath = Path.Combine(directoryPath, "inventory.json");
        json = File.ReadAllText(filePath);
        inventory = JsonConvert.DeserializeObject<List<Item>>(json);

        // List<Item> storeInventory
        filePath = Path.Combine(directoryPath, "storeInventory.json");
        json = File.ReadAllText(filePath);
        storeInventory = JsonConvert.DeserializeObject<List<Item>>(json);

        // List<Quest> questList
        filePath = Path.Combine(directoryPath, "questList.json");
        json = File.ReadAllText(filePath);
        questList = JsonConvert.DeserializeObject<List<Quest>>(json);

        // List<Skill> SkillList에서 Active 저장이 안됨 ㅜ
        // 플레이어 레벨에 따라 일일히 스킬 리스트에 저장
        if (player.Level >= 2)
        {
            skills.SkillEarn(Skill.SkillName.Slash); // 휩쓸기
        }
        if(player.Level >= 3)
        {
            skills.SkillEarn(Skill.SkillName.RevengeAttack); // 복수
        }

        Console.WriteLine($"{player.Name} 데이터 로딩을 완료했습니다.");
        Thread.Sleep(500);

        return true;
    }
}