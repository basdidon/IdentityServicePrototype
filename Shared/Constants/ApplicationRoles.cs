namespace Shared.Constants
{
    public static class ApplicationRoles
    {
        public const string Admin = "Admin";
        public const string Instructor = "Instructor";
        public const string Student = "Student";

        public static string[] All => [Admin, Instructor, Student];
    }
}
