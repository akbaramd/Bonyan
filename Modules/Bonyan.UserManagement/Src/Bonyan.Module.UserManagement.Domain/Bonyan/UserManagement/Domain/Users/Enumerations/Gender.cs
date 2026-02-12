using Bonyan.Layer.Domain.Enumerations;

namespace Bonyan.UserManagement.Domain.Users.Enumerations;

/// <summary>
/// جنسیت کاربر (اختیاری؛ صرفاً برای پروفایل).
/// </summary>
/// <summary>
/// User gender for profile only. Not used for authentication or identity.
/// </summary>
public class Gender : BonEnumeration
{
    public static readonly Gender Male = new(1, nameof(Male));
    public static readonly Gender Female = new(2, nameof(Female));
    public static readonly Gender Other = new(3, nameof(Other));
    public static readonly Gender PreferNotToSay = new(4, nameof(PreferNotToSay));

    private Gender(int id, string name) : base(id, name) { }
}
