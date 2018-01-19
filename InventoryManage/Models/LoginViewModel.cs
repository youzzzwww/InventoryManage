using System.ComponentModel.DataAnnotations;

namespace InventoryManage.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "登录名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }
}
