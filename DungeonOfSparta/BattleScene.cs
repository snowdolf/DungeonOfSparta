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

    // 스테이지 선택
    int stage;

    // 스테이지를 선택하는 씬
    private void StageSelectScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Stage Select ■");

        Console.WriteLine("");
        Console.Write("전투 시작 전 스테이지를 선택하세요. ");
        ConsoleUtility.PrintTextHighlights("(현재 목표 : ", player.MaxStage.ToString(), " 층)");

        Console.WriteLine("");
        if(player.MaxStage == 1)
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
                MainScene();
                break;
            default:
                stage = keyInput;
                BattleScene();
                break;
        }

        BattleScene();
    }

    // 전투 전 몬스터를 랜덤하게 생성하는 씬
    private void BattleScene()
    {
        initialPlayerHp = player.Hp;

        bonusAtk = inventory.Select(item => item.IsEquipped ? item.Atk : 0).Sum();
        bonusDef = inventory.Select(item => item.IsEquipped ? item.Def : 0).Sum();
        bonusHp = inventory.Select(item => item.IsEquipped ? item.Hp : 0).Sum();

        monsters = new List<Monster>();

        randomNumber = random.Next(1, 5);   // random.Next(1, 5) = 1 ~ 4
        for (int i = 0; i < randomNumber; i++)
        {
            // Enum.GetNames(typeof(MonsterType)).Length = 3
            // random.Next(3) = 0 ~ 2
            MonsterType monsterType = (MonsterType)random.Next(Enum.GetNames(typeof(MonsterType)).Length);

            switch (monsterType)
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

        BattleStartScene();
    }

    // 플레이어의 행동을 선택하는 씬
    // 일단은 기본공격만 구현
    private void BattleStartScene()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].PrintMonsterDescription();
        }

        Console.WriteLine("");
        player.PrintPlayerDescription(bonusHp);

        Console.WriteLine("");
        Console.WriteLine("1. 공격");
        Console.WriteLine("2. 스킬");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(1, 2))
        {
            case 1:
                MyBattlePhaseScene();
                break;
            case 2:
                MyBattlePhaseSceneSkillSelect();  // 스킬 선택 씬으로 이동
                break;
            default:
                break;
        }
    }

    // 플레이어가 공격할 대상을 선택하는 씬
    private void MyBattlePhaseScene(string? prompt = null) 
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
        player.PrintPlayerDescription(bonusHp);

        Console.WriteLine("");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");

        int keyInput = ConsoleUtility.PromptSceneChoice(0, monsters.Count);

        switch (keyInput)
        {
            case 0:
                BattleStartScene();
                break;
            default:
                if (monsters[keyInput - 1].IsDead)
                {
                    MyBattlePhaseScene("이미 죽은 몬스터입니다.");
                }
                else
                {
                    int damage = player.Atk + bonusAtk;
                    int damageMargin = (int)Math.Ceiling(damage * 0.1f);
                    
                    // 공격 대상 및 데미지를 넘겨준다
                    MyBattleResultScene(keyInput - 1, random.Next(damage - damageMargin, damage + damageMargin + 1));
                }
                break;
        }
    }

    // 스킬 관련
    private void MyBattlePhaseSceneSkillSelect()
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        player.PrintPlayerDescription();

        Console.WriteLine("");
        skills.SkillSelect();

        Console.WriteLine("");
        Console.WriteLine("0. 취소");
        Console.WriteLine("");

        // 스킬 선택
        int keyInput = ConsoleUtility.PromptSceneChoice(0, skills.SkillList.Count);

        switch (keyInput)
        {
            case 0:
                BattleStartScene();
                break;
            default:
                MyBattlePhaseSceneSkill(keyInput);
                break;
        }
    }

    private void MyBattlePhaseSceneSkill(int skillIdx, string? prompt = null)
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

        int keyInput = ConsoleUtility.PromptSceneChoice(0, monsters.Count);
        switch (keyInput)
        {
            case 0:
                MyBattlePhaseSceneSkillSelect();
                break;
            default:
                if (monsters[keyInput - 1].IsDead)
                {
                    MyBattlePhaseSceneSkill(skillIdx, "이미 죽은 몬스터입니다.");
                }
                else
                {
                    int damage = player.Atk + bonusAtk;
                    int damageMargin = (int)Math.Ceiling(damage * 0.1f);

                    // 공격 대상 및 데미지를 넘겨준다
                    MyBattleResultSceneSkill(keyInput - 1, random.Next(damage - damageMargin, damage + damageMargin + 1), skillIdx);
                }
                break;
        }
    }

    // 플레이어의 전투 결과를 보여주는 씬
    private void MyBattleResultScene(int monsterIdx, int damage)
    {
        Monster target = monsters[monsterIdx];

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        Console.WriteLine($"{player.Name} 의 공격!");

        Console.WriteLine("");
        player.PrintAttackDescription(target, damage);

        Console.WriteLine("");
        target.PrintMonsterChangeDescription(target.Hp, damage);

        Console.WriteLine("");
        Console.WriteLine("0. 다음");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                if(monsters.All(monster => monster.IsDead))
                {
                    // 모든 몬스터가 죽었으면 최종 결과 씬으로 이동
                    FinalBattleResultScene(true);
                }
                else
                {
                    // 플레이어 턴이 끝났음
                    // 적 턴 시작
                    // Monster 리스트 안의 0번 monster부터 공격 가능한지 ( 살아있는지 ) 탐색
                    EnemyBattlePhaseScene(0);
                }
                break;
            default:
                break;
        }
    }

    // 스킬 관련 씬
    private void MyBattleResultSceneSkill(int monsterIdx, int damage, int skillIdx)
    {
        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! ■");

        Console.WriteLine("");
        skills.SkillUse(monsters, monsterIdx, player, damage, skillIdx);

        Console.WriteLine("");
        Console.WriteLine("0. 다음");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                if (monsters.All(monster => monster.IsDead))
                {
                    // 모든 몬스터가 죽었으면 최종 결과 씬으로 이동
                    FinalBattleResultScene(true);
                }
                else
                {
                    // 플레이어 턴이 끝났음
                    // 적 턴 시작
                    // Monster 리스트 안의 0번 monster부터 공격 가능한지 ( 살아있는지 ) 탐색
                    EnemyBattlePhaseScene(0);
                }
                break;
            default:
                break;
        }
    }

    // Monster 리스트 안의 모든 monster 공격 가능한지 탐색
    private void EnemyBattlePhaseScene(int monsterIdx)
    {
        if(monsterIdx >= monsters.Count)
        {
            // 모든 몬스터가 공격을 마침
            // 다시 플레이어 행동 선택
            BattleStartScene();
        }
        else
        {
            Monster target = monsters[monsterIdx];

            if(target.IsDead)
            {
                // 해당 index 몬스터가 이미 죽음
                // 다음 순서의 몬스터 공격 가능한지 탐색
                EnemyBattlePhaseScene(monsterIdx + 1);
            }
            else
            {
                // 해당 index 몬스터 공격 진행
                EnemyBattleResultScene(monsterIdx);
            }
        }
    }

    private void EnemyBattleResultScene(int monsterIdx)
    {
        Monster target = monsters[monsterIdx];

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Enemy ■");

        Console.WriteLine("");
        target.PrintAttackDescription(player);

        Console.WriteLine("");
        player.PrintPlayerChangeDescription(player.Hp, target.Atk);

        Console.WriteLine("");
        Console.WriteLine("0. 다음");
        Console.WriteLine("");

        switch (ConsoleUtility.PromptSceneChoice(0, 0))
        {
            case 0:
                if (player.IsDead)
                {
                    // 플레이어가 죽었으면 최종 결과 씬으로 이동
                    FinalBattleResultScene(false);
                }
                else
                {
                    // 다음 순서의 몬스터 공격 가능한지 탐색
                    EnemyBattlePhaseScene(monsterIdx + 1);
                }
                break;
            default:
                break;
        }
    }

    private void FinalBattleResultScene(bool isPlayerWin)
    {

        Console.Clear();

        ConsoleUtility.ShowTitle("■ Battle!! - Result ■");

        Console.WriteLine("");
        if(isPlayerWin)
        {
            // 플레이어 승리
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Victory");
            Console.ResetColor();
            Console.WriteLine("");
            ConsoleUtility.PrintTextHighlights("던전에서 몬스터 ", monsters.Count.ToString(), "마리를 잡았습니다.");
            player.EarnExp(monsters.Count, skills); // 몬스터를 죽인 수만큼 경험치 획득
            player.CheckUnlockStage(stage);
        }
        else
        {
            // 플레이어 패배
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You Lose");
            Console.ResetColor();
        }

        Console.WriteLine("");
        player.PrintPlayerChangeDescription(initialPlayerHp);

        Console.WriteLine("");
        Console.WriteLine("0. 다음");
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
}
