using Lab2BookRecommendationSystem.Contracts;
using Lab2BookRecommendationSystem.Domain;

namespace Lab2BookRecommendationSystem.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepo _repo;
    private Member _loggedInMember;
    
    public MemberService(IMemberRepo repo)
    {
        _repo = repo;
        _loggedInMember = null;
    }

    public Member CreateNewMember(string name)
    {
        int newAccountID = _repo.GenerateNewAccountId();
        Member newMember = new Member(newAccountID, name);
        _repo.AddMember(newMember);
        return newMember;
    }

    public bool Login(int account)
    {
        Member member = _repo.GetMemberByAccount(account);
        if (member != null)
        {
            _loggedInMember = member;
            return true;
        }
        return false;
    }

    public void Logout()
    {
        _loggedInMember = null;
    }

    public Member GetLoggedInMember()
    {
        return _loggedInMember;
    }

    public bool IsLoggedIn()
    {
        return _loggedInMember != null;
    }

    public bool UpdateLoggedInMember(string newName)
    {
        //making sure to validate the update
        if (!IsLoggedIn() || string.IsNullOrWhiteSpace(newName))
        {
            return false;
        }
        
        //extra feature to update member info (name)
        _loggedInMember.Name = newName;
        _repo.UpdateMember((_loggedInMember));

        return true;
    }
}