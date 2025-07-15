using System.Security.Claims;
using Bonyan.Novino.Core.Menus;

namespace Bonyan.Novino.Web.Menus
{
    /// <summary>
    /// Menu provider for the main web application navigation
    /// </summary>
    public class WebMainMenuProvider : MenuProviderBase
    {
        public override string ProviderId => "web-main-menu-provider";

        public override int Priority => 1000; // High priority for main navigation

        public override IEnumerable<string> SupportedLocations => new[]
        {
            "sidebar-main",
            "sidebar-system",
            "topbar-user"
        };

        public override async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(string location, ClaimsPrincipal? user = null)
        {
            await Task.Delay(1); // Simulate async operation

            return location.ToLowerInvariant() switch
            {
                "sidebar-main" => GetSidebarMainItems(user),
                "sidebar-system" => GetSidebarSystemItems(user),
                "topbar-user" => GetTopbarUserItems(user),
                _ => Enumerable.Empty<MenuItem>()
            };
        }

        private IEnumerable<MenuItem> GetSidebarMainItems(ClaimsPrincipal? user)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("داشبورد", "/", "ri-dashboard-2-line", 1)
                {
                    CssClass = "nav-link menu-link",
                    Children = new List<MenuItem>
                    {
                        new MenuItem("نمای کلی", "/Dashboard", "ri-home-line", 1)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("آمار و تحلیل", "/Analytics", "ri-bar-chart-line", 2)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("گزارشات", "/Reports", "ri-file-chart-line", 3)
                        {
                            CssClass = "nav-link"
                        }
                    }
                }
            };

            return items;
        }

        private IEnumerable<MenuItem> GetSidebarSystemItems(ClaimsPrincipal? user)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("مدیریت کاربران", "/UserManagement", "ri-user-settings-line", 1)
                {
                    CssClass = "nav-link menu-link",
                    Children = new List<MenuItem>
                    {
                        new MenuItem("لیست کاربران", "/UserManagement/Users", "ri-user-list-line", 1)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("افزودن کاربر", "/UserManagement/Users/Create", "ri-user-add-line", 2)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("ویرایش کاربران", "/UserManagement/Users/Edit", "ri-user-edit-line", 3)
                        {
                            CssClass = "nav-link"
                        }
                    }
                },
                new MenuItem("مدیریت نقش‌ها", "/RoleManagement", "ri-shield-user-line", 2)
                {
                    CssClass = "nav-link menu-link",
                    Children = new List<MenuItem>
                    {
                        new MenuItem("لیست نقش‌ها", "/RoleManagement/Roles", "ri-list-check", 1)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("افزودن نقش", "/RoleManagement/Roles/Create", "ri-add-circle-line", 2)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("مدیریت مجوزها", "/RoleManagement/Permissions", "ri-lock-line", 3)
                        {
                            CssClass = "nav-link"
                        }
                    }
                },
                new MenuItem("تنظیمات سیستم", "/System", "ri-settings-3-line", 3)
                {
                    CssClass = "nav-link menu-link",
                    Children = new List<MenuItem>
                    {
                        new MenuItem("تنظیمات عمومی", "/System/General", "ri-settings-line", 1)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("امنیت", "/System/Security", "ri-shield-line", 2)
                        {
                            CssClass = "nav-link"
                        },
                        new MenuItem("پشتیبان‌گیری", "/System/Backup", "ri-database-2-line", 3)
                        {
                            CssClass = "nav-link"
                        }
                    }
                }
            };

            return items;
        }

        private IEnumerable<MenuItem> GetTopbarUserItems(ClaimsPrincipal? user)
        {
            var items = new List<MenuItem>();

            if (user?.Identity?.IsAuthenticated == true)
            {
                items.AddRange(new[]
                {
                    new MenuItem("پروفایل", "/Account/Profile", "mdi mdi-account-circle", 1)
                    {
                        CssClass = "dropdown-item"
                    },
                    new MenuItem("تنظیمات", "/Account/Settings", "mdi mdi-cog-outline", 2)
                    {
                        CssClass = "dropdown-item"
                    },
                    new MenuItem("خروج", "/Account/Logout", "mdi mdi-logout", 3)
                    {
                        CssClass = "dropdown-item text-danger"
                    }
                });
            }
            else
            {
                items.AddRange(new[]
                {
                    new MenuItem("ورود", "/Account/Login", "mdi mdi-login", 1)
                    {
                        CssClass = "dropdown-item"
                    }
                });
            }

            return items;
        }
    }
} 