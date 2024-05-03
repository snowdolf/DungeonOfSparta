using System.Reflection.Emit;
using System.Xml.Linq;

internal class Skill
{
    public List<Skill> SkillList { get; set; }  // 이곳은 스킬을 모아줄 목록입니다. (참조명).SkillList 를 유의!
    public string Name { get; set; }
    public string SkillDesc { get; set; }

    public virtual void Active(List<Monster> target, int idx, Player player, int damage, bool critical) { }  // 범용성을 위해서 List<Monster>를 가져왔고, player의 정보 자체를 가져와서 플레이어의 특정 스텟을 데미지 계산에 이용하실 수 있습니다.

    public Skill() 
    {
        SkillList = new List<Skill>(); // 참조형 변수들은 아무것도 참조하고 있지 않았을 때 접근 시도를 하면 Null참조 에러 뜹니다.
    }

    public enum SkillName   // 추가되는 대로 넣어주시면 됩니다.
    {
        DoubleAttack,
        Slash,
        RevengeAttack
    }

    public void SkillEarn(SkillName idx)  // 스킬 획득입니다. case에 맞게 스킬을 추가하시면 됩니다.
    {
        switch ((int)idx)
        {
            case 0:
                SkillList.Add(new DoubleAttack());
                break;
            case 1:
                SkillList.Add(new Slash());
                break;
            case 2:
                SkillList.Add(new RevengeAttack());
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
    {;
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
                Console.WriteLine("");
                SkillList[i].PrintSkillDescription(true, i + 1);
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
    }
}