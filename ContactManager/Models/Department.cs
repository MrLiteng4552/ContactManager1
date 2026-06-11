using System.Collections.Generic;

namespace ContactManager.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Связь: у одного отдела может быть много контактов
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
