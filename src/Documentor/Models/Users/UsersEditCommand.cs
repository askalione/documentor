namespace Documentor.Models.Users
{
    public class UsersEditCommand
    {
        public IEnumerable<string> Emails { get; set; } = new List<string>();
    }
}
