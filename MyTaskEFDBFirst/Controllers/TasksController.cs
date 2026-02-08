using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyTaskEFDBFirst.DTOs.SignUps.Request;
using MyTaskEFDBFirst.DTOs.Tasks.Request;
using MyTaskEFDBFirst.Models;
using static MyTaskEFDBFirst.Helpers.Enums.MyTasksEnums;

namespace MyTaskEFDBFirst.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TestdbContext _testdbContext;

        public TasksController(TestdbContext testdbContext) 
        {
            _testdbContext = testdbContext;   
        }

        [HttpPost]
        [Route("add-task")]
        public async Task<IActionResult> Create([FromBody] CreateTaskInputDTO input)
        {
            var message = "0";
            try
            {
                //check if user id exist
                if (!_testdbContext.Users.Any(x => x.Id == input.userId))
                    throw new Exception("User does not exists");
                //end date greater than start date
                if (input.StartDate >= input.EndDate)
                    throw new Exception("End date should be after start date");
                //title not empty and at least 5 char
                if (string.IsNullOrEmpty(input.Title) || input.Title.Length < 5)
                    throw new Exception("Title is required and should contain at least 5 charachter");
                Models.Task task = new Models.Task();
                task.Title = input.Title;
                task.Description =input.Description;
                task.Status = MyTaskStatus.New.ToString();
                task.Start=input.StartDate;
                task.End=input.EndDate;
                task.UserId= input.userId;
                await _testdbContext.AddAsync(task);
                await _testdbContext.SaveChangesAsync();

                return StatusCode(200, message);
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("update-task")]
        public async Task<IActionResult> Update([FromBody] UpdateTaskInputDTO input)
        {
            var message = "0";
            try
            {
                //for best result, do get first first then check if it exists(FOR THE UPDATE)
                var task = _testdbContext.Tasks.Where(x => x.Id == input.Id).SingleOrDefault();
                if (task != null)
                {
                    //validate what's in the request 
                    if (!string.IsNullOrEmpty(input.Title))
                    {
                        //title not empty and at least 5 char
                        if (string.IsNullOrEmpty(input.Title) || input.Title.Length < 5)
                            throw new Exception("Title is required and should contain at least 5 charachter");
                        else
                            task.Title = input.Title;

                    }
                    if (!string.IsNullOrEmpty(input.Description))
                    {
                        task.Description = input.Description;
                    }
                    if (!string.IsNullOrEmpty(input.Status))
                    {
                        if (!Enum.TryParse(input.Status, false, out MyTaskStatus status))
                            throw new Exception("Invalid Status Value");
                        task.Status = input.Status;
                    }
                    if (input.StartDate != null)
                    {
                        if (input.EndDate != null)
                        {
                            if (input.StartDate >= input.EndDate)
                                throw new Exception("End date should be after start date");
                        }
                        if (input.StartDate >= task.End)
                            throw new Exception("End date should be after start date");
                        task.Start = input.StartDate;
                    }

                    if (input.EndDate != null)
                    {
                        if (task.Start >=input.EndDate)
                            throw new Exception("End date should be after start date");
                        task.End = input.EndDate;
                    }
                    _testdbContext.Update(task);
                    _testdbContext.SaveChanges();
                }
            
                throw new Exception($"No task with the given ID {input.Id}");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

    }
}
