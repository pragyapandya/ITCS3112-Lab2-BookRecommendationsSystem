namespace Lab2BookRecommendationSystem.Domain;

public class Member
{
    public int AccountId { get; set; }
    public string Name { get; set; }

    public Member(int accountId, string name)
    {
        AccountId = accountId;
        Name = name;
    }

    public void UpdateMember(int accountId, string name)
    {
        AccountId = accountId;
        Name = name;
    }
}
