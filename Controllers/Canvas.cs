using EDSU_SYSTEM.Data;
using EDSU_SYSTEM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EDSU_SYSTEM.Controllers
{
    //[Authorize(Roles = "staff, superAdmin")]
    public class Canvas : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        const int perPage = 100; // Number of courses per page
        const int UserperPage = 100; // Number of users per page
        
        private readonly HttpClient client;
        public Canvas(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            client = new HttpClient
            {
                BaseAddress = new Uri(Environment.GetEnvironmentVariable("CANVAS_BASE_URL"))
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("CANVAS_TOKEN"));
        }
        public async Task<CanvasCourse> GetCourse(string id)
        {
            try
            {
                string courseCode = id;
                string accessToken = Environment.GetEnvironmentVariable("CANVAS_TOKEN");

                // Set up the HttpClient with the access token
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    string url = $"https://edouniversity.instructure.com/api/v1/accounts/1/courses?search_by=courses&search_term={courseCode}";

                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse and return the response JSON
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        //CanvasCourse course = JsonConvert.DeserializeObject<CanvasCourse>(jsonResponse);
                        List<CanvasCourse> courses = JsonConvert.DeserializeObject<List<CanvasCourse>>(jsonResponse);

                        // Assuming you are interested in the first course in the list
                        return courses.FirstOrDefault();
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve course. Status code: {response.StatusCode}");
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorResponse);
                        return null; // Or throw an exception if you want to handle errors differently
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null; // Or throw an exception if you want to handle errors differently
            }
        }
        public async Task<CanvasUser> GetUser(string id)
        {
            try
            {
                string user = id;
                string accessToken = Environment.GetEnvironmentVariable("CANVAS_TOKEN");

                // Set up the HttpClient with the access token
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    string url = $"https://edouniversity.instructure.com/api/v1/accounts/1/users?search_by=users&search_term={user}";

                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse and return the response JSON
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        List<CanvasUser> users = JsonConvert.DeserializeObject<List<CanvasUser>>(jsonResponse);

                        // Assuming you are interested in the first course in the list
                        return users.FirstOrDefault();
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve course. Status code: {response.StatusCode}");
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorResponse);
                        return null; // Or throw an exception if you want to handle errors differently
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null; // Or throw an exception if you want to handle errors differently
            }
        }
        public async Task<IActionResult> EnrollStudent(int id)
        {
            try
            {
                var currentSession = _context.Sessions.FirstOrDefault(x => x.IsActive == true);
                var studentEmail = (from s in _context.Students where s.Id == id select s.SchoolEmailAddress).FirstOrDefault();

                var courses = (from s in _context.CourseRegistrations where s.StudentId == id && s.Courses.Semesters.IsActive == true select s).Include(x => x.Courses).ThenInclude(s => s.Semesters).Include(n => n.Students).Include(x => x.Courses).ThenInclude(x => x.Levels).Include(x => x.Courses).ThenInclude(x => x.Departments).ToList();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("CANVAS_TOKEN"));

                    foreach (var courseCode in courses)
                    {
                        try
                        {
                            var canvasCourses = await GetCourse(courseCode.Courses.Code);
                            var courseId = canvasCourses.id;
                            var user = await GetUser(studentEmail);
                            var userId = user.id;
                            string apiUrl = $"https://edouniversity.instructure.com/api/v1/courses/{courseId}/enrollments";
                            var enrollmentData = new
                            {
                                enrollment = new
                                {
                                    user_id = userId,
                                    type = "StudentEnrollment",
                                    enrollment_state = "active"
                                }
                            };

                            string jsonData = JsonConvert.SerializeObject(enrollmentData);
                            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                            Console.WriteLine(content);

                            if (response.IsSuccessStatusCode)
                            {
                                TempData["success"] = $"{user.name} has been enrolled to LMS";
                            }
                            else
                            {
                                TempData["err"] = $"Failed to enroll user. Status code: {response.StatusCode}";
                                Console.WriteLine($"Failed to enroll user. Status code: {response.StatusCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["err"] = $"An error occurred: {ex.Message}";
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    }

                    return RedirectToAction("approved", "courseregistrations");
                }
            }
            catch (Exception ex)
            {
                TempData["err"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("approved", "courseregistrations");
            }
        }
        public async Task<IActionResult> EnrollCourses()
        {
            try
            {
                var currentSession = _context.Sessions.FirstOrDefault(x => x.IsActive == true);

                var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                var staff = loggedInUser.StaffId;
                var courses = (from s in _context.CourseAllocations where s.LecturerId == staff && s.Courses.Semesters.IsActive == true select s).Include(x => x.Courses).ThenInclude(s => s.Semesters).Include(n => n.Staff).Include(x => x.Courses).ThenInclude(x => x.Levels).Include(x => x.Courses).ThenInclude(x => x.Departments).ToList();
                var staffEmail = (from s in _context.Staffs where s.Id == staff select s.SchoolEmail).FirstOrDefault();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("CANVAS_TOKEN"));

                    foreach (var courseCode in courses)
                    {
                        try
                        {
                            var canvasCourses = await GetCourse(courseCode.Courses.Code);
                            var courseId = canvasCourses.id;
                            var user = await GetUser(staffEmail);
                            var userId = user.id;

                            string apiUrl = $"https://edouniversity.instructure.com/api/v1/courses/{courseId}/enrollments";
                            var enrollmentData = new
                            {
                                enrollment = new
                                {
                                    user_id = userId,
                                    type = "TeacherEnrollment",
                                    enrollment_state = "active"
                                }
                            };

                            string jsonData = JsonConvert.SerializeObject(enrollmentData);
                            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                            HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                            Console.WriteLine(content);

                            if (response.IsSuccessStatusCode)
                            {
                                TempData["success"] = "Courses enrolled successfully. Kindly log on to Canvas LMS to publish courses. Have fun!🎉";
                            }
                            else
                            {
                                TempData["err"] = $"Failed to enroll user. Status code: {response.StatusCode}";
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["err"] = $"An error occurred: {ex.Message}";
                            
                        }
                    }

                    return RedirectToAction("mycourses", "courses");
                }
            }
            catch (Exception ex)
            {
                TempData["err"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("mycourses", "courses");
            }
        }


    }
    public class CanvasUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string Email { get; set; }
        public string User_id { get; set; }
        public string LoginId { get; set; }
        public string Role { get; set; }
        public bool Active { get; set; }
    }
    public class CanvasCourse
    {
        public string id { get; set; }
        public string sis_course_id { get; set; }
        public string name { get; set; }
        public string Course_id { get; set; }
        public bool IsActive { get; set; }
    }
}
