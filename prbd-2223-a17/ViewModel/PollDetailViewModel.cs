using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;
namespace MyPoll.ViewModel;

//------------------------------ VM reprennant les Comments et la grille générale de la grille dynamique ---------------------------------------


public class PollDetailViewModel : ViewModelCommon {

//------------------------------ Command ---------------------------------------

    public ICommand AddCommentCommand { get; set; }
    public ICommand PersistCommentCommand { get; set; }
    public ICommand DeleteCommentCommand { get; set; }
    public ICommand EditPollCommand { get; set; }
    public ICommand DeletePollCommand { get; set; }
    public ICommand ReopenPollCommand { get; set; }

//------------------------------ PollProperty ---------------------------------------

    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private bool _editPollMode = false;
    public bool EditPollMode {
        get => _editPollMode;
        set => SetProperty(ref _editPollMode, value);
    }

    public bool CanReopen => (Poll.CreatorId == CurrentUser.UserId || IsAdmin) && Poll.IsClosed;

//------------------------------ CommentProperty ---------------------------------------


    private string _comment;
    public string Comment {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }

    private ObservableCollection<Comment> _comments;
    public ObservableCollection<Comment> Comments {
        get => _comments;
        set => SetProperty(ref _comments, value);
    }
    private bool _addCommentMode;
    public bool AddCommentMode {
        get => _addCommentMode;
        set => SetProperty(ref _addCommentMode, value);
    }

    private bool _isDeletable;
    public bool IsDeletable {
        get => _isDeletable;
        set => SetProperty(ref _isDeletable, value);
    }

//------------------------------ ChoiceProperty ---------------------------------------


    private List<Choice> _choices;
    public List<Choice> Choices => _choices;

    private List<VoteParticipantsViewModel> _participantsVM;
    public List<VoteParticipantsViewModel> ParticipantsVM => _participantsVM;

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

//------------------------------ VM lors d'une edition de Poll, initialisée à l'édition d'un poll ---------------------------------------


    private AddPollViewModel _editPollViewModel;
    public AddPollViewModel EditPollViewModel {
        get => _editPollViewModel;
        set => SetProperty(ref _editPollViewModel, value);
    }



//------------------------------ Constructeur ---------------------------------------


    public PollDetailViewModel(Poll poll) {
        Poll = poll;
        _isDeletable = Poll.CreatorId == CurrentUser.UserId || IsAdmin;

        _choices = Context.Choices
            .Where(c => c.Poll.PollId == Poll.PollId)
            .OrderBy(c => c.ChoiceText)
            .ToList();

        List<User> participants = Context.Participations
            .Where(p => p.Poll.PollId == Poll.PollId)
            .Select(p => p.User)
            .OrderBy(p => p.FullName)
            .ToList();
        _participantsVM = participants.Select(p => new VoteParticipantsViewModel(this, p, _choices)).ToList();

        Comments = new ObservableCollection<Comment>(Context.Comments
            .Where(c => c.Poll.PollId == Poll.PollId)
            .OrderByDescending(c => c.DateTime)
            .ToList());

        AddCommentCommand = new RelayCommand(() => {
            if (!Poll.IsClosed) {
                AddCommentMode = true;
            }
        });
        PersistCommentCommand = new RelayCommand(PersistCommentAction,() => !Comment.IsNullOrEmpty() );
        DeleteCommentCommand = new RelayCommand<Comment>(DeleteCommentAction);

        EditPollCommand = new RelayCommand(() => {
            EditPollMode = true;
            EditPollViewModel = new AddPollViewModel(this);
        });
        DeletePollCommand = new RelayCommand<Poll>(DeletePollAction);
        ReopenPollCommand = new RelayCommand(ReopenPollAction);

        RaisePropertyChanged();
    }

    public PollDetailViewModel(){}

//------------------------------ CommentFunction ---------------------------------------

    private void PersistCommentAction() {
        AddCommentMode = false;
        Comment comment = new Comment { CommentText = Comment, DateTime = DateTime.Now, PollId = Poll.PollId, UserId = CurrentUser.UserId};
        Comments.Insert(0, comment);
        Context.Comments.Add(comment);
        Comment = "";
        Context.SaveChanges();
    }

    private void DeleteCommentAction(Comment comment) {
        Comments.Remove(comment);
        Context.Comments.Remove(comment);
        Context.SaveChanges();
    }

//------------------------------ PollFunction ---------------------------------------


    private void DeletePollAction(Poll poll) {
        var result = MessageBox
            .Show("You're about to delete " + poll.Title + ", do you want to proceed ?",
                "Confirmation", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes) {
            Context.Polls.Remove(poll);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_DELETE_POLL, poll);
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
        }
    }

    private void ReopenPollAction() {
        Poll.IsClosed = false;
            ParticipantsVM.ForEach(vpvm => vpvm.Changes());
            RaisePropertyChanged(nameof(CanReopen));
            Context.SaveChanges();
            NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }

    public void AskEditMode(bool editMode) {
        EditMode = editMode;
        foreach (var p in ParticipantsVM)
            p.Changes();
    }
}
