using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyTaskEFDBFirst.DTOs.SignUps.Request;
using MyTaskEFDBFirst.DTOs.Tasks.Request;
using MyTaskEFDBFirst.DTOs.Tasks.Response;
using MyTaskEFDBFirst.DTOs.Users.Response;
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

        //Create
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

        //Update
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


        //Get Task
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var task = await _testdbContext.Tasks.FindAsync(id);

                if(task == null)
                {
                    return NotFound($"No Task With The Given Id {id}");
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        //get all tasks
        [HttpGet("tasks")]
        public async Task<IActionResult> ListAll()
        {
            try
             {
                var items = _testdbContext.Tasks.ToList();
                var list = new List<TaskDTO>();
                foreach (var item in items)
                        {
                            list.Add(new TaskDTO
                            {
                                Id = item.Id,
                                Title = item.Title,
                                DeadLine = item.End.ToString(),
                                Status = item.Status
                            });
                        }
                    return Ok(list);
                }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("tasks-select")]
        public async Task<IActionResult> ListAll2()
        {
            try
            {
                //creating object for every record I have
                var items = _testdbContext.Tasks.Select(x => new TaskDTO
                {
                    Id = x.Id,
                    Title = x.Title,
                    DeadLine = x.End.ToString(),
                    Status = x.Status

                }).ToList();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        } 


        [HttpGet("tasks-query")]
        public async Task<IActionResult> ListAll3()
        {
            try
            {
                //sql background
                //Select Id,Title,Status,End As DeadLine From Tasks
                var items = from t in _testdbContext.Tasks
                            select new TaskDTO
                            {
                                Id = t.Id,
                                Title = t.Title,
                                DeadLine = t.End.ToString(),
                                Status = t.Status
                            };
                    return Ok(items.ToList());
                }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("list-user")]
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                var query = _testdbContext.Users.Select(x => new UserDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
                return Ok(query);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("join-users")]
        public async Task<IActionResult> TestJoin()
        {
            try
            {
                var users = _testdbContext.Users.ToList();
                var tasks=_testdbContext.Tasks.ToList();

                var joinQuery = from u in users
                                join t in tasks on u.Id equals t.UserId
                                select new //Annoynmous Object
                                {
                                    User= u,
                                    MyTask=t
                                };
                var test = joinQuery.ToList();
                foreach(var item in joinQuery)
                {
                    Console.WriteLine(item.MyTask.Id);
                }
                return Ok(test);
            }
            catch(Exception ex) 
            {
                return StatusCode(500,ex.Message);
            }
        }


        [HttpGet("join-users-2")]
        public async Task<IActionResult> TestJoinFromDb()
        {
            try
            {
               
                var joinQuery = from u in _testdbContext.Users
                                join t in _testdbContext.Tasks on u.Id equals t.UserId
                                select new //Annoynmous Object
                                {
                                    Id=t.Id,
                                    Username=u.Name,
                                    Title=t.Title,
                                    DeadLine=t.End.ToString(),
                                    Status=t.Status
                                };
                return Ok(joinQuery.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("remove/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var task = _testdbContext.Tasks.Where(x => x.Id == Id).SingleOrDefault();
                if (task != null)
                {
                   _testdbContext.Remove(task);
                    _testdbContext.SaveChanges();
                    return Ok("removed");
                }
                throw new Exception("No task exist with the selected Id");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
