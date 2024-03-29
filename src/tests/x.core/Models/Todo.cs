﻿using Ws.Core.Extensions.Data;

namespace x.core.Models;

public class Todo: IEntity<int>, IApp
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public bool Completed { get; set; } = false;
}
