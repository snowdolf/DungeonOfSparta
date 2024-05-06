internal class Player
{
    public string Name { get; set; }
    public string Job { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Atk { get; set; }
    public int BonusAtk { get; set; }
    public int Def { get; set; }
    public int BonusDef { get; set; }
    public int Hp { get; set; }
    public int BonusHp { get; set; }
    public int Gold { get; set; }
    public int Potion { get; set; }
    public int MaxStage { get; set; }
    public bool IsDead { get; set; }

    public Player(string name, string job, int level, int atk, int def, int hp, int gold, int potion, bool isDead = false)
    {
        Name = name;
        Job = job;
        Level = level;
        Atk = atk;
        Def = def;
        Hp = hp;
        Gold = gold;
        Potion = potion;
        MaxStage = 1;
        IsDead = isDead;
    }

    // 물약 전용 오버로딩
    public void PrintPlayerDescription(int bonusHp)
    {
        Console.WriteLine("[내정보]");
        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), $" {Name} ( {Job} )");
        ConsoleUtility.PrintTextHighlights("HP ", Hp.ToString(), "", false);
        ConsoleUtility.PrintTextHighlights("/", (100 + BonusHp).ToString());
    }

    // BattleScene에서 player 정보 출력할 때 사용
    public void PrintPlayerDescription()
    {
        Console.WriteLine("[내정보]");
        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), $" {Name} ( {Job} )");
        ConsoleUtility.PrintTextHighlights("HP ", Hp.ToString(), "", false);
        ConsoleUtility.PrintTextHighlights("/", (100 + BonusHp).ToString());
        ConsoleUtility.PrintTextHighlights("공격력 : ", (Atk + BonusAtk).ToString(), " ", false); ConsoleUtility.PrintTextHighlights("방어력 : ", (Def + BonusDef).ToString());
    }

    // BattleScene에서 player 정보 변화를 출력할 때 사용
    public void PrintPlayerChangeDescription(int initialHp, int damage = 0)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            IsDead = true;
            Hp = 0;
        }

        ConsoleUtility.PrintTextHighlights("Lv.", Level.ToString(), $" {Name}");
        ConsoleUtility.PrintTextHighlights("HP ", initialHp.ToString(), " -> ", false);
        if (IsDead)
        {
            Console.WriteLine("Dead");
        }
        else
        {
            ConsoleUtility.PrintTextHighlights("", Hp.ToString());
        }
    }

    // BattleScene에서 player가 monster를 공격할 때 사용
    public void PrintAttackDescription(Monster monster, int damage, bool critical)
    {
        if (critical) { ConsoleUtility.PrintTextHighlights("", $" {Name} 의 치명적인 공격! Critical!"); }
        else { Console.WriteLine($" {Name} 의 공격!"); }
        ConsoleUtility.PrintTextHighlights("Lv.", monster.Level.ToString(), "", false);
        Console.Write($" {monster.Name} 을(를) 맞췄습니다. ");
        ConsoleUtility.PrintTextHighlights("[데미지 : ", damage.ToString(), "]");
    }

    public void EarnLoot(int stage, List<Monster> monsters) // 보상 획득 메서드입니다.
    {
        Console.WriteLine();
        int monsterLoot = 0;
        foreach (Monster monster in monsters)
        {
            monsterLoot += 50 * (int)monster.MonstersType; // 몬스터 등급이 높을수록 보상 증가
        }
        int totalRewards = (stage * 250) + monsterLoot;
        int previousGold = Gold;
        Gold += totalRewards;
        ConsoleUtility.PrintTextHighlights("", $"{totalRewards}", "G 만큼의 금액을 획득하셨습니다!");
        ConsoleUtility.PrintTextHighlights("보유 금액 : ", $"{previousGold} - > {Gold}", "G");
        if (stage == 5)
        {
            Potion += stage;
            ConsoleUtility.PrintTextHighlights("보스 스테이지 추가 보상으로 ", $"물약 {stage}개", "를 획득하셨습니다!");
        }
    }

    public void EarnExp(int exp, Skill skills) // 경험치 획득 메서드입니다.
    {
        Console.Write($"{exp}만큼의 경험치를 획득했습니다! ");
        ConsoleUtility.PrintTextHighlights("", $"{Exp}", "/", false);
        ConsoleUtility.PrintTextHighlights("", $"{Level * 5}", " -> ", false);
        ConsoleUtility.PrintTextHighlights("", $"{Exp + exp}", "/", false);
        ConsoleUtility.PrintTextHighlights("", $"{Level * 5}");
        Exp += exp;
        if (Exp >= 5 * Level) // 경험치가 오르면!
        {
            Exp = Exp - 5 * Level; // 요구되는 경험치만큼 감소
            Level++;    // 레벨 증가
            LevelUpRewards(Level, skills);  // 레벨업 시 레벨 보상 메서드입니다.
        }
    }

    void LevelUpRewards(int level, Skill skills) // 레벨 보상 메서드입니다.
    {
        ConsoleUtility.PrintTextHighlights("", "\n레벨이 올랐습니다!");

        ConsoleUtility.PrintTextHighlights("\n공격력 증가! ", $"{Atk}", " -> ", false);
        ConsoleUtility.PrintTextHighlights("", $"{Atk + 2}");
        ConsoleUtility.PrintTextHighlights("방어력 증가! ", $"{Def}", " -> ", false);
        ConsoleUtility.PrintTextHighlights("", $"{Def + 2}");

        switch (level)  // 레벨별로 얻는 보상을 나눴습니다.
        {
            case 2:
                Atk += 2;
                Def += 2;
                skills.SkillEarn(SkillName.Slash); // 휩쓸기
                break;
            case 3:
                Atk += 2;
                Def += 2;
                skills.SkillEarn(SkillName.RevengeAttack); // 복수
                break;
            case 4:
                Atk += 2;
                Def += 2;
                break;
            case 5:
                Atk += 2;
                Def += 2;
                skills.SkillEarn(SkillName.CounterAttack); // 반격
                break;
            case 6:
                Atk += 2;
                Def += 2;
                skills.SkillEarn(SkillName.MoneyAttack); // 동전 던지기
                break;
            default:
                Console.WriteLine("추후에 업데이트할 예정입니다.");
                Atk += 2;
                Def += 2;
                break;
        }
    }

    public void CheckUnlockStage(int stage)
    {
        if(stage == MaxStage)
        {
            ConsoleUtility.PrintTextHighlights("\n최고 스테이지를 돌파하였습니다! ", $"{MaxStage}", " -> ", false);
            ConsoleUtility.PrintTextHighlights("", $"{++MaxStage}");
        }
    }

    // 장착된 아이템 해제하거나 판매할 때 최대체력 넘어가는 버그
    // 120/120 -> 120/100
    // player.CheckHpError(inventory) <- 에러 예상 구역에 추가함
    public void CheckHpError(List<Item> inventory)
    {
        BonusHp = inventory.Select(item => item.IsEquipped ? item.Hp : 0).Sum();
        if (Hp > 100 +  BonusHp)
        {
            Hp = 100 + BonusHp;
        }
    }
}