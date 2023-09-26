namespace Learning_App.Models.Response
{
    public class ListInstructorCourses
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int UserId { get; set; }
        public string ImageUrl { get; set; }
        
    }
}