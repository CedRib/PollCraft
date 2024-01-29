using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;
namespace MyPoll.ViewModel;

//----------------------------- VM correspondant à une ligne de la grille dynamique -----------------------------------


public class VoteParticipantsViewModel : ViewModelCommon {

//------------------------------ Command ---------------------------------------


    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand DeleteCommand { get; }


//------------------------------ Property  ---------------------------------------


    private PollDetailViewModel _pollDetailViewModel;
    public User User { get; }

    private List<Choice> _choices;

    private List<VoteParticipantsChoiceViewModel> _voteVM;
    public List<VoteParticipantsChoiceViewModel> VoteVM {
        get => _voteVM;
        set => SetProperty(ref _voteVM, value);
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value, EditModeChanged);
    }

    public bool Editable => !EditMode && !ParentEditMode &&
                            (User.UserId == CurrentUser.UserId || IsAdmin) &&
                            !_pollDetailViewModel.Poll.IsClosed;
    private bool ParentEditMode => _pollDetailViewModel.EditMode;


//------------------------------ Constructeur ---------------------------------------


    public VoteParticipantsViewModel(PollDetailViewModel pollDetailViewModel, User user, List<Choice> choices) {
        _pollDetailViewModel = pollDetailViewModel;
        _choices = choices;
        User = user;

        RefreshVote();

        EditCommand = new RelayCommand(() => EditMode = true);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        DeleteCommand = new RelayCommand(Delete);
    }


//------------------------------ FonctionCommandAction ---------------------------------------


    private void EditModeChanged() {
        foreach (VoteParticipantsChoiceViewModel vPCVM in _voteVM) {
            vPCVM.EditMode = EditMode;
        }
        _pollDetailViewModel.AskEditMode(EditMode);
    }

    public void Changes() {
        RaisePropertyChanged(nameof(Editable));
    }

    private void RefreshVote() {
        VoteVM = _choices.Select(c => new VoteParticipantsChoiceViewModel(User, c)).ToList();
    }

    private void Save() {
        EditMode = false;
            User.Votes = VoteVM.Where(v => v.HasVoted)
                .Select(v => v.Vote).ToList();
            Context.SaveChanges();
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }

    private void Cancel() {
        EditMode = false;
        VoteVM.ForEach(v => v.Vote.Reload());
        RefreshVote();
    }

    private void Delete() {
        User.Votes.Where(v =>
                (v.UserId == CurrentUser.UserId || IsAdmin) && v.Choice.Poll.PollId == _pollDetailViewModel.Poll.PollId)
            .ToList()
            .ForEach(vote => vote.Type = VoteType.None);
        Context.SaveChanges();
        RefreshVote();
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }
}
