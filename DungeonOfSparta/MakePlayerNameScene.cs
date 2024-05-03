public partial class GameManager
{
    private void MakePlayerNameScene()
    {
        string inputName;

        do
        {
            Console.Clear();

            Console.Write("원하시는 이름을 설정해주세요: ");

            inputName = Console.ReadLine().Trim();  // 입력 앞뒤 공백 제거
        }while(string.IsNullOrEmpty(inputName));    // 이름 입력 받을 때까지 반복

        player.Name = inputName;

        if(LoadDataFromJson() == true)
        {
            // 세이브 데이터 존재
            MainScene($"{player.Name} 데이터를 로딩합니다.");
        }
        else
        {
            // 세이브 데이터 존재x
            // 데이터 새로 생성

            SelectPlayerJobScene();
        }
    }
}