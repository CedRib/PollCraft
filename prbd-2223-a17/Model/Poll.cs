using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRBD_Framework;

namespace MyPoll.Model;

public enum PollType {
    Multiple,
    Single

}

public class Poll : EntityBase<MyPollContext> {

    public Poll() {
    }

    [Key] public int PollId { get; set; }

    [Required] public string Title { get; set; }

    public PollType Type { get; set; }
    public bool IsClosed { get; set; }

    [Required]
    [ForeignKey(nameof(Creator))]
    public int CreatorId { get; set; }

    [Required] public virtual User Creator { get; set; }

    public int ParticipantCount => Participations?.Count ?? 0;
    public int CounterVote => Choices.Sum(c => c.Votes.Count(v => v.Type != VoteType.None));
    public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();
    public virtual ICollection<Choice> Choices { get; set; } = new HashSet<Choice>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    [NotMapped]
    public ICollection<Choice> MostVotedChoice =>
        Choices.Where(c => c.Votes.Any(v => v.Type != VoteType.None) &&
                           c.Votes.Sum(v => v.Type == VoteType.Yes ? 1 : v.Type == VoteType.No ? -1 : v.Type == VoteType.Maybe ? 0.5 : 0)
                           >= Choices.Max(ch => ch.Votes.Any(v => v.Type != VoteType.None) ?
                               ch.Votes.Sum(v => v.Type == VoteType.Yes ? 1 : v.Type == VoteType.No ? -1 : v.Type == VoteType.Maybe ? 0.5 : 0) : double.MinValue))
            .ToList();

    private string PollColor() {
        if (IsClosed) {
            return "#FFE6DC";
        }
        bool allVotesNone = Choices.SelectMany(c => c.Votes).All(v => v.Type == VoteType.None);
        if (allVotesNone || !userVotedIds.Contains(App.CurrentUser.UserId) ) {
            return "#D3C095";
        }
        return "#C4E0C4";
    }


    private ICollection<int> userVotedIds =>
        (Context.Votes.Where(v => v.Choice.PollId == PollId).Select(vote => vote.UserId).ToList());

    public string PColor => PollColor();
}
