using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using MyPoll.Model;
using PRBD_Framework;

//----------------------------- VM correspondant à une cellule de la grille dynamique -----------------------------------

namespace MyPoll.ViewModel {
    public class VoteParticipantsChoiceViewModel : ViewModelCommon {

//------------------------------ Command ---------------------------------------

        public ICommand ChangeVoteYes { get; set; }
        public ICommand ChangeVoteNo { get; set; }
        public ICommand ChangeVoteMaybe { get; set; }

//------------------------------ Property ---------------------------------------


        private User User { get; set; }
        private Poll poll { get; set; }

        private bool _editMode;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }

        public Vote Vote { get; set; }

        private bool _hasVoted;
        public bool HasVoted {
            get => _hasVoted;
            set => SetProperty(ref _hasVoted, value);
        }

        public bool HasVotedYes {
            get { return Vote.Type == VoteType.Yes; }
        }

        public bool HasVotedNo {
            get => Vote.Type == VoteType.No;
        }

        public bool HasVotedMaybe {
            get => Vote.Type == VoteType.Maybe;
        }


//------------------------------ Constructeur ---------------------------------------


        public VoteParticipantsChoiceViewModel(User user, Choice choice) {
            poll = choice.Poll;
            User = user;

            HasVoted = Context.Votes.Any(v => v.User == user && v.Choice == choice);
            Vote = user.Votes.SingleOrDefault(v => v.Choice.ChoiceId == choice.ChoiceId,
                new Vote() {
                    UserId = user.UserId, ChoiceId = choice.ChoiceId, Type = VoteType.None
                });

            ChangeVoteYes = new RelayCommand(() => UpdateVote(VoteType.Yes));
            ChangeVoteNo = new RelayCommand(() => UpdateVote(VoteType.No));
            ChangeVoteMaybe = new RelayCommand(() => UpdateVote(VoteType.Maybe));

            RaisePropertyChanged();
        }


//------------------------------ UpdateVotesCommand ---------------------------------------


        private void UpdateVote(VoteType voteType) {
            if (poll.Type == PollType.Multiple) {
                Vote.Type = (Vote.Type == voteType) ? VoteType.None : voteType;
            } else {
                Vote userVote = Context.Votes.SingleOrDefault(v => v.Choice.Poll == poll && CurrentUser == v.User &&
                                                                   v.Type != VoteType.None);

                if (userVote != null) {
                    userVote.Type = VoteType.None;
                }
                Vote.Type = (Vote.Type == voteType) ? VoteType.None : voteType;
            }
            HasVoted = true;
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);


            RaisePropertyChanged(nameof(HasVotedYes));
            RaisePropertyChanged(nameof(HasVotedNo));
            RaisePropertyChanged(nameof(HasVotedMaybe));
            RaisePropertyChanged(nameof(VotedIcon));
            RaisePropertyChanged(nameof(VotedColor));
            RaisePropertyChanged(nameof(VotedToolTip));
        }



//------------------------------ Gestion des icones / Couleurs ---------------------------------------

        public EFontAwesomeIcon VotedIcon => HasVoted ? Vote.Type switch {
                VoteType.Yes => EFontAwesomeIcon.Solid_Check,
                VoteType.No => EFontAwesomeIcon.Solid_Xmark,
                VoteType.Maybe => EFontAwesomeIcon.Solid_CircleQuestion,
                _ => EFontAwesomeIcon.None
        } : EFontAwesomeIcon.None;


        public Brush VotedColor => HasVoted ?  (Vote.Type switch {
            VoteType.Yes => Brushes.Green,
            VoteType.No => Brushes.Red,
            VoteType.Maybe => Brushes.Orange,
            _ => Brushes.White
        }) : Brushes.White;

        public string VotedToolTip => Vote.Type switch {
            VoteType.Yes => "Yes",
            VoteType.No => "No",
            VoteType.Maybe => "Maybe",
            _ => ""
        };
    }
}
