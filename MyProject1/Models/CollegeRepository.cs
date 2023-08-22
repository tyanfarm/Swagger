namespace MyProject1.Models
{
    public static class CollegeRepository
    {
        public static List<Student> Students { get; set; } = new List<Student>(){
                new Student
                {
                    Id = 1,
                    StudentName = "Tyan",
                    Email = "phamquangtuyen.nt@gmail.com",
                    Address = "12 Melbourne"
                },
                new Student
                {
                    Id = 2,
                    StudentName = "Scul",
                    Email = "student2@gmail.com",
                    Address = "Los Angeles"
                }
            }; 
    }
}
