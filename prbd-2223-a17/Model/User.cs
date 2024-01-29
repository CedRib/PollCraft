using PRBD_Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPoll.Model;

public enum Role
{
    User = 1,
    Administrator = 2
}

public class User : EntityBase<MyPollContext>
{
    [Key]
    public int UserId { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public Role Role { get; protected set; } = Role.User;

    public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    [NotMapped]
    public List<Vote> Votes {
        get => (from v in Context.Votes where v.UserId == UserId select v).ToList();
        set => value.Except(Context.Votes).ToList().ForEach(vote => Context.Votes.Add(vote));
    }

    [NotMapped]
    public static List<string> AllAppEmail {
        get => (from u in Context.Users select u.Email).ToList();
    }

    public int GetVoteCountForPoll(int pollId) {
        return Participations
            .Where(p => p.PollId == pollId)
            .SelectMany(p => p.Poll.Choices)
            .SelectMany(c => c.Votes)
            .Where(v => v.Type != VoteType.None)
            .Count(v => v.UserId == UserId);
    }



    public User() { }
}


