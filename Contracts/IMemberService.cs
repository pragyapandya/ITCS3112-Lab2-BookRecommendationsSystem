using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Contracts;

/// <summary>
/// This interface provides methods for member service activities.
/// It provides methods for logging into/logging out services as well
/// as updating member information with login information. 
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// Creates a new member account based on member name.
    /// </summary>
    /// <param name="name"> Member name</param>
    /// <remarks>
    /// Generates member account id with the help of GenerateNewAccountId() <see cref="IMemberRepo"/>
    /// </remarks>
    /// <returns> Member account information. </returns>
    Member CreateNewMember(string name);
    
    /// <summary>
    /// Logs the user into their account.
    /// </summary>
    /// <param name="account"> Account ID Number. </param>
    /// <returns>Boolean true/false according to login match. </returns>
    bool Login(int account);
    
    /// <summary>
    /// Logs member out of their account.
    /// </summary>
    void Logout();
    
    /// <summary>
    /// Retrieves member information/account as long as they are logged in.
    /// </summary>
    /// <returns> Member account information </returns>
    Member GetLoggedInMember();
    
    /// <summary>
    /// Confirms that a member is logged into their account.
    /// </summary>
    /// <returns>Boolean true/false if user is/is not logged in </returns>
    bool IsLoggedIn();
    
    /// <summary>
    /// Used to change member name if they are logged in.
    /// </summary>
    /// <param name="newName"> Member's new name</param>
    /// <remarks>
    /// Feature is only active for users who are logged into their account.
    /// </remarks>
    /// <returns> Confirmation (boolean true/false) to indicate successful update </returns>
    bool UpdateLoggedInMember(string newName); //so members can change name once logged in
}