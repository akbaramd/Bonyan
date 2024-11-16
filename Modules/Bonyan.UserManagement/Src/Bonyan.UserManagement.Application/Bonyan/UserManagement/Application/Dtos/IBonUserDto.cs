﻿using Bonyan.Layer.Application.Dto;
using Bonyan.UserManagement.Domain.Enumerations;
using Bonyan.UserManagement.Domain.ValueObjects;

namespace Bonyan.UserManagement.Application.Dtos;

public interface IBonUserDto : IBonAggregateRootDto<BonUserId>
{
    /// <summary>
    /// Gets or sets the user's unique username.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// Can be null if the user has not provided an email.
    /// </summary>
    public Email? Email { get;  set; }

    /// <summary>
    /// Gets or sets the user's phone number.
    /// Can be null if the user has not provided a phone number.
    /// </summary>
    public PhoneNumber? PhoneNumber { get;  set; }

    /// <summary>
    /// Gets the current status of the user.
    /// </summary>
    public UserStatus Status { get;  set; }
}