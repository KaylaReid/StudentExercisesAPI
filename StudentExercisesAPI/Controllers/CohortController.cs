using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
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
        // GET api/cohorts?q=Taco
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
                // this saves the sql statement to the var sql
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

        // GET api/cohort/5
        [HttpGet("{id}", Name = "GetCohort")]
            public async Task<IActionResult> Get([FromRoute]int id)
            {
            // this saves the sql statement to the var sql
            string sql = $@"
                SELECT 
                    c.Id,
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
        // POST api/cohort
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cohort cohort)
        {
            // this saves the sql statement to the var sql
            string sql = $@"INSERT INTO Cohort(Name)
            VALUES('{cohort.Name}')
            SELECT SCOPE_IDENTITY();";

            using (IDbConnection conn = Connection)
            {
                var newId = (await conn.QueryAsync<int>(sql)).Single();
                cohort.Id = newId;
                return CreatedAtRoute("GetStudent", new { id = newId }, cohort);
            }
        }

        //PUT api/cohort/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Cohort cohort)
        {
            // this saves the sql statement to the var sql
            string sql = $@"
            UPDATE Cohort 
            SET Name = '{cohort.Name}'
            WHERE Id = {id}";

            //try to afect the one and row i'm giving you if you can do it else throw exception 
            try
            {
                using (IDbConnection conn = Connection)
                {
                    int rowsAffected = await conn.ExecuteAsync(sql);
                    if (rowsAffected > 0)
                    {
                        return new StatusCodeResult
                        (StatusCodes.Status204NoContent);
                    }
                    throw new Exception("No rows affected");
                }
            }
            //catch if unable to 
            catch (Exception)
            {
                if (!CohortExists(id))
                {
                    return NotFound();

                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE api/student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string sql = $@"DELETE FROM Cohort WHERE Id = {id}";

            using (IDbConnection conn = Connection)
            {
                int rowsAffected = await conn.ExecuteAsync(sql);
                if (rowsAffected > 0)
                {
                    return new StatusCodeResult
                    (StatusCodes.Status204NoContent);
                }
                throw new Exception("No rows affected");
            }
        }

        private bool CohortExists(int id)
        {
            string sql = $"SELECT Id FROM Cohort WHERE Id = {id}";
            using (IDbConnection conn = Connection)
            {
                return conn.Query<Cohort>(sql).Count() > 0;   
            }
        }
    }
}