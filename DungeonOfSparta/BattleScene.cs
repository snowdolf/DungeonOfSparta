using System.Reflection.Emit;
using System.Xml.Linq;

enum MonsterType
{
    Minion,     // 미니언
    VoidSwarm,  // 공허충
    SiegeMinion // 대포미니언
}

public partial class GameManager
{
    private List<Monster> monsters;

    Random random = new Random();
    int randomNumber;

    // 던전 입장 전 player 체력
    // FinalBattleResultScene 에서 활용될 예정
    int initialPlayerHp;

    int bonusAtk;
    int bonusDef;
    int bonusHp;

    enum BattleSituation
    {
        Peace = -1,
        BattleNow,
        BattleFlee,
        BattleWin,
        BattleLose,
    }

    enum PlayerActSelect
    {
        Null = -1,
        Cancel,
        Attack,
        Skill,
        Item,
        FailAct = 10,
    }

    BattleSituation battlesituation;
    PlayerActSelect act;

    // 전투에 필요한 정보들
    private int selectIdx;
    private int skillIdx;

    // 반영하는 공격력!
    private int totalDamage;
    private bool isCritical;

    private void BattleScene()
    {
        battlesituation = BattleSituation.Peace;

        BattleCycle();
    }

    private void StageManager()
    {
        // 추후에 구현 예정
    }

    private void BattleCycle()
    {
        // 인카운터 진입
        BeforeBattle();

        // 전투 중
        while (battlesituation == BattleSituation.BattleNow)    // 왜 이렇게 했나요? : 그래야 흐름을 한 눈에 보기 쉽고, 중간 탈출문들을 어디다 넣을 지 감이 바로 잡힌다. 
        {                                                       // 추가적으로 메서드 내에서 return 없이 계속해서 서로 간의 호출을 계속할 경우 호출 스택이 지워지지 않고 그대로 쌓인다.
            ParameterCleaning();
            BattleStartScene();
            if (battlesituation == BattleSituation.BattleFlee) { break;}
            switch ((int)act)   // 0 취소, 1 공격, 2 스킬. 3 아이템 10. 특정 행동 실패
            {
                case 0:
                    continue;
                case 1: // 기본 공격이라 선택의 이유가 없다.
                    break;
                case 2:
                    SkillSelectScene();
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("아직 구현 중인 기능입니다!");
                    Console.WriteLine();
                    Console.WriteLine("엔터를 눌러 계속 진행하세요!");
                    Console.ReadLine();
                    continue;
                case 10:
                    Console.WriteLine(" 엔터를 눌러 계속... ");
                    Console.ReadLine();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("구현되지 않은 입력입니다.!");
                    Console.ReadLine();
                    continue;
            }
            if (act == PlayerActSelect.Cancel) { continue; }    // 캔슬! 반복문 처음부터
            else if (act == PlayerActSelect.Attack || act == PlayerActSelect.Skill)  // 스킬을 선택했다면 스킬 선택지 열기
            { 
                EnemySelectScene(); if (act == PlayerActSelect.Cancel) { continue; } // 캔슬! 반복문 처음부터
            }
            // 플레이어의 행동 결과!
            PlayerResultScene(); if (battlesituation == BattleSituation.BattleLose || battlesituation == BattleSituation.BattleWin) { break; }

            foreach (Monster monster in monsters)
            {
                if (monster.IsDead == true) { continue; }   // 죽은 적은 아무 행동도 하지 않습니다.
                EnemyBattleResultScene(monster); if (battlesituation == BattleSituation.BattleLose || battlesituation == BattleSituation.BattleWin) { break; }
            }
        } // 전투 중의 마지막 싸이클

        // 전투 결과
        FinalBattleResultScene();

        Console.WriteLine("");
        Console.WriteLine("0. 다음");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                Console.WriteLine("마을로 복귀 중입니다..");
                Thread.Sleep(1000);
                return;
            default:
                break;
        }
    }

    private void ParameterCleaning()
    {
        selectIdx = -1;
        skillIdx = -1;
        totalDamage = 0;
        isCritical = false;
}

    private void BeforeBattle()
    {
        MonsterSpawn();

        Console.Clear();

        ConsoleUtility.ShowTitle("■ 적들이 등장했습니다! ■");

        MonsterAndPlayerStatus();

        Console.WriteLine("");
        Console.WriteLine("원하는 행동을 선택해주십시오!");
        ConsoleUtility.PrintTextHighlights("", "지금이라면 안전하게 도주할 수 있습니다!");

        Console.WriteLine("");
        Console.WriteLine("1. 전투 돌입!");
        Console.WriteLine("2. 도주!!");
        Console.WriteLine("");

        while (true)
        {
            switch (ConsoleUtility.PromptSceneChoice(1, 2))
            {
                case 1:
                    battlesituation = BattleSituation.BattleNow; return;
                case 2:
                    battlesituation = BattleSituation.BattleFlee; return;
                default:
                    break;
            }
        }
    }

    // 전투 전 몬스터를 랜덤하게 생성하는 씬
    private void MonsterSpawn()
    {
        initialPlayerHp = player.Hp;

        monsters = new List<Monster>();

        randomNumber = random.Next(1, 5);   // random.Next(1, 5) = 1 ~ 4
        for (int i = 0; i < randomNumber; i++)
        {
            // Enum.GetNames(typeof(MonsterType)).Length = 3
            // random.Next(3) = 0 ~ 2
            MonsterType monsterType = (MonsterType)random.Next(Enum.GetNames(typeof(MonsterType)).Length);

            switch (monsterType)    // 이거 나중에 몬스터로 다 챙겨가자.
            {
                case MonsterType.Minion:
                    monsters.Add(new Monster("미니언", 2, 15, 5));
                    break;
                case MonsterType.VoidSwarm:
                    monsters.Add(new Monster("공허충", 3, 10, 9));
                    break;
                case MonsterType.SiegeMinion:
                    monsters.Add(new Monster("대포미니언", 5, 25, 8));
                    break;
            }
        }
    }

    // 플레이어의 행동을 선택하는 씬
    private void BattleStartScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        MonsterAndPlayerStatus();

        Console.WriteLine("");
        Console.WriteLine("1. 공격");
        Console.WriteLine("2. 스킬");
        Console.WriteLine("3. 아이템");
        Console.WriteLine("4. 도주");
        Console.WriteLine("");

        act = PlayerActSelect.Null;

        switch (ConsoleUtility.PromptSceneChoice(1, 4))
        {
            case 1:
                act = PlayerActSelect.Attack;
                break;
            case 2:
                act = PlayerActSelect.Skill;
                break;
            case 3:
                act = PlayerActSelect.Item;
                break;
            case 4:
                FleeSelectScene();
                break;
            default:
                break;
        }
    }

    private void FleeSelectScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine();
        ConsoleUtility.PrintTextHighlights("정말로 도주를 시도하시겠습니까? ", "확률이 낮습니다!");

        Console.WriteLine();
        Console.WriteLine("0. 네 1. 아니오");
        Console.WriteLine();

        switch (ConsoleUtility.PromptSceneChoice(0, 1))
        {
            case 0:
                int fleeChance = random.Next(0, 101);
                if (10 > fleeChance)
                {
                    battlesituation = BattleSituation.BattleFlee;
                }
                else
                {
                    act = PlayerActSelect.FailAct;
                    ConsoleUtility.PrintTextHighlights("", "도주에 실패하셨습니다!");
                }
                return;
            case 1:
                act = PlayerActSelect.Cancel;
                return;
        }

    }

    // 스킬 선택 씬
    private void SkillSelectScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        MonsterAndPlayerStatus();

        Console.WriteLine("");
        skills.SkillSelect();   // 스킬 선택창 보여주기

        Console.WriteLine("");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");

        // 스킬 선택
        skillIdx = ConsoleUtility.PromptSceneChoice(0, skills.SkillList.Count);

        switch (skillIdx)
        {
            case 0:
                act = PlayerActSelect.Cancel;
                break;
            default:
                break;
        }   // 스킬 선택
    }

    // 적 선택 씬
    private void EnemySelectScene(string? prompt = null)
    {
        if (prompt != null)
        {
            // 1초간 메시지를 띄운 다음에 다시 진행
            Console.Clear();
            ConsoleUtility.ShowTitle(prompt);
            Thread.Sleep(300);
        }

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].PrintMonsterDescription(true, i + 1);
        }

        Console.WriteLine("");
        Console.WriteLine("공격하고 싶은 적을 선택하세요!");

        Console.WriteLine("");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");

        // 몬스터 선택
        while (true)
        {
            selectIdx = ConsoleUtility.PromptSceneChoice(0, monsters.Count);
            switch (selectIdx)
            {
                case 0:
                    act = PlayerActSelect.Cancel;
                    return;
                default:
                    if (monsters[selectIdx - 1].IsDead)
                    {
                        EnemySelectScene("이미 사망한 적입니다!");
                        return;
                    }
                    else
                    {
                        selectIdx--;    // 매우 중요한 항목이다! (몬스터인덱싱 때문에)
                        PlayerDamageCaculate();
                        return;
                    }
            }
        }
    }
    // 전투 관련 씬
    private void PlayerResultScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        if (act == PlayerActSelect.Skill && skillIdx != -1) // 스칼 사용을 선택했을 시
        {
            skills.SkillUse(monsters, selectIdx, player, totalDamage, skillIdx, isCritical);
        }
        else if (act == PlayerActSelect.Attack) // 공격 사용을 선택했을 시
        {
            Monster target = monsters[selectIdx];
            player.PrintAttackDescription(target, totalDamage, isCritical);
            Console.WriteLine("");
            target.PrintMonsterChangeDescription(target.Hp, totalDamage);
        }
        else { Console.WriteLine("당신은 이미 턴을 소모하셨습니다..."); } // 아이템 사용 또는 도주 실패 시

        Console.WriteLine("");
        Console.WriteLine("계속 진행하려면 엔터를 눌러주세요."); // 바꾼 이유. 유저 입장에서는 2번 눌러줘야해서 귀찮다
        Console.WriteLine("");

        Console.ReadLine();
        if (monsters.All(monster => monster.IsDead))
        {
            // 모든 몬스터가 죽었으면 최종 결과 씬으로 이동
            battlesituation = BattleSituation.BattleWin;
        }
    }

    // Monster의 행동 턴!
    private void EnemyBattleResultScene(Monster enemy)
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Enemy ■");

        int DodgeChance = random.Next(0, 101);  // 나중에 따로 메서드로 넣을 지 고민되는 부분...
        if (50 > DodgeChance)
        {
            Console.WriteLine("");
            ConsoleUtility.PrintTextHighlights("Lv.", enemy.Level.ToString(), "", false);
            Console.WriteLine($" {enemy.Name} 의 공격!");
            ConsoleUtility.PrintTextHighlights($"{player.Name}이(가) 공격을 피했습니다! ",  "Dodge!");
        }
        else
        {
            Console.WriteLine("");
            enemy.PrintAttackDescription(player);
            Console.WriteLine("");
            player.PrintPlayerChangeDescription(player.Hp, enemy.Atk);
        }

        Console.WriteLine("");
        Console.WriteLine("계속 진행하려면 엔터를 눌러주세요.");
        Console.WriteLine("");

        Console.ReadLine();
        if (player.IsDead)
        {
            // 플레이어가 죽었으면 최종 결과 씬으로 이동
            battlesituation = BattleSituation.BattleLose;
        }
    }

    private void FinalBattleResultScene()
    {

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Result ■");

        Console.WriteLine("");
        if (battlesituation == BattleSituation.BattleWin)
        {
            // 플레이어 승리
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Victory");
            Console.ResetColor();
            Console.WriteLine("");
            ConsoleUtility.PrintTextHighlights("던전에서 몬스터 ", monsters.Count.ToString(), "마리를 잡았습니다.");
            player.EarnExp(monsters.Count, skills); // 몬스터를 죽인 수만큼 경험치 획득
        }
        else if (battlesituation == BattleSituation.BattleFlee)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("전투에서 후퇴하셨습니다!");
            Console.ResetColor();
        }
        else if (battlesituation == BattleSituation.BattleLose)
        {
            // 플레이어 패배
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You Lose");
            Console.ResetColor();
        }

        Console.WriteLine("");
        player.PrintPlayerChangeDescription(initialPlayerHp);
    }

    // 출력 도우미

    private void MonsterAndPlayerStatus()
    {
        Console.WriteLine("");
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].PrintMonsterDescription();
        }

        Console.WriteLine("");
        player.PrintPlayerDescription(bonusHp);
    }


    // 전투에 사용되는 메서드

    // 플레이어 공격력 계산 (스킬도 이 공격을 포함시켜준다!)
    private void PlayerDamageCaculate()
    {
        int damage = player.Atk + bonusAtk;
        int damageMargin = (int)Math.Ceiling(damage * 0.1f);
        int criticalChance = random.Next(0, 101);
        if (50 > criticalChance)  // 여기다가 나중에 플레이어의 치명타 확률을 넣으시면 됩니다.
        {
            totalDamage = damage * 2; // 일단 무조건 2배 (이후에 치명타 데미지도 고민할 사항)
            isCritical = true;
        }
        else
        {
            totalDamage = random.Next(damage - damageMargin, damage + damageMargin + 1); // 최종 공격력 반영
        }
    }
}
