namespace Bonyan.Novino.Web.Models
{
    public class LogoutViewModel
    {
        public string ReturnUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; } = false;
    }
} 