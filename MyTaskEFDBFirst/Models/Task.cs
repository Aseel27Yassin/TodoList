using System;
using System.Collections.Generic;

namespace MyTaskEFDBFirst.Models;

public partial class Task
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    public int Id { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
