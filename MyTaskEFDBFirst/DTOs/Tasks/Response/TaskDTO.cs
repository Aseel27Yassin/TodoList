namespace MyTaskEFDBFirst.DTOs.Tasks.Response
{
    public class TaskDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public string DeadLine { get; set; }
    }
}
