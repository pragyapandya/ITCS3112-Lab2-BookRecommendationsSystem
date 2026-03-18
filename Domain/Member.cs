namespace Lab2BookRecommendationSystem.Domain;

public class Member
{
    public int Account { get; set; }
    public string Name { get; set; }

    public Member(int account, string name)
    {
        Account = account;
        Name = name;
    }
}