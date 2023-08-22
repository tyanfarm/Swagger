using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyProject1.Models;
using MyProject1.MyLogging;
using System.Net;

namespace MyProject1.Controllers
{
    // Dấu ngoặc vuông đại diện cho các chữ ở trước 'Controller' (Student)
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController: ControllerBase
    {
        // DI đã được thêm vào ở Program.cs
        // var builder = WebApplication.CreateBuilder(args);
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        // Show trước các loại type response
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<StudentDTO>> GetStudents()
        {
            _logger.LogInformation("GetStudents method started");
            var students = CollegeRepository.Students.Select(s => new StudentDTO()
            {
                Id = s.Id,
                StudentName = s.StudentName,
                Email = s.Email,
                Address = s.Address
            });

            // Ok - 200 - Success
            return Ok(students);
        }

        [HttpGet]
        // Đặt trong {} vì là giá trị động
        [Route("{id:int}", Name = "GetStudentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> GetStudentByID(int id)
        {
            // BadRequest - 400 - Client error
            if (id <= 0)
            {
                _logger.LogWarning("Bad Request");
                return BadRequest();
            }

            var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();

            // NotFound - 404 - Client error
            if (student == null)
            {
                _logger.LogError("Student not found with given id");
                return NotFound($"The student with id {id} is not found");
            }

            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Email = student.Email,
                Address = student.Address
            };

            // Trả về sinh viên đầu tiên trùng khớp hoặc null nếu không tìm thấy
            // Ok - 200 - Success
            return Ok(studentDTO);
        }

        [HttpGet]
        [Route("{name:alpha}", Name = "GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> GetStudentByName(string name)
        {
            // BadRequest - 400 - Client error
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var student = CollegeRepository.Students.Where(n => n.StudentName == name).FirstOrDefault();
            // NotFound - 404 - Client error
            if (student == null)
            {
                return NotFound($"The student with name {name} is not found");
            }

            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Email = student.Email,
                Address = student.Address
            };

            // Trả về sinh viên đầu tiên trùng khớp hoặc null nếu không tìm thấy
            // Ok - 200 - Success
            return Ok(studentDTO);
        }

        [HttpPost]
        [Route("Create")]
        // api/student/create
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> CreateStudent([FromBody] StudentDTO model)
        {
            //// Checker thay vai trò của ApiController
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            if (model == null)
            {
                return BadRequest();
            }

            //if (model.AdmissionDate < DateTime.Now)
            //{
            //    // 1. Directly adding error message to modelstate
            //    // 2. Using custom attribute
            //    ModelState.AddModelError("AdmissionDate Error", "Admission date must be greater than or equal to todays date !");
            //    return BadRequest(ModelState);
            //}

            // Tạo id mới ở cuối list
            int newID = CollegeRepository.Students.LastOrDefault().Id + 1;

            Student student = new Student
            {
                Id = newID,
                StudentName = model.StudentName,
                Email = model.Email,
                Address = model.Address
            };

            CollegeRepository.Students.Add(student);

            model.Id = student.Id;

            // Trả về url của student mới add
            // Status - 201
            // 
            return CreatedAtRoute("GetStudentByID", new { id = model.Id }, model);
        }

        [HttpPut]
        [Route("Update")]
        // api/student/update
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> UpdateStudent([FromBody] StudentDTO model)
        {
            if (model == null || model.Id <= 0)
            {
                return BadRequest();
            }

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == model.Id).FirstOrDefault();

            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.StudentName = model.StudentName;
            existingStudent.Email = model.Email;
            existingStudent.Address = model.Address;

            return NoContent();
        }

        [HttpPatch]
        [Route("{id:int}/UpdatePartial")]
        // api/student/1/updatepartial
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == id).FirstOrDefault();

            if (existingStudent == null)
            {
                return NotFound();
            }

            // Tất cả các xử lí phải đưa qua DTO
            // Sao chép từ student cần tìm vào dto
            var studentDTO = new StudentDTO
            {
                Id = existingStudent.Id,
                StudentName = existingStudent.StudentName,
                Email = existingStudent.Email,
                Address = existingStudent.Address
            };

            // Áp dụng các chỉnh sửa từ người dùng
            patchDocument.ApplyTo(studentDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Chỉnh lại ở trong student chính
            existingStudent.StudentName = studentDTO.StudentName;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Address = studentDTO.Address;

            // 204 - NoContent
            return NoContent();
        }

        [HttpDelete("Delete/{id:min(1):max(100)}", Name = "DeleteStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> DeleteStudents(int id)
        {
            // BadRequest - 400 - Client error
            if (id <= 0)
            {
                return BadRequest();
            }

            var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
            // NotFound - 404 - Client error
            if (student == null)
            {
                return NotFound($"The student with id {id} is not found");
            }

            CollegeRepository.Students.Remove(student);

            // Ok - 200 - Success
            return true;
        }
    }
}
