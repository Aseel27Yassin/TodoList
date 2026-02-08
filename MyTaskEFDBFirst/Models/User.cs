using System;
using System.Collections.Generic;

namespace MyTaskEFDBFirst.Models;

public partial class User
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int Id { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
