namespace Helios.Authentication.Consumers.ConsumerModels
{
    public class AddStudyUserModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ResearchName { get; set; }
        public int ResearchLanguage { get; set; }
        public bool FirstAddition { get; set; }
    }
}
