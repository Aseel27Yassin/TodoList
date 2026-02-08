namespace MyTaskEFDBFirst.DTOs.Tasks.Request
{
    public class CreateTaskInputDTO
    {
        public string? Description { get; set; }

        public string Title { get; set; }

        public int userId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
