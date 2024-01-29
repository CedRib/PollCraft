using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;

public class Participation : EntityBase<MyPollContext>
{
    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    [Required]
    public virtual User User { get; set; }

}
