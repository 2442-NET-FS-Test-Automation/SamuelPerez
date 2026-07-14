using System.ComponentModel.DataAnnotations;

namespace Library.Data.Entities;

public class User
{
    public int Id {get; set;}
    [MaxLength(64)]
    
    public string UserName {get; set;} = "";
    public string PasswordHash {get; set;} = "";
    public string Role {get; set;} = "consumer";
}