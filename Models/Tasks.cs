using System;

namespace p48_lessons_asp_main.Models
{
    public class Tasks{
    public Guid Id{get;set;}=Guid.NewGuid();
    public bool Completed{get;set;}
    public string Title{get;set;}=string.Empty;
    public string? Description{get;set;}
    public DateTime CreatedAt{get;set;}=DateTime.UtcNow;
    public DateTime DeadlineDate{get;set;}
    }
}

