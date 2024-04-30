using System;
using System.Xml.Linq;

enum MonsterType
{
    Minion,
    VoidSwarm,
    SiegeMinion
}

public partial class GameManager
{
    // 필요한 콜백함수
    private Action<int, int> MonsterDamageCallback;
    private Action<int> PlayerDamagedCallback;

    private List<Monster> monsters;

    Random random = new Random();
    int randomNumber;
    
    int delay = 1000;   // 텍스트 보여주는 딜레이를 조정할 수 있습니다.

    // 전리품과 승리 여부
    int loot;
    bool isWin;
    bool isLose;

    int escapeDifficult = 5;    // 도주 난이도

    // 장비에 따라서 플레이어 스텟 세팅
    int playerAtk;
    int playerDef;

    // 전투 씬 세팅
    private void BattleScene()
    {
        monsters = new List<Monster>();

        random = new Random();

        randomNumber = random.Next(1, 5);
        for (int i = 0; i < randomNumber; i++)
        {
            // Enum.GetNames(typeof(MonsterType)).Length = 3
            // random.Next(3) = 0 ~ 2
            MonsterType monsterType = (MonsterType)random.Next(Enum.GetNames(typeof(MonsterType)).Length);

            switch (monsterType)
            {
                case MonsterType.Minion:
                    monsters.Add(new Monster("미니언", 2, 15, 5, 1));
                    break;
                case MonsterType.VoidSwarm:
                    monsters.Add(new Monster("공허충", 3, 10, 9, 2));
                    break;
                case MonsterType.SiegeMinion:
                    monsters.Add(new Monster("대포미니언", 5, 25, 8, 3));
                    break;
            }
        }

        // 초기 세팅 (유니티랑 다르게 이걸 안해주면 값이 계속 쌓여있다.)
        loot = 0;
        isWin = false;
        isLose = false;

        // 플레이어 장비값 추가 세팅
        playerAtk = player.Atk + (inventory.Select(item => item.IsEquipped ? item.Atk : 0).Sum());
        playerDef = player.Def + (inventory.Select(item => item.IsEquipped ? item.Def : 0).Sum());

        // 콜백 세팅
        MonsterDamageCallback = (select, damage) =>
        {
            // 몬스터 데미지 입기
            monsters[select].TakeDamage(playerAtk);
            Console.WriteLine($"{monsters[select].Name}이(가) {damage}만큼 피해를 입었습니다! {monsters[select].Hp + damage} -> {monsters[select].Hp}");
            Thread.Sleep(delay);

            // 체력 판정
            if (monsters[select].Hp <= 0)
            {
                Console.WriteLine($"{monsters[select].Name}이(가) 쓰러졌습니다!");
                loot += monsters[select].Loot;
                Thread.Sleep(delay);
                monsters.RemoveAt(select);
            }

            // 몬스터가 더이상 없다면
            if (monsters.Count <= 0)
            {
                Console.WriteLine($"더 이상 적이 없습니다!");
                Thread.Sleep(delay);
                isWin = true;
                BattleEnd("승리");
            }
        };

        PlayerDamagedCallback = (damage) =>
        {
            // 플레이어 데미지 입기
            player.Hp -= damage;
            Console.WriteLine($" {damage}의 데미지를 가했습니다! 당신의 HP : {player.Hp + damage} -> {player.Hp}");

            // 플레이어의 체력이 다 떨어졌다면
            if (player.Hp <= 0)
            {
                Console.WriteLine($"{player.Name}이(가) 쓰러졌습니다!");
                Thread.Sleep(delay);
                Console.WriteLine($"눈 앞이 깜깜해졌다!");
                Thread.Sleep(delay);
                isLose = true;
                BattleEnd("패배");
            }
        };

        BattleStartScene();
    }

        // 전투 씬
    private void BattleStartScene()
    {
        Console.Clear();

        ShowStatus("나의 턴!");

        Console.WriteLine("");
        Console.WriteLine("1. 공격");
        Console.WriteLine("2. 스킬");
        Console.WriteLine("3. 소모품");
        Console.WriteLine("4. 턴 넘기기");
        Console.WriteLine("0. 도주");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 4))
        {
            case 0:
                PlayerFlee();
                break;
            case 1:
                if (monsters.Count == 1) { InstantAttack(); break; }
                    PlayerAttack();
                break;
            case 2:
                UpdateSoon();   // 플레이어의 스킬목록을 열어서 스킬 선택지를 본다.
                break;
            case 3:
                PotionBattleScene();   // 플레이어의 인벤토리를 열어서 소모품 목록을 출력
                break;
            default:
                ConsoleUtility.ShowTitle("당신은 아무것도 하지 않습니다!");
                Thread.Sleep(delay);
                EnemyTurn();
                break;
        }
    }

        // 플레이어의 선택지

            // 도주
    private void PlayerFlee()
    {
        Console.WriteLine("플레이어 도주 시도!"); Thread.Sleep(delay);
        int escape = random.Next(0, 9);
        if (escape >= escapeDifficult) { Console.WriteLine("도주에 성공하셨습니다!"); Thread.Sleep(delay); BattleEnd("도주"); }
        else { Console.WriteLine("도주에 실패하셨습니다!"); Thread.Sleep(delay); EnemyTurn(); }
    }

            // 공격
    private void InstantAttack() // 적이 하나 밖에 없을 경우
    {
        MonsterDamageCallback(0, playerAtk);
        EnemyTurn();
    }

    private void PlayerAttack() // 통상적인 공격 선택지
    {
        ShowStatus("나의 턴!", true);

        Console.WriteLine("");
        Console.WriteLine("공격하고자 하는 적을 선택하세요!");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");
       
        int select = ConsoleUtility.PromptSceneChoice(0, monsters.Count);
        if (select == 0) { BattleStartScene(); return; }
        else
        {
            select--;   // 인덱싱에 유의할 것
            MonsterDamageCallback(select, playerAtk);
        }
        EnemyTurn();
    }

            // 소모품

    private void PotionBattleScene(string? prompt = null)
    {
        ShowStatus("나의 턴!");

        Console.WriteLine("");
        ConsoleUtility.ShowTitle("■ 소모품 사용 ■");
        Console.Write("포션을 사용하면 체력을 30 회복할 수 있습니다. ");
        ConsoleUtility.PrintTextHighlights("(남은 포션 : ", player.Potion.ToString(), " )");
        Console.WriteLine("");

        ConsoleUtility.PrintTextHighlights($"", $"!주의! - 사용 시 적의 턴으로 넘어갑니다!");

        Console.WriteLine("");
        Console.WriteLine("1. 사용하기");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 1))
        {
            case 0:
                BattleStartScene();
                break;
            case 1:
                // 체력이 충분하다면
                if (player.Hp >= 100) 
                { 
                    ConsoleUtility.ShowTitle("이미 최대 체력입니다!");
                    Thread.Sleep(delay);
                    PotionBattleScene();
                    break;
                }
                // 보유 포션이 충분하다면
                ConsoleUtility.PrintTextHighlights($"체력이 회복되었습니다! ", $"{player.Hp} -> ", "", false);
                if (player.Potion > 0)
                {
                    player.Hp += 30;
                    if (player.Hp > 100)
                    {
                        player.Hp = 100;
                    }
                    player.Potion--;
                    ConsoleUtility.PrintTextHighlights("", $"{player.Hp}");
                    Thread.Sleep(delay);
                    EnemyTurn();
                }
                // 보유 포션이 부족하다면
                else
                {
                    ConsoleUtility.ShowTitle("포션이 부족합니다.");
                    Thread.Sleep(delay);
                    PotionBattleScene();
                }
                break;
        }
    }


    // 적의 턴
    private void EnemyTurn()
    {
        ShowStatus("적의 턴!");

        Console.WriteLine("");
        for (int i = 0; i < monsters.Count; i++)
        {
            Thread.Sleep(delay);
            int enemyAct = random.Next(0, 2);
            EnemyAct(i, enemyAct);
            if (isLose) { return; }
        }

        Thread.Sleep(delay);
        BattleStartScene();
    }



                //적의 행동
    private void EnemyAct(int i ,int actNum)
    {
        switch (actNum)
        {
            case 0:
                Console.WriteLine($"{monsters[i].Name}이(가) 당신을 바라보고 있습니다.");
                break;
            case 1:
                Console.Write($"{monsters[i].Name}이(가) 당신에게");
                PlayerDamagedCallback(monsters[i].Atk); 
                break;
            default:
                Console.WriteLine($"{monsters[i].Name}은(는) 아무것도 하고 있지 않습니다..");
                break;
        }
    }

    private void BattleEnd(string result)   
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");
        Console.WriteLine($"전투에서 {result}하셨습니다!");

        if (isWin) { GetReward(); }
        if (isLose) { GetPenalty(); }

        Console.WriteLine("");
        Console.WriteLine("0. 돌아가기");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                MainScene();
                break;
            default:
                break;
        }
    }
    // 보상 획득창
    private void GetReward()
    {
        int reward = 1000 + 100 * loot;

        Console.WriteLine("");
        Console.WriteLine("[결과]");
        ConsoleUtility.PrintTextHighlights("Gold ", player.Gold.ToString(), "", false); ConsoleUtility.PrintTextHighlights(" -> ", $"{player.Gold + reward}");

        player.Gold += reward;
    }
    // 패배 페널티창
    private void GetPenalty()
    {
        int penalty = player.Gold / 2;

        Console.WriteLine("");
        Console.WriteLine("[결과]");
        ConsoleUtility.PrintTextHighlights("Gold ", player.Gold.ToString(), "", false); ConsoleUtility.PrintTextHighlights(" -> ", $"{player.Gold - penalty}");

        player.Gold -= penalty;
        player.Hp = 50;
    }


    // 전투 정보창 보여주는 곳

    // 정보 보여주기

    private void ShowStatus(string whosTurn, bool showNum = false)
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");
        Console.WriteLine(whosTurn);
        Console.WriteLine("");

        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].PrintMonsterDescription(showNum, i+1);
        }

        ShowPlayerStatus();
    }

                // 플레이어 정보창 보여주기
    private void ShowPlayerStatus()
    {
        Console.WriteLine("");
        Console.WriteLine("[내정보]");
        ConsoleUtility.PrintTextHighlights("Lv.", player.Level.ToString(), $" {player.Name} ( {player.Job} )");
        ConsoleUtility.PrintTextHighlights("HP ", player.Hp.ToString(), "", false); ConsoleUtility.PrintTextHighlights("/", "100");
        ConsoleUtility.PrintTextHighlights("공격력 ", playerAtk.ToString());
        ConsoleUtility.PrintTextHighlights("방어력 ", playerDef.ToString());
    }

    // 추후에 업데이트할 예정
    private void UpdateSoon()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");
        Console.WriteLine("나의 턴!");
        Console.WriteLine("");

        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].PrintMonsterDescription();
        }

        Console.WriteLine("");
        Console.WriteLine("[내정보]");
        ConsoleUtility.PrintTextHighlights("Lv.", player.Level.ToString(), $" {player.Name} ( {player.Job} )");
        ConsoleUtility.PrintTextHighlights("HP ", player.Hp.ToString(), "", false);
        ConsoleUtility.PrintTextHighlights("/", "100");

        Console.WriteLine("");
        Console.WriteLine("아직 구현 중인 기능입니다!");
        Console.WriteLine("");

        Thread.Sleep(delay);

        BattleStartScene();
    }
}
