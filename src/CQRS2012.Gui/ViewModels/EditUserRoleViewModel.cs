namespace CQRS2012.Gui.ViewModels
{
    public class EditUserRoleViewModel
    {
        public EditUserRoleViewModel(string userName)
        {
            this.UserName = userName;
        }

        public string UserName { get; private set; }
        public string Role { get; set; }
    }
}