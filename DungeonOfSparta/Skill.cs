using System.Reflection.Emit;
using System.Xml.Linq;
public enum SkillName   // 추가되는 대로 넣어주시면 됩니다.
{
    DoubleAttack,
    Slash,
    RevengeAttack,
    MoneyAttack
}

internal class Skill
{
    public List<Skill> SkillList { get; set; }  // 이곳은 스킬을 모아줄 목록입니다. (참조명).SkillList 를 유의!
    public string Name { get; set; }
    public string SkillDesc { get; set; }

    private int coolTime;
    public int CoolTime 
    { 
        get { return coolTime; }
        set
        {
            coolTime = value;
            if (coolTime < 0)
            {
                coolTime = 0;
            }
        }
    }
    public List<SkillName> SkillForSave { get; set; } // 세이브 때문에 추가

    public virtual void Active(List<Monster> target, int idx, Player player, int damage, bool critical) { }  // 범용성을 위해서 List<Monster>를 가져왔고, player의 정보 자체를 가져와서 플레이어의 특정 스텟을 데미지 계산에 이용하실 수 있습니다.

    public Skill() 
    {
        SkillList = new List<Skill>(); // 참조형 변수들은 아무것도 참조하고 있지 않았을 때 접근 시도를 하면 Null참조 에러 뜹니다.
        SkillForSave = new List<SkillName>();
    }

    public void SkillEarn(SkillName idx)  // 스킬 획득입니다. case에 맞게 스킬을 추가하시면 됩니다.
    {
        switch ((int)idx)
        {
            case 0:
                SkillList.Add(new DoubleAttack());
                SkillForSave.Add(SkillName.DoubleAttack);
                break;
            case 1:
                SkillList.Add(new Slash());
                SkillForSave.Add(SkillName.Slash);
                break;
            case 2:
                SkillList.Add(new RevengeAttack());
                SkillForSave.Add(SkillName.RevengeAttack);
                break;
            case 3:
                SkillList.Add(new MoneyAttack());
                SkillForSave.Add(SkillName.MoneyAttack);
                break;
            default:
                Console.WriteLine("이 번호의 스킬은 존재하지 않습니다!");
                break;
        }
    }

    public void SkillDescriptionOnce(int idx)   // 스킬 보여주기 (한개)
    {
        idx -= 1;
        Console.WriteLine("■ 현재 스킬 ■");
        SkillList[idx].PrintSkillDescription();
    }

    public void SkillDescription()   // 스킬 보여주기
    {
        Console.WriteLine("■ 현재 스킬 목록 ■");

        if (SkillList.Count == 0) { Console.WriteLine("보유 중인 스킬이 없습니다!"); }
        else
        {
            for (int i = 0; i < SkillList.Count; i++)
            {
                SkillList[i].PrintSkillDescription();
            }
        }
    }

    public void SkillSelect()   // 스킬 선택창
    {
        Console.WriteLine("■ 현재 스킬 목록 ■");

        if (SkillList.Count == 0) { Console.WriteLine("보유 중인 스킬이 없습니다!"); }
        else
        {
            for (int i = 0; i < SkillList.Count; i++)
            {
                if (SkillList[i].CoolTime != 0)
                {
                    Console.WriteLine();
                    SkillList[i].PrintSkillDescriptionCoolTime(true, i + 1);
                }
                else
                {
                    Console.WriteLine("");
                    SkillList[i].PrintSkillDescription(true, i + 1);
                }
            }
            Console.WriteLine();
            Console.WriteLine("사용할 스킬을 선택하세요!");
        }
    }

    public void SkillUse(List<Monster> target, int idx, Player player, int damage, int skillIdx, bool critical)    // 스킬 사용 (idx에 유의)
    {
        skillIdx -= 1;
        SkillList[skillIdx].Active(target, idx, player, damage, critical);
    }

    public void PrintSkillDescription(bool withNumber = false, int idx = 0) // 스킬 출력
    {
            if (withNumber)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write($"{idx} ");
                Console.ResetColor();
            }
            Console.WriteLine($"스킬 이름 : {Name} ");
            ConsoleUtility.PrintTextHighlights("효과 - ", $"{SkillDesc}");
    }

    public void PrintSkillDescriptionCoolTime(bool withNumber = false, int idx = 0) // 스킬 출력
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        if (withNumber)
        {
            Console.Write($"{idx} ");
        }
        Console.WriteLine($"스킬 이름 : {Name} - 현재 쿨타임입니다! 남은 턴 {CoolTime}");
        Console.ResetColor();
    }
}

        // 지금부터 밑에 있는 클래스들은 스킬 목록입니다. (형식에 맞게 추가하시면 바로 새로운 스킬을 적용시킬 수 있습니다.)

class DoubleAttack : Skill
{
    public DoubleAttack()
    {
        Name = "더블 어택";
        SkillDesc = "적에게 연속으로 두 번의 공격을 날립니다!";

        ConsoleUtility.PrintTextHighlights($"스킬을 획득했습니다! - ", $"{Name}");
    }
    public override void Active(List<Monster> targets, int idx , Player player, int damage, bool critical)
    {
        damage = (int)(damage * 2 * 0.8);

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"  {player.Name}이(가) 더블어택을 시전합니다!");
        Console.ResetColor();

        Console.WriteLine("");
        player.PrintAttackDescription(targets[idx], damage, critical);

        Console.WriteLine("");
        targets[idx].PrintMonsterChangeDescription(targets[idx].Hp, damage);

        CoolTime += 2;
    }
}

class Slash : Skill
{ 
    public Slash()
    {
        Name = "휩쓸기";
        SkillDesc = "전투 중인 모든 적에게 공격을 가합니다.";

        ConsoleUtility.PrintTextHighlights($"스킬을 획득했습니다! - ", $"{Name}");
    }
    public override void Active(List<Monster> targets, int idx, Player player, int damage, bool critical)
    {
        damage = (int)(damage * 0.8);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  {player.Name}이(가) 휩쓸기를 시전합니다!");
        Console.ResetColor();
        foreach (Monster i in targets)
        {
            if (i.IsDead == true) { continue; }

            Console.WriteLine("");
            player.PrintAttackDescription(i, damage, critical);

            Console.WriteLine("");
            i.PrintMonsterChangeDescription(i.Hp, damage);
        }

        CoolTime += 3;
    }
}

class RevengeAttack : Skill
{
    public RevengeAttack()
    {
        Name = "복수";
        SkillDesc = "현재 깎인 체력 비례의 데미지를 적에게 줍니다.";

        ConsoleUtility.PrintTextHighlights($"스킬을 획득했습니다! - ", $"{Name}");
    }
    public override void Active(List<Monster> targets, int idx, Player player, int damage, bool critical)
    {
        damage = (int)((damage * 0.5) + ((100 - player.Hp) * 0.5));

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"  {player.Name}이(가) 복수를 시전합니다!");
        Console.ResetColor();

        Console.WriteLine("");
        player.PrintAttackDescription(targets[idx], damage, critical);

        Console.WriteLine("");
        targets[idx].PrintMonsterChangeDescription(targets[idx].Hp, damage);

        CoolTime += 3;
    }
}

class MoneyAttack : Skill
{
    public MoneyAttack()
    {
        Name = "동전 던지기";
        SkillDesc = "사용한 돈만큼 적들에게 공격을 가합니다! (적 선택 이후 취소 불가)\n       100원당 1데미지를 가합니다. (내림 판정)\n       1000원 이상의 금액을 던질 시 전체 공격으로 변합니다."; 

        ConsoleUtility.PrintTextHighlights($"스킬을 획득했습니다! - ", $"{Name}");
    }
    public override void Active(List<Monster> targets, int idx, Player player, int damage, bool critical)
    {
        bool checkInt = false ;
        int value = 0;
        while (!checkInt) // checkInt가 참이 될 때까지
        {
            Console.Clear();

            ConsoleUtility.ShowTitle("■ Battle!! ■");
            Console.WriteLine();
            ConsoleUtility.PrintTextHighlights("사용하고자 하는 금액을 입력하세요. 최소 금액 ", "100", "원");
            Console.WriteLine();
            ConsoleUtility.PrintTextHighlights("보유 금액 : ", $"{player.Gold}");

            checkInt = int.TryParse(Console.ReadLine(), out value);
            if (value > player.Gold) { ConsoleUtility.PrintTextHighlights("", "그 정도의 금액을 소지하고 있지 않습니다!"); Thread.Sleep(500); checkInt = false; }
            else if (!checkInt) { ConsoleUtility.PrintTextHighlights("", "제대로 입력해주세요!"); Thread.Sleep(500); }
            else if (value < 100) { ConsoleUtility.PrintTextHighlights("최소 금액은 ", "100", "원입니다!"); Thread.Sleep(500); checkInt = false; }
        }

        value = (int)(Math.Truncate((double)value / 100) * 100); // 내림 메서드
        player.Gold -= value;

        damage = (int)(value * 0.01);

        Console.ForegroundColor = ConsoleColor.Yellow;
        if (damage < 10) { Console.WriteLine($"  {player.Name}이(가) 적에게 동전을 던집니다!"); } // 텍스트도 바뀌게 해두었다. 1damage = 100G
        else if (damage >= 100) { Console.WriteLine($"  {player.Name}이(가) 적들에게 동전 소나기를 뿌립니다!"); }
        else if (damage >= 10) { Console.WriteLine($"  {player.Name}이(가) 적들에게 동전 다발을 뿌립니다!"); }

        Console.ResetColor(); // 사용한 금액에 따라 스킬 범위가 달라진다.
        if (damage < 10) 
        {
            Console.WriteLine("");
            player.PrintAttackDescription(targets[idx], damage, false); // 치명타 판정이 없다.

            Console.WriteLine("");
            targets[idx].PrintMonsterChangeDescription(targets[idx].Hp, damage);
        }
        else
        {
            foreach (Monster i in targets)
            {
                if (i.IsDead == true) { continue; }

                Console.WriteLine("");
                player.PrintAttackDescription(i, damage, false); // 여기도 주의!

                Console.WriteLine("");
                i.PrintMonsterChangeDescription(i.Hp, damage);
            }
        }
    }
}