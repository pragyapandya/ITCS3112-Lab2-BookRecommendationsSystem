using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Contracts;

public interface IMemberService
{
    Member CreateNewMember(string name);
    bool Login(int account);
    void Logout();
    Member GetLoggedInMember();
    bool IsLoggedIn();
    bool UpdateLoggedInMember(string newName); //so members can change name once logged in
}