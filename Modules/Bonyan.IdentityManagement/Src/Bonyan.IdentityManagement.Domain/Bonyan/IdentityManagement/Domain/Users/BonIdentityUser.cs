using Bonyan.IdentityManagement.Domain.Roles;
using Bonyan.UserManagement.Domain.Users;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.IdentityManagement.Domain.Users;

public class BonIdentityUser : BonUser,IBonIdentityUser
{
        // Parameterless constructor for EF Core
        protected BonIdentityUser()
        {
        }

        /// <summary>
        /// Initializes a new user with a username only.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="userName">The unique username of the user.</param>
        public BonIdentityUser(BonUserId id, string userName) : base(id, userName)
        {
        }


        /// <summary>
        /// Initializes a new user with a username and password.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="userName">The unique username of the user.</param>
        /// <param name="password">The initial password of the user.</param>
        public BonIdentityUser(BonUserId id, string userName, string password) : base(id, userName,password)
        {
            
        }

        /// <summary>
        /// Initializes a new user with complete details including email and phone number.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="userName">The unique username of the user.</param>
        /// <param name="password">The initial password of the user.</param>
        /// <param name="email">The initial email address of the user.</param>
        /// <param name="phoneNumber">The initial phone number of the user.</param>
        public BonIdentityUser(BonUserId id, string userName, string password, BonUserEmail? email, BonUserPhoneNumber? phoneNumber)
            : base(id, userName, password,email,phoneNumber)
        {
        }


    
}