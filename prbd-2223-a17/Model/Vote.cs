    using PRBD_Framework;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    namespace MyPoll.Model;

    public enum VoteType
    {
        Yes,
        No,
        Maybe,
        None,
    }

    public class Vote : EntityBase<MyPollContext>
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [Required]
        public virtual User User { get; set; }
        [ForeignKey(nameof(Choice))]
        public int ChoiceId { get; set; }
        [Required]
        public virtual Choice Choice { get; set; }
        public VoteType Type { get; set; }

        public Vote() { }


    }


