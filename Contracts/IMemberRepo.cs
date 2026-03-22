using System.Collections.Generic;
using Lab2BookRecommendationSystem.Domain;
namespace Lab2BookRecommendationSystem.Contracts
{
    /// <summary>
    /// This interface represents a repository to hold member information.
    /// It allows for adding new members to the repo, update account info,
    /// and find users by ID.
    /// </summary>
    public interface IMemberRepo
    {
        /// <summary>
        ///  Creates new member and adds it to database.
        /// </summary>
        /// <param name="member"> Member's name </param>
        void AddMember(Member member);
    
        /// <summary>
        /// Retrieves member information/account based on account number.
        /// </summary>
        /// <param name="account"> Member's account number (ID) </param>
        /// <remarks>
        /// Only uses account id to find member information, as multiple
        /// members are allowed to have the same name.
        /// </remarks>
        /// <returns> Returns member account information. </returns>
        Member GetMemberByAccount(int account);
        
        /// <summary>
        /// Autogenerates unique member account ids.
        /// </summary>
        /// <remarks>
        /// Uses a simple count logic to make sure that new id increases
        /// by 1 from the previous id.
        /// </remarks>
        /// <returns> A unique id number to allocate to each member </returns>
        int GenerateNewAccountId();
        
        /// <summary>
        /// Updates existing member information once they are logged in.
        /// </summary>
        /// <param name="member"></param>
        void UpdateMember(Member member);

        List<Member> GetAllMembers();
    }
}