using System;

namespace p48_lessons_asp.Models;

public class ProductListItemModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
