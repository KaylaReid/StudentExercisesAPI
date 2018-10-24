using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesAPI.Data;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortController : Controller
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET api/students?q=Taco
        [HttpGet]
        public async Task<IActionResult> Get(string q)
        {
            string sql = @"
            SELECT 
                c.Id,
                c.Name
            FROM Cohort c
            WHERE 1=1
            ";

            if (q != null)
            {
                string isQ = $@"
                   AND c.Name LIKE '%{q}%'";
                sql = $"{sql} {isQ}";
            }

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Cohort> cohorts = await conn.QueryAsync<Cohort>(
                    sql);
                return Ok(cohorts);
            }
        }

        // GET api/students/5
        [HttpGet("{id}", Name = "GetCohort")]
            public async Task<IActionResult> Get([FromRoute]int id)
            {
            string sql = $@"
                SELECT 
                    c.id,
                    c.Name
                FROM Cohort c
                WHERE c.Id = {id}
            ";

            using (IDbConnection conn = Connection)
            {
                IEnumerable<Cohort> cohorts = await conn.QueryAsync<Cohort>(sql);
                return Ok(cohorts);
            }
        }
        // POST api/students
        [HttpPost]
    }
}