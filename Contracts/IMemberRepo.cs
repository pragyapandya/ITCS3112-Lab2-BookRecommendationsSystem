using Lab2BookRecommendationSystem.Domain;

namespace Lab2BookRecommendationSystem.Contracts
{
    public interface IMemberRepo
    {
        void AddMember(Member member);
        Member GetMemberByAccount(int account);
        int GenerateNewAccountId();
        void UpdateMember(Member member);

    }
}