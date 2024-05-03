using System;

enum QuestRewardType
{
    Reward_Item,
    Reward_Gold,
    Reward_Potion
}


public partial class GameManager
{
    
    List<Quest> questList = new List<Quest>();
    
    public void QuestManager()
    {
        questList.Add(new Quest("미니언잡기","미니언을 잡아라",MonsterType.Minion,5, 0));
        questList.Add(new Quest("대포미니언잡기","대포미니언을 잡아라", MonsterType.SiegeMinion, 5, 0));
        questList.Add(new Quest("공허충잡기","공허충을 잡아라", MonsterType.VoidSwarm, 5,0));
    }

    public void PrintQuestInfo(int idx)
    {
        Console.WriteLine(questList[idx].Title);
        Console.WriteLine(questList[idx].Description);
        Console.WriteLine($"{questList[idx].MonsterType} - {questList[idx].MonsterCount}/{questList[idx].MonsterKill}");

        Console.Write("Reward - ");
        Console.WriteLine(storeInventory[3].Name);
    }

    public void PrintQuestTitle(int idx)
    {
        if (questList[idx].IsCompleted)
        {
            Console.WriteLine($"{idx+1} - {questList[idx].Title}[완료]");
        }
        else if (!questList[idx].IsCompleted && questList[idx].IsAccept)
        {
            Console.WriteLine($"{idx + 1} - {questList[idx].Title}[진행중]");
        }
        else
        {
            Console.WriteLine($"{idx + 1} - {questList[idx].Title}");
        }
    }

    public void QuestSelectScene()
    {
        Console.Clear();
        
        ConsoleUtility.ShowTitle("▣ Quest ▣");

        Console.WriteLine("");
        for (int i = 0; i < questList.Count; i++)
        {
            PrintQuestTitle(i);
        }
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");
        int selectQuestNumber = ConsoleUtility.PromptSceneChoice(0, 3);
        switch (selectQuestNumber)
        {
            case 0:
                MainScene();
                break;
            default:
                QuestCurrectScene(selectQuestNumber - 1);
                break;
        }
    }

    public void QuestCurrectScene(int selectQuest)
    {
        
        Console.Clear();

        ConsoleUtility.ShowTitle("▣ Quest ▣");
        if (questList[selectQuest].IsAccept == true)
        {
            Console.WriteLine(" ");
            ConsoleUtility.ShowTitle("[진행중]");
            Console.WriteLine(" ");
        }
        else if(!questList[selectQuest].IsAccept&& questList[selectQuest].IsCompleted)
        {
            Console.WriteLine(" ");
            ConsoleUtility.ShowTitle("[ 완 료 ]");
            Console.WriteLine(" ");
        }

        PrintQuestInfo(selectQuest);
        Console.WriteLine(" ");


        if (questList[selectQuest].IsAccept == false && questList[selectQuest].IsCompleted == false)
        {
            Console.WriteLine("1. 수락");
            Console.WriteLine("2. 거절");
            switch (ConsoleUtility.PromptSceneChoice(1, 2))
            {
                case 1:
                    questList[selectQuest].IsAccept = true;
                    QuestCurrectScene(selectQuest);
                    break;
                case 2:
                    QuestSelectScene();
                    break;
            }
        }
        else if (questList[selectQuest].IsAccept==true && questList[selectQuest].IsCompleted == false)
        {
            Console.WriteLine("1. 취소");
            Console.WriteLine("0. 나가기");
            switch (ConsoleUtility.PromptSceneChoice(0, 1))
            {
                case 1:
                    questList[selectQuest].IsAccept = false;
                    QuestCurrectScene(selectQuest);
                    break;
                case 0:
                    QuestSelectScene();
                    break;
            }
        }
        else if (questList[selectQuest].IsAccept == true && questList[selectQuest].IsCompleted == true)
        {
            Console.WriteLine("1. 퀘스트 완료");
            Console.WriteLine("0. 나가기");
            switch (ConsoleUtility.PromptSceneChoice(0,1))
            {
                case 1:
                    QuestCurrectScene(selectQuest);
                    questList[selectQuest].IsAccept = false;
                    break;
                case 0:
                    QuestSelectScene();
                    break;
            }
        }
        else
        {
            Console.WriteLine("이미 완료된 퀘스트입니다.");
            Console.WriteLine("0. 나가기");
            ConsoleUtility.PromptSceneChoice(0, 0);
            QuestSelectScene();
        }


        //Console.WriteLine(quest.MonsterKill);



    }
}

internal class Quest
{
    public MonsterType MonsterType { get; }  
    public string Title { get;}
    public string Description { get; }

    public int MonsterKill { get; }
    public int MonsterCount { get; set; }

    public bool IsCompleted {  get; set; }
    public bool IsAccept {  get; set; }


    public Quest(string title, string description,MonsterType monsterType,int monsterKill,int monsterCount)
    {
        Title = title;
        Description = description;
        MonsterType = monsterType;
        MonsterKill = monsterKill;
        MonsterCount = monsterCount;
        IsCompleted = false;
        IsAccept = false;
    }

    


    //public void QuestSceneTitleInfo(string title, string description,string monsterType)
    //{
    //    Console.Clear();

    //    ConsoleUtility.ShowTitle(title);

    //    Console.WriteLine(description);

    //    //Console.WriteLine($" - {monsterType} {monsterCount}처치  {monsterCount}");
    //    if (!IsAccept)
    //    {
    //        switch (ConsoleUtility.PromptSceneChoice(1, 2))
    //        {
    //            case 1:
    //                Console.WriteLine("1. 수락");
    //                break;
    //            case 2:
    //                Console.WriteLine("2. 거절");
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        switch (ConsoleUtility.PromptSceneChoice(1, 2))
    //        {
    //            case 1:
    //                Console.WriteLine("1. 취소");
    //                break;
    //            case 2:
    //                Console.WriteLine("2. 나가기");
    //                break;
    //        }
    //    }
    //}
    public void QuestReard(string rewardItem)
    {
        if(IsCompleted)
        {
            // 아이템 및 골드
        }
    }
}

