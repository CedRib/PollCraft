using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;

public class Choice : EntityBase<MyPollContext>
{
    [Key]
    public int ChoiceId { get; set; }
    [ForeignKey(nameof(Poll))]
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }
    public string ChoiceText { get; set; }

    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();

    public double Score => Votes.Sum(v => v.Type == VoteType.Yes ? 1 : v.Type == VoteType.No ? -1 : v.Type == VoteType.Maybe ? 0.5 : 0);

    public int VoteCount => Votes.Count(v =>v.Type != VoteType.None);
    public Choice() { }


}
