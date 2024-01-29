using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;

public class Comment
{
    [Key]
    public int CommentId { get; set; }
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    [Required]
    public virtual User User { get; set; }
    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }
    public string CommentText { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;

    public Comment() { }


}
