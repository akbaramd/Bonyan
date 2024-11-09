using Bonyan.Layer.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Layer.Domain.Aggregates;
using Bonyan.Layer.Domain.Events;
using Bonyan.UserManagement.Domain;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.Events;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.IdentityManagement.Domain
{
    public class BonIdentityUser : BonUser
    {
        private readonly List<BonIdentityRole> _roles = new();

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
        public BonIdentityUser(BonUserId id, string userName, string password, Email? email, PhoneNumber? phoneNumber)
            : base(id, userName, password,email,phoneNumber)
        {
        }

        public IReadOnlyCollection<BonIdentityRole> Roles => _roles;

        public void TryAssignRole(BonIdentityRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            if (_roles.All(r => r.Name != role.Name))
            {
                _roles.Add(role);
            }
        }

        public void TryRemoveRole(BonIdentityRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            var existingRole = _roles.FirstOrDefault(r => r.Name == role.Name);
            if (existingRole != null)
            {
                _roles.Remove(existingRole);
            }
        }
    }
}