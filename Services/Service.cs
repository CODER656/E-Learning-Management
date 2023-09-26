using Microsoft.EntityFrameworkCore;
using Learning_App.Models;
using Learning_App.Models.Request;
using Learning_App.Models.Response;
using System.Text;

namespace Learning_App.Services
{
    public class Service
    {
        private DatabaseContext _context;
        public Service(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> SignUp(SignUpRequest requestModel)
        {
            try
            {
                User user = new User()
                {
                    Email = requestModel.Email,
                    FirstName = requestModel.FirstName,
                    LastName = requestModel.LastName,
                    Password = Encoding.UTF8.GetBytes(BCrypt.Net.BCrypt.HashPassword(requestModel.Password)),
                    Role = requestModel.Role,
                };

                await _context.Users.AddAsync(user);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<User?> Login(LoginRequest requestModel)
        {
            try
            {
                var user = await _context.Users
                .Where(x => x.Email == requestModel.Email )
                .FirstOrDefaultAsync();

                if(user == null)
                {
                    return null;
                }
             
                if(!BCrypt.Net.BCrypt.Verify(requestModel.Password, Encoding.UTF8.GetString(user.Password)))
                {
                    return null;
                }
                return user;
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        public async Task<List<ListUserResponse>> GetUserList()
        {
            try
            { 
                var users = await _context.Users // users is List<User>
                .Select(x => new ListUserResponse() // x is User
                {
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Role = x.Role,
                })
                .ToListAsync(); 

                return users; 
            }
            catch (Exception e)
            {
                return null;
            }
        } 
        
        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.Users
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

                if(user == null)
                {
                    return false;
                }

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        } 

        public async Task<bool> CreateCourse(Course course, List<Lesson> Lessons, List<CourseResource> Resources, List<CourseAssignments> Assignments)
        {

            try
            {
                var newCourse = new Course()
                {
                    Description = course.Description,
                    ImageUrl = course.ImageUrl,
                    UserId = course.UserId,
                    Title = course.Title,
                };

                await _context.Courses.AddAsync(newCourse);

                await _context.SaveChangesAsync();

                List<int> lessonIds = new List<int>();


                foreach (var lesson in Lessons)
                {
                    var newLesson = new Lesson()
                    {
                        Course = newCourse,
                        CourseId = newCourse.CourseId,
                        Description = lesson.Description,
                        Title = lesson.Title,
                        Resources = new List<CourseResource>(),
                    };

                    await _context.Lessons.AddAsync(newLesson); // lesson is Lesson
                    await _context.SaveChangesAsync(); // lesson.LessonId is now available
                    lessonIds.Add(newLesson.LessonId); // lessonIds is List<int>
                }

                for (int i = 0; i < Resources.Count; i++)
                {
                    var resource = Resources[i];
                    var newResource = new CourseResource()
                    {
                        CourseId = newCourse.CourseId,
                        LessonId = lessonIds[i],
                        ContentInfo = resource.ContentInfo,
                        ResourceType = resource.ResourceType,
                    };

                    await _context.CourseResources.AddAsync(newResource);
                    await _context.SaveChangesAsync();

                }

                foreach (var assignment in Assignments)
                {
                    var newAssignment = new CourseAssignments()
                    {
                        CourseId = newCourse.CourseId,
                        Description = assignment.Description,
                        OverallScore = 0,
                        Title = assignment.Title,
                        FileURL = "",
                    };

                    await _context.CourseAssignments.AddAsync(newAssignment); // lesson is Lesson
                    await _context.SaveChangesAsync(); // lesson.LessonId is now available
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

            
        }
    }


}
