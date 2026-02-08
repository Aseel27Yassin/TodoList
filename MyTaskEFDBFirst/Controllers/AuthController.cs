using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using MyTaskEFDBFirst.DTOs.SignIns.Request;
using MyTaskEFDBFirst.DTOs.SignIns.Response;
using MyTaskEFDBFirst.DTOs.SignUps.Request;
using MyTaskEFDBFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace MyTaskEFDBFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //Project of EF with DB First

        private readonly TestdbContext context; //for the dependency injection  (setup holder)

        public AuthController(TestdbContext testdbContext) //constructor , c# method to initialize object in memory
        {
            context= testdbContext; //complete dependency injection
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Registration([FromBody] SignUpInputDTO input)
        {
            try
            {
                //validate input 
                if (!string.IsNullOrEmpty(input.Password) && !string.IsNullOrEmpty(input.Email) &&
                    !string.IsNullOrEmpty(input.Name))
                {
                    if (context.Users.Any(x => x.Email == input.Email))
                        throw new Exception("Email already exist");
                    User user = new User(); //empty object or default object calling insturctor 
                    user.Email = input.Email;
                    user.Password = input.Password;
                    user.Name = input.Name;
                    context.Add(user); //making it record 
                    context.SaveChanges(); //commit or approve and saved on Data
                    //Validation using Query 
                    //1-search
                    var users1 = context.Users.Where(x => x.Email == user.Email).ToList();
                    //2-Get One
                    var users2 = context.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                    var users3 = context.Users.Where(x => x.Email == user.Email).SingleOrDefault();
                    var isUserInserted = context.Users.Where(x => x.Email == user.Email).ToList().Count() == 1;
                    //Check If Exist (recomenned to use)
                    if (context.Users.Any(x => x.Email == user.Email))
                        return StatusCode(201, "Account Created");
                    else
                        return StatusCode(400, "Failed to Create Account");
                    //Validationg using the same object
                    if (user.Id > 0)
                        return StatusCode(201, "Account Created");
                    else
                        return StatusCode(400, "Failed to Create Account");
                }
                return StatusCode(400, "Failed to Create Account");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Error was Occured {ex.Message}");
            }

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LogIn(SignInInputDTO input)
        {
            var response = new SignInOutputDTO();
            try
            {
                if (string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
                    throw new Exception("Email and Password Are Required!");

                //C# LINQ QUIRIES --> Filteration then Selection (After creating the object)
                var query = context.Users.Where(x => x.Email.Equals(input.Email) && x.Password.Equals(input.Password)).SingleOrDefault();
                //Mapping - Extract information
                //either null or error or result
                if (query == null)
                    throw new Exception("Invalid Email / Password");
                    response.Id = query.Id;
                    response.Name = query.Name;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An Error was Occured {ex.Message}");
            }


        }


       
    }
}
