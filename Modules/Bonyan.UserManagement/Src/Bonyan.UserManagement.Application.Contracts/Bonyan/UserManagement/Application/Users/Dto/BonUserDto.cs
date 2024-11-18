using Bonyan.Layer.Application.Dto;
using Bonyan.UserManagement.Domain.Users.Enumerations;
using Bonyan.UserManagement.Domain.Users.ValueObjects;

namespace Bonyan.UserManagement.Application.Users.Dto;

public class BonUserDto: BonFullAuditableAggregateRootDto<BonUserId>
{
    /// <summary>
    /// Gets or sets the user's unique username.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// Can be null if the user has not provided an email.
    /// </summary>
    public BonUserEmail? Email { get;  set; }

    /// <summary>
    /// Gets or sets the user's phone number.
    /// Can be null if the user has not provided a phone number.
    /// </summary>
    public BonUserPhoneNumber? PhoneNumber { get;  set; }

    /// <summary>
    /// Gets the current status of the user.
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Inactive;
}


public class BonUserCreateDto
{
    public string UserName { get; set; }
    public BonUserEmail? Email { get;  set; }
    public BonUserPhoneNumber? PhoneNumber { get;  set; }
    public UserStatus Status { get; set; } = UserStatus.Inactive;
}


public class BonUserUpdateDto
{
    public string UserName { get; set; }
    public BonUserEmail? Email { get;  set; }
    public BonUserPhoneNumber? PhoneNumber { get;  set; }
    public UserStatus Status { get; set; } = UserStatus.Inactive;
}