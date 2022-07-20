using Ws.Core.Extensions.Data;

namespace x.core.Models.Cms;

public class Admin_User: IEntity<int>
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool Is_Active { get; set; }
    public bool Blocked { get; set; }
    public string? Prefered_Language { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
    public virtual ICollection<Admin_Users_Roles_Links>? Roles { get; set; }
}

public class Admin_Role : IEntity<int>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}

public class Admin_Users_Roles_Links : IEntity<string>
{
    private string? _id; 
    public string? Id { get => _id ?? $"{Admin_UserId}-{Admin_RoleId}"; set => _id = value; }
    public int Admin_UserId { get; set; }
    public int Admin_RoleId { get; set; }
}


public class Admin_Permission : IEntity<int>
{
    public int Id { get; set; }
    public string? Action { get; set; }
    public string? Subject { get; set; }
    public string? Properties { get; set; }
    public string? Conditions { get; set; }
    public string? Created_At { get; set; }
    public string? Updated_At { get; set; }
}
