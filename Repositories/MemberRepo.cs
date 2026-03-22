using System.Collections.Generic;
using System.Linq;
using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;

namespace Lab2BookRecommendationSystem.Repositories;

public class MemberRepo : IMemberRepo
{
    private readonly List<Member> _members = new List<Member>();

    public void AddMember(Member member)
    {
        _members.Add(member);
    }

    public Member GetMemberByAccount(int account)
    {
        return _members.SingleOrDefault(m => m.AccountId == account);
    }

    public int GenerateNewAccountId()
    {
        if (_members.Count == 0) return 1;
        return _members.Max(m => m.AccountId) + 1;
    }

    public void UpdateMember(Member updatedMember)
    {
        var existingMember = GetMemberByAccount(updatedMember.AccountId);
     
        if (existingMember != null)
        {
            existingMember.Name = updatedMember.Name;
        }
    }

    public List<Member> GetAllMembers()
    {
        List<Member> memList = new List<Member>();
        foreach (var mem in _members)
        {
            memList.Add(mem);
        }

        return memList;
    }
}