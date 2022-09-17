using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCurdApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public async Task<ActionResult<List<Students>>> GetAllStudents()
        {

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Students> students = await SelectAllStudents(connection);
            return Ok(students);
        }


        [HttpGet("{StuId}")]
        public async Task<ActionResult<List<Students>>> GetStudents(int StuId)
        {

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var student = await connection.QueryFirstAsync<Students>("select * from StudentsTb where studentid=@StudentID",
                new {StudentID= StuId });
            return Ok(student);
        }
        [HttpPost]
        public async Task<ActionResult<List<Students>>> CreateStudents(Students student)
        {

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into StudentsTb (firstname,lastname,address,mobile)values(@FirstName,@LastName,@Address,@Mobile)", student);
            return Ok(await SelectAllStudents(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Students>>> UpdateStudents(Students student)
        {

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update  StudentsTb set firstname=@FirstName,lastname=@LastName,address=@Address,mobile=@Mobile where studentid=@StudentID", student);
            return Ok(await SelectAllStudents(connection));
        }

        [HttpDelete("{StuId}")]
        public async Task<ActionResult<List<Students>>> DeleteStudents(int StuId)
        {

            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from  StudentsTb where studentid=@StudentID", new { StudentID = StuId });
            return Ok(await SelectAllStudents(connection));
        }

        private static async Task<IEnumerable<Students>> SelectAllStudents(SqlConnection connection)
        {
            return await connection.QueryAsync<Students>("select * from StudentsTb ");
        }

    }
}

