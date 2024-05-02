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

        SelectPlayerJobScene();
    }
}