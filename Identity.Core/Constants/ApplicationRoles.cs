namespace Identity.Core.Constants
{
    public static class ApplicationRoles
    {
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";

        public static string[] All => [ Admin, Teacher, Student ];
    }
}