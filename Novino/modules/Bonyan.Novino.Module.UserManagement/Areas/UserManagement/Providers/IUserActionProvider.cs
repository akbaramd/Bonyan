using System.Collections.Generic;
using Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Models;

namespace Bonyan.Novino.Module.UserManagement.Areas.UserManagement.Providers
{
    public interface IUserActionProvider
    {
        /// <summary>
        /// اکشن‌های قابل نمایش برای هر کاربر را برمی‌گرداند
        /// </summary>
        IEnumerable<UserActionItem> GetUserActions(UserListViewModel user);
    }

    public class UserActionItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string CssClass { get; set; }
        public bool Show { get; set; } = true;
        public string Tooltip { get; set; }
    }
} 