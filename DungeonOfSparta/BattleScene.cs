﻿using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;

enum MonsterType
{
    Minion,         // 미니언
    MeleeMinion,    // 전사미니언
    SiegeMinion,    // 대포미니언
    SuperMinion,    // 슈퍼미니언
    BlueSentinel,   // 푸른파수꾼
    RedBrambleback, // 붉은덩굴정령
    Dragon,         // 드래곤
    BaronNashor,     // 내셔남작
    Krugs = 10,      // 돌거북
}

public partial class GameManager
{
    private List<Monster> monsters;

    Random random = new Random();

    int randomNumber;

    // 던전 입장 전 player 체력
    // FinalBattleResultScene 에서 활용될 예정
    int initialPlayerHp;

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

    // 스테이지 선택
    int stage;

    // 재귀 호출 피하기
    bool isBattleCycle;

    // 아이템 사용 시 출력하게할 메시지
    string UseItemText;

    // 방어형 스킬 유무
    bool isDefensiveSkill;

    List<Monster> DeadDeathRattleMonsters = new List<Monster>();    // 죽음의 메아리 있는 몬스터들이 죽었을 경우 모으는 목록

    private void BattleScene()
    {
        // 낀 장비 적용하기
        player.BonusAtk = inventory.Select(item => item.IsEquipped ? item.Atk : 0).Sum();
        player.BonusDef = inventory.Select(item => item.IsEquipped ? item.Def : 0).Sum();
        player.BonusHp = inventory.Select(item => item.IsEquipped ? item.Hp : 0).Sum();

        isBattleCycle = false;
        StageSelectScene();
        while (isBattleCycle)
        {
            BattleCycle();
        }

        MainScene();
    }

    // 스테이지를 선택하는 씬
    private void StageSelectScene()
    {
        battlesituation = BattleSituation.Peace;

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Stage Select ■");

        Console.WriteLine("");
        Console.Write("전투 시작 전 스테이지를 선택하세요. ");
        ConsoleUtility.PrintTextHighlights("(현재 목표 : ", player.MaxStage.ToString(), " 층)");

        Console.WriteLine("");
        player.PrintPlayerDescription();

        Console.WriteLine("");
        if (player.MaxStage == 1)
        {
            Console.WriteLine("1. 스테이지 입력");
        }
        else
        {
            Console.WriteLine($"1 ~ {player.MaxStage}. 스테이지 입력");
        }
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");

        int keyInput = ConsoleUtility.PromptSceneChoice(0, player.MaxStage);

        switch (keyInput)
        {
            case 0:
                isBattleCycle = false;
                break;
            default:
                stage = keyInput;
                isBattleCycle = true;
                break;
        }
    }

    private void BattleCycle()
    {
        // 인카운터 진입
        BeforeBattle();

        // 전투 중
        while (battlesituation == BattleSituation.BattleNow)    // 왜 이렇게 했나요? : 그래야 흐름을 한 눈에 보기 쉽고, 중간 탈출문들을 어디다 넣을 지 감이 바로 잡힌다. 
        {                                                       // 추가적으로 메서드 내에서 return 없이 계속해서 서로 간의 호출을 계속할 경우 호출 스택이 지워지지 않고 그대로 쌓인다.
            BattleStartScene();
            if (battlesituation == BattleSituation.BattleFlee) { break; }
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
                    ItemSelectScene();
                    break;
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
            else if (act == PlayerActSelect.Attack || act == PlayerActSelect.Skill && !isDefensiveSkill)    // 공격이거나, 방어형 스킬이 아닌 스킬인 경우 (&&일 경우 서로 연결되고 ||일 경우 따로 떨어지는 것 같다.)
            {
                EnemySelectScene(); if (act == PlayerActSelect.Cancel) { continue; } // 캔슬! 반복문 처음부터
            }
            // 플레이어의 행동 결과!
            PlayerResultScene();

            foreach (Monster monster in monsters)   // 적 턴!
            {
                if (monster.IsDead == true) { if (monster.DeathRattle) { DeadDeathRattleMonsters.Add(monster); } continue; }   // 죽은 적은 아무 행동도 하지 않습니다. (특수 몬스터들은 예외)
                EnemyBattleResultScene(monster);    // 적의 행동
                if (monster.IsDead == true) { if (monster.DeathRattle) { DeadDeathRattleMonsters.Add(monster); } }   // 반격기로 죽을 경우에도 추가 
            }

            // 죽음의 메아리 발동!
            foreach (Monster monster in DeadDeathRattleMonsters)    // 왜 따로 나눴나요? : 몬스터 리스트가 반복의 조건문이 되는 동안 해당 리스트 수정이 불가능하기 때문에
            {
                EnemyDeathRattleResultScene(monster);
            }

            // 각종 매개변수 초기화 및 스킬 쿨타임 감소
            ParameterCleaning();

            // 승리, 패배 조건 확인 - 왜 옮겼나요? : 죽음의 메아리 때문에
            if (player.IsDead)
            {
                // 플레이어가 죽었으면 최종 결과 씬으로 이동
                battlesituation = BattleSituation.BattleLose;
            }
            else if (monsters.All(monster => monster.IsDead)) 
            {
                // 모든 몬스터가 죽었으면 최종 결과 씬으로 이동
                battlesituation = BattleSituation.BattleWin;
            }
        }

        // 전투 결과
        FinalBattleResultScene();

        Console.WriteLine("");

        if (battlesituation == BattleSituation.BattleWin)
        {
            Console.WriteLine("계속 진행하시겠습니까?");
            stage++;
            ConsoleUtility.PrintTextHighlights($"다음 스테이지 : ", $"stage {stage}");
        }

        Console.WriteLine("");
        Console.WriteLine("0. 마을로");
        if (battlesituation == BattleSituation.BattleWin) { Console.WriteLine("1. 다음 스테이지로!"); }
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 1))
        {
            case 0:
                Console.WriteLine("마을로 복귀 중입니다..");
                Thread.Sleep(1000);
                isBattleCycle = false;
                return;
            case 1:
                if (battlesituation != BattleSituation.BattleWin)
                {
                    Console.WriteLine("마을로 복귀 중입니다..");
                    Thread.Sleep(1000);
                    isBattleCycle = false;
                    return;
                }
                Console.WriteLine("다음 스테이지로 넘어가는 중...");
                Thread.Sleep(1000);
                isBattleCycle = true;
                return;
            default:
                break;
        }
    }

    // -1은 int에서 선택을 안했다라는 의미를 나타냄과 idx 선택을 제대로 반영을 했는지 일부러 오류를 일으키기 위해 사용됩니다. 
    private void ParameterCleaning()
    {
        selectIdx = -1;      
        skillIdx = -1;
        totalDamage = 0;
        isCritical = false;
        isDefensiveSkill = false;

        DeadDeathRattleMonsters.Clear();    // 죽메 효과 치우기

        foreach (Skill skill in skills.SkillList)
        {
                skill.CoolTime -= 1; // 스킬 쿨타임 감소!
        }
    }

    private void BeforeBattle()
    {
        MonsterSpawn(); // 몬스터 스폰

        Console.Clear();

        if (stage == 5) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("■ 보스 스테이지입니다! ■"); Console.ResetColor(); }
        else { ConsoleUtility.ShowTitle("■ 적들이 등장했습니다! ■"); }
        
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

        if (stage == 5) // 보스 스테이지
        {
            monsters.Add(new AncientKrug());
            monsters.Add(new Krug());
        }
        else
        { 
            // 높은 스테이지일수록 더 많은 몬스터 등장 
            randomNumber = random.Next(1, 4 + stage / 3);   // randomNumber = 1 ~ (3 + stage / 3)
            for (int i = 0; i < randomNumber; i++)
            {
                // Enum.GetNames(typeof(MonsterType)).Length = 8
                // random.Next(8) = 0 ~ 7

                // 몬스터 레벨이 스테이지 이상이어야 등장
                int monsterMaxIdx = Enum.GetNames(typeof(MonsterType)).Length;
                int monsterMinIdx = 0;  // 최소 보정치
                if (stage < 5)
                {
                    monsterMaxIdx = stage;
                }
                else if (stage < 7)
                {
                    monsterMinIdx = 2;
                    monsterMaxIdx = 6;
                }
                else if (stage < 10)
                {
                    monsterMinIdx = 3;
                    monsterMaxIdx = 7;
                }
                else
                {
                    monsterMinIdx = 4;
                }

                MonsterType monsterType = (MonsterType)random.Next(monsterMinIdx, monsterMaxIdx);

                switch (monsterType)
                {
                    case MonsterType.Minion:
                        monsters.Add(new Monster("미니언", 1, 15, 5, MonsterType.Minion));
                        break;
                    case MonsterType.MeleeMinion:
                        monsters.Add(new Monster("전사미니언", 2, 25, 5, MonsterType.MeleeMinion));
                        break;
                    case MonsterType.SiegeMinion:
                        monsters.Add(new Monster("대포미니언", 3, 25, 10, MonsterType.SiegeMinion));
                        break;
                    case MonsterType.SuperMinion:
                        monsters.Add(new Monster("슈퍼미니언", 4, 30, 8, MonsterType.SuperMinion));
                        break;
                    case MonsterType.BlueSentinel:
                        monsters.Add(new Monster("푸른파수꾼", 5, 40, 15, MonsterType.BlueSentinel));
                        break;
                    case MonsterType.RedBrambleback:
                        monsters.Add(new Monster("붉은덩굴정령", 5, 40, 15, MonsterType.RedBrambleback));
                        break;
                    case MonsterType.Dragon:
                        monsters.Add(new Monster("드래곤", 7, 70, 20, MonsterType.Dragon));
                        break;
                    case MonsterType.BaronNashor:
                        monsters.Add(new Monster("내셔남작", 10, 100, 30, MonsterType.BaronNashor));
                        break;
                }
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

    // 아이템 사용씬
    private void ItemSelectScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine();
        Console.WriteLine("■ 체력 포션 ■");
        Console.WriteLine("포션을 사용하면 체력을 30 회복할 수 있습니다. ");
        ConsoleUtility.PrintTextHighlights("(남은 포션 : ", player.Potion.ToString(), " )");

        Console.WriteLine("");
        player.PrintPlayerDescription();

        Console.WriteLine("");
        Console.WriteLine("1. 사용하기");
        Console.WriteLine("0. 나가기");
        Console.WriteLine("");

        while (true)
        {
            switch (ConsoleUtility.PromptSceneChoice(0, 1))
            {
                case 0:
                    act = PlayerActSelect.Cancel;
                    return;
                case 1:
                    // 보유 포션이 충분하다면
                    UseItemText = $"체력을 회복했습니다! {player.Hp}";
                    if (player.Potion > 0)
                    {
                        player.Hp += 30;
                        if (player.Hp > (100 + player.BonusHp))
                        {
                            player.Hp = 100 + player.BonusHp;
                        }
                        player.Potion--;
                     UseItemText += $" -> {player.Hp}";
                        return;
                    }
                    // 보유 포션이 부족하다면
                    Console.WriteLine("포션이 부족합니다.");
                    continue;
            }
        }
    }

    // 도주 재확인 씬
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
                skillIdx--; // 인덱싱에 혼란스러우니 여기서 감소할 것
                if (skills.SkillList[skillIdx].CoolTime != 0)
                {
                    ConsoleUtility.PrintTextHighlights("", "해당 스킬은 아직 쿨타임이 남아있습니다!");
                    Thread.Sleep(500);
                    act = PlayerActSelect.Cancel;
                }
                else if (skills.SkillList[skillIdx].DefensiveSkill) // 방어형 스킬일 경우
                {
                    isDefensiveSkill = true;
                }
                else if (skills.SkillList[skillIdx].ConditionCheck) // 조건이 필요한 스킬일 경우
                {
                    if (skills.SkillList[skillIdx].ConditionCheckMethod(player) == false)
                    {
                        Thread.Sleep(500);
                        act = PlayerActSelect.Cancel;
                        return;
                    }
                }
                
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
                        selectIdx--;    // 몬스터 인덱싱 때문에
                        PlayerDamageCaculate();
                        return;
                    }
            }
        }
    }


     // 플레이어의 선택 결과 씬
    private void PlayerResultScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        if (isDefensiveSkill)   // 반격기일 경우
        {
            skills.SkillList[skillIdx].DefensiveSkillReady(player);
        }
        else if (act == PlayerActSelect.Skill && skillIdx != -1) // 스킬 사용을 선택했을 시
        {
            skills.SkillUse(monsters, selectIdx, player, totalDamage, skillIdx, isCritical);
            MonsterCountForQuest();
        }
        else if (act == PlayerActSelect.Attack) // 공격 사용을 선택했을 시
        {
            Monster target = monsters[selectIdx];
            player.PrintAttackDescription(target, totalDamage, isCritical);
            Console.WriteLine("");
            target.PrintMonsterChangeDescription(target.Hp, totalDamage);
            MonsterCountForQuest();
        }
        else if (act == PlayerActSelect.Item) // 템 사용을 선택했을 시
        {
            Console.WriteLine("당신은 템 사용에 턴을 소모하였습니다!");
            Console.WriteLine("");
            ConsoleUtility.PrintTextHighlights("", UseItemText);
        }
        else { Console.WriteLine("당신은 이미 턴을 소모하셨습니다..."); } // 도주 실패 시

        



        Console.WriteLine("");
        Console.WriteLine("계속 진행하려면 엔터를 눌러주세요."); // 바꾼 이유. 유저 입장에서는 2번 눌러줘야해서 귀찮다
        Console.WriteLine("");

        Console.ReadLine();
    }

    // Monster의 행동 턴!
    private void EnemyBattleResultScene(Monster enemy)
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Enemy ■");

        int DodgeChance = random.Next(0, 101);  // 회피 판정
        if (isDefensiveSkill)   // 반격기나 방어기인 경우
        {
            List<Monster> target = new List<Monster>() { enemy };   // 단일 대상 시 (나중에 범위 공격 논리값도 나눠야할 것 같다;)

            Console.WriteLine();
            Console.WriteLine($" {enemy.Name}의 공격!");
            PlayerDamageCaculate(); // 반격 데미지 재계산
            skills.SkillUse(target, selectIdx, player, totalDamage, skillIdx, isCritical);
        }
        else if (50 > DodgeChance)  // 회피 성공 시
        {
            Console.WriteLine("");
            ConsoleUtility.PrintTextHighlights("Lv.", enemy.Level.ToString(), "", false);
            Console.WriteLine($" {enemy.Name}의 공격!");
            ConsoleUtility.PrintTextHighlights($"{player.Name}이(가) 공격을 피했습니다! ",  "Dodge!");
        }
        else
        {
            int damage = enemy.Atk - (player.Def + player.BonusDef) / 5;
            if (damage <= 0)
            {
                damage = 1;
            }

            Console.WriteLine("");
            enemy.PrintAttackDescription(player, damage);
            Console.WriteLine("");
            player.PrintPlayerChangeDescription(player.Hp, damage);
        }

        Console.WriteLine("");
        Console.WriteLine("계속 진행하려면 엔터를 눌러주세요.");
        Console.WriteLine("");

        Console.ReadLine();
    }

    // 몬스터 죽음의 메아리 씬
    private void EnemyDeathRattleResultScene(Monster enemy)
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Enemy ■");

        enemy.DeathRattleActive(monsters);

        Console.WriteLine("");
        Console.WriteLine("계속 진행하려면 엔터를 눌러주세요.");
        Console.WriteLine("");

        Console.ReadLine();
    }

    private void FinalBattleResultScene()   // 전투 최종 결과
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
            player.EarnLoot(stage, monsters);   // 보상 획득
            player.CheckUnlockStage(stage);
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

        foreach (Skill skill in skills.SkillList)
        {
            skill.CoolTime = 0;     // 모든 스킬 쿨 초기화!
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
        player.PrintPlayerDescription();
    }


    // 전투에 사용되는 메서드

    // 플레이어 공격력 계산 (스킬도 이 공격을 포함시켜준다!)
    private void PlayerDamageCaculate()
    {
        int damage = player.Atk + player.BonusAtk;
        int damageMargin = (int)Math.Ceiling(damage * 0.1f);
        int criticalChance = random.Next(0, 101);
        if (50 > criticalChance)  // 여기다가 나중에 플레이어의 치명타 확률을 넣으시면 됩니다.
        {
            totalDamage = damage * 2; // 일단 무조건 2배 (이후에 치명타 데미지도 고민할 사항)
            isCritical = true;
        }
        else
        {
            isCritical = false;     // 평타일 경우 당연히 이 판정이 들어가줘야한다.
            totalDamage = random.Next(damage - damageMargin, damage + damageMargin + 1); // 최종 공격력 반영
        }
    }
}
