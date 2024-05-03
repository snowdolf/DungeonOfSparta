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

        // skills (SkillForSave 저장용)
        filePath = Path.Combine(directoryPath, $"skills.json");
        json = JsonConvert.SerializeObject(skills, Formatting.Indented);
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

        // skills (SkillForSave 저장용)
        filePath = Path.Combine(directoryPath, $"skills.json");
        json = File.ReadAllText(filePath);
        skills = JsonConvert.DeserializeObject<Skill>(json);

        // 스킬 할당을 위해 잠깐 다른 곳으로 빼두고 [참조형이니까 new를 꼭 써줘야한다.]
        List<SkillName> temp = new List<SkillName>(skills.SkillForSave); 

        // 중복 방지를 위해 지우는 작업
        skills.SkillList.Clear(); 
        skills.SkillForSave.Clear(); 

        foreach (SkillName skillname in temp) 
        {
            skills.SkillEarn(skillname);
        }
        Console.Clear(); // 스킬 얻는 거 가리기용
        Console.WriteLine($"{player.Name} 데이터 로딩을 완료했습니다.");
        Thread.Sleep(500);

        return true;
    }
}