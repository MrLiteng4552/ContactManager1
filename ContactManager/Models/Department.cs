using System.Collections.Generic;

namespace ContactManager.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
