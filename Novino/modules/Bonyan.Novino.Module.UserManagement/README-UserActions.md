# User Management Action Pages

This document describes the user action pages and zone models that have been created to support extensible user management functionality.

## Overview

The user management module now includes dedicated pages for common user actions:
- **Change Password**: Secure password change with extensible form fields
- **Lock/Unlock**: User account locking with reason tracking
- **Delete Confirmation**: Safe user deletion with username verification

All pages support zone-based extensibility, allowing other modules to add custom fields, validation, and functionality.

## Zone Models

### 1. UserChangePasswordZone
**Location**: `Abstractions/Zones/UserActions/UserChangePasswordZone.cs`

**Purpose**: Zone model for change password functionality with extensible form support.

**Key Features**:
- Contains only `UserId` (minimal data approach)
- Supports custom form fields via `AddCustomField()`
- Supports custom validation scripts via `AddValidationScript()`
- Supports additional data via `AddAdditionalData()`

**Usage Example**:
```csharp
var zone = new UserChangePasswordZone
{
    UserId = user.Id.ToString()
};

// Add custom fields
zone.AddCustomField("<div class='custom-field'>...</div>");

// Add validation
zone.AddValidationScript("console.log('Custom validation');");
```

### 2. UserLockZone
**Location**: `Abstractions/Zones/UserActions/UserLockZone.cs`

**Purpose**: Zone model for lock/unlock functionality.

**Key Features**:
- Contains `UserId` and `ActionType` ("lock" or "unlock")
- Same extensibility features as UserChangePasswordZone
- Supports custom fields and validation for lock/unlock forms

### 3. UserDeleteZone
**Location**: `Abstractions/Zones/UserActions/UserDeleteZone.cs`

**Purpose**: Zone model for delete confirmation functionality.

**Key Features**:
- Contains only `UserId`
- Supports custom fields for delete confirmation forms
- Allows additional validation and warnings

## Action Pages

### 1. Change Password Page
**Location**: `Pages/User/ChangePassword.cshtml` and `ChangePassword.cshtml.cs`

**Route**: `/UserManagement/User/ChangePassword/{id}`

**Features**:
- ✅ Secure password change with confirmation
- ✅ Force change password option
- ✅ Zone-based extensible form fields
- ✅ Permission-based access control
- ✅ Comprehensive validation
- ✅ User-friendly interface with password visibility toggle

**Form Fields**:
- New Password (with confirmation)
- Force Change Password checkbox
- Custom fields from zone components

**Security**:
- Validates current user permissions
- Ensures password confirmation matches
- Logs all password change activities

### 2. Lock/Unlock Page
**Location**: `Pages/User/Lock.cshtml` and `Lock.cshtml.cs`

**Route**: `/UserManagement/User/Lock/{id}/{action}` where action is "lock" or "unlock"

**Features**:
- ✅ Dynamic interface based on action type
- ✅ Reason field for lock operations
- ✅ Zone-based extensible form fields
- ✅ Permission-based access control
- ✅ Status validation (prevents invalid operations)

**Form Fields**:
- Reason field (for lock operations)
- Custom fields from zone components

**Security**:
- Validates current user permissions for specific action
- Ensures action is valid for current user status
- Logs all lock/unlock activities with reasons

### 3. Delete Confirmation Page
**Location**: `Pages/User/DeleteConfirm.cshtml` and `DeleteConfirm.cshtml.cs`

**Route**: `/UserManagement/User/DeleteConfirm/{id}`

**Features**:
- ✅ Username verification requirement
- ✅ Zone-based extensible form fields
- ✅ Permission-based access control
- ✅ Comprehensive user information display
- ✅ Multiple confirmation steps

**Form Fields**:
- Username confirmation field
- Custom fields from zone components

**Security**:
- Requires exact username verification
- Validates current user permissions
- Checks if user can be deleted
- Logs all deletion activities

## Zone View Components

### UserChangePasswordFormZoneView
**Location**: `ZoneViews/UserChangePasswordFormZoneView.cs`

**Purpose**: Demonstrates how other modules can extend the change password form.

**Features**:
- Adds notification checkbox
- Adds reason selection dropdown
- Adds admin note field
- Includes custom validation JavaScript
- Shows permission-based rendering

**Example Output**:
```html
<div class="mb-3">
    <div class="form-check">
        <input type="checkbox" id="sendNotification" name="sendNotification" value="true">
        <label for="sendNotification">
            <i class="ri-notification-line me-1"></i>ارسال اعلان به کاربر
        </label>
    </div>
</div>
```

## Integration with Table Actions

The `UserTableActionDefaultZoneView` has been updated to use the new pages:

### Before (JavaScript-based):
```html
<button onclick="resetUserPassword('{userId}', '{userName}')">
    <i class="ri-lock-password-line"></i>
</button>
```

### After (Page-based):
```html
<a href="/UserManagement/User/ChangePassword/{userId}">
    <i class="ri-lock-password-line"></i>
</a>
```

## Benefits of This Approach

### 1. **Security**
- ✅ Server-side validation and processing
- ✅ Permission-based access control
- ✅ Comprehensive logging
- ✅ CSRF protection through forms

### 2. **User Experience**
- ✅ Consistent UI/UX across all actions
- ✅ Clear confirmation steps
- ✅ Comprehensive error handling
- ✅ Responsive design

### 3. **Extensibility**
- ✅ Zone-based form extension
- ✅ Custom validation support
- ✅ Additional field injection
- ✅ Module-agnostic design

### 4. **Maintainability**
- ✅ Clean separation of concerns
- ✅ Reusable zone models
- ✅ Consistent patterns
- ✅ Easy to extend and modify

## Usage Examples

### Adding Custom Fields to Change Password Form

```csharp
// In another module
public class CustomChangePasswordZoneView : ZoneViewComponent<UserChangePasswordZone>
{
    public override async Task<ZoneComponentResult> HandleAsync(UserChangePasswordZone model, ZoneRenderingContext context, ZoneComponentParameters parameters, CancellationToken cancellationToken = default)
    {
        var customField = @"
            <div class=""mb-3"">
                <label class=""form-label"">Custom Field</label>
                <input type=""text"" class=""form-control"" name=""customField"">
            </div>";
            
        return ZoneComponentResult.Html(customField, true);
    }
}
```

### Adding Custom Validation

```csharp
// In the zone model
zone.AddValidationScript(@"
    document.getElementById('customField').addEventListener('change', function() {
        // Custom validation logic
    });
");
```

## Security Considerations

1. **Permission Checks**: All pages validate user permissions before allowing actions
2. **Input Validation**: Comprehensive server-side validation for all inputs
3. **CSRF Protection**: Forms include anti-forgery tokens
4. **Logging**: All actions are logged with user context
5. **Status Validation**: Prevents invalid state transitions (e.g., locking already locked user)

## Future Enhancements

1. **Bulk Operations**: Support for bulk user actions
2. **Audit Trail**: Enhanced audit logging and history
3. **Email Notifications**: Automatic email notifications for actions
4. **Workflow Integration**: Integration with approval workflows
5. **API Endpoints**: REST API endpoints for programmatic access

## Conclusion

The new user action pages provide a secure, extensible, and user-friendly approach to user management operations. The zone-based architecture allows for easy extension by other modules while maintaining clean separation of concerns and consistent security practices. 