using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;

public class Administrator : User
{
    public Administrator() {
        Role = Role.Administrator;
    }

    // public Administrator(int id, string name, string email, string password)
    //     : base(id, name, email, password)
    // {
    //     Role = Role.Administrator;
    // }
}
