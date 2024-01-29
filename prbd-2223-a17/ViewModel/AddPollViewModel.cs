using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class AddPollViewModel : ViewModelCommon {

    public ICommand AddParticipantCommand { get; set; }
    public ICommand DeleteParticipantCommand { get; set; }
    public ICommand AddMyselfCommand { get; set; }
    public ICommand AddEverybodyCommand { get; set; }
    public ICommand AddChoiceCommand { get; set; }
    public ICommand EditChoiceCommand { get; set; }
    public ICommand DeleteChoiceCommand { get; set; }
    public ICommand SavePollCommand { get; set; }
    public ICommand CancelNewPollCommand { get; set; }
    public ICommand CancelChoiceCommand { get; set; }
    public ICommand SaveChoiceCommand { get; set; }

//----------------------------- PollProperty -----------------------------------

    private PollDetailViewModel _pollDetailViewModel;

    private Poll _pollToEdit;
    public Poll PollToEdit {
        get => _pollToEdit;
    }

    private string _pollName;
    public string PollName {
        get => _pollName;
        set => SetProperty(ref _pollName, value, () => Validate());
    }

    private User _user;
    public User User {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    private int _pollType;
    public int PollType {
        get => _pollType;
        set => SetProperty(ref _pollType, value);
    }

    private bool _isChecked;
    public bool IsChecked {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }

    public bool EditPollMode => _pollDetailViewModel.EditPollMode;

    public bool IsSelected => PollToEdit.Type == Model.PollType.Multiple;

    public bool isComboBoxEnabled => PollToEdit.Type == Model.PollType.Multiple &&
                                     PollToEdit.Choices.SelectMany(c => c.Votes)
                                         .GroupBy(v => v.UserId)
                                         .Any(g => g.Count() > 1);


//----------------------------- ParticipantProperty -----------------------------------

    // User selectionné dans la ComboBox
    private User _selectedUser;
    public User SelectedUser {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    // Liste de user dans la ComboBox
    private ObservableCollection<User> _participants;
    public ObservableCollection<User> Participants {
        get => _participants;
        set => SetProperty(ref _participants, value);
    }

    // Liste de user selectionné
    private ObservableCollection<User> _selectedParticipants;
    public ObservableCollection<User> SelectedParticipants {
        get => _selectedParticipants;
        set => SetProperty(ref _selectedParticipants, value);
    }

    // Liste de participation à sauver en DB si l'utilisateur save
    private ICollection<Participation> _savedParticipants;
    public ICollection<Participation> SavedParticipants {
        get => _savedParticipants;
        set => SetProperty(ref _savedParticipants, value);
    }

    // Liste de ViewModel bindé à la vue, se créé à partir de SelectedParticipant
    private List<ParticipantListViewModel> _participantListVm = new();
    public List<ParticipantListViewModel> ParticipantListVM {
        get => _participantListVm;
        set => SetProperty(ref _participantListVm, value);
    }

    public bool IsParticipantSelected => SelectedParticipants.Count == 0;

    public bool AddMySelfBtn => Participants.Contains(CurrentUser);

    public bool AddEverybodyBtn => !Participants.IsNullOrEmpty();


//----------------------------- ChoiceProperty -----------------------------------


    // Liste de Choice selectionné
    private ObservableCollection<Choice> _choices;
    public ObservableCollection<Choice> Choices {
        get => _choices;
        set => SetProperty(ref _choices, value);
    }

    // Liste de Choice à sauver en DB si l'utilisateur save
    private ICollection<Choice> _savedChoices;
    public ICollection<Choice> SavedChoices {
        get => _savedChoices;
        set => SetProperty(ref _savedChoices, value);
    }

    // Choix qui va recevoir le texte du TextBox et créer un new Choice()
    private Choice _choice;
    public Choice Choice {
        get => _choice;
        set => SetProperty(ref _choice, value);
    }

    // String Bindé sur le TextBox où on ajoute/edite le choix
    private string _choiceText;
    public string ChoiceText {
        get => _choiceText;
        set => SetProperty(ref _choiceText, value);
    }

    // Liste de ViewModel bindé à la vue, se créé à partir de Choices
    private List<ChoiceListViewModel> _choiceListVM;
    public List<ChoiceListViewModel> ChoiceListVM {
        get => _choiceListVM;
        set => SetProperty(ref _choiceListVM, value);
    }

    public bool IsChoiceSelected {
        get => Choices.Count == 0;
    }

    private bool _isEditChoice;
    public bool IsEditChoice {
        get => _isEditChoice;
        set => SetProperty(ref _isEditChoice, value);
    }


//----------------------------- Constructeur -----------------------------------


    public AddPollViewModel(PollDetailViewModel pollDetailViewModel) {
        _pollDetailViewModel = pollDetailViewModel;
        _user = CurrentUser;
        _pollToEdit = _pollDetailViewModel.Poll;
        _pollName = _pollDetailViewModel.Poll.Title;
        _isChecked = _pollDetailViewModel.Poll.IsClosed;
        _savedChoices = _pollDetailViewModel.Poll.Choices;
        _selectedParticipants = new ObservableCollection<User>(PollToEdit.Participations.Select(p => p.User));
        _choices = new ObservableCollection<Choice>(_pollDetailViewModel.Poll.Choices);
        _participants = new ObservableCollection<User>(Context.Users.Where(u => !SelectedParticipants.Contains(u)).ToList());
        _savedParticipants = new HashSet<Participation>(_pollDetailViewModel.Poll.Participations);

        ParticipantListVM = SelectedParticipants.Select(u => new ParticipantListViewModel(u, PollToEdit)).ToList();
        ChoiceListVM = Choices.Select(c => new ChoiceListViewModel(PollToEdit, c)).ToList() ;

        AddParticipantCommand = new RelayCommand(AddParticipantAction);
        AddMyselfCommand = new RelayCommand(AddMyselfAction);
        DeleteParticipantCommand = new RelayCommand<User>(DeleteParticipantAction);
        AddEverybodyCommand = new RelayCommand(AddEverybodyAction);

        AddChoiceCommand = new RelayCommand(AddChoiceAction);
        EditChoiceCommand = new RelayCommand<Choice>((choice) => {
            _isEditChoice = true;
            Choice = choice;
            ChoiceText = Choice.ChoiceText;
            RaisePropertyChanged();
        });
        CancelChoiceCommand = new RelayCommand(() => {
            _isEditChoice = false;
            ChoiceText = "";
            RaisePropertyChanged();
        });
        DeleteChoiceCommand = new RelayCommand<Choice>(DeleteChoiceAction);

        SavePollCommand = new RelayCommand(SavePollAction, () => Validate());
        CancelNewPollCommand = new RelayCommand(CancelNewPollAction);

        RaisePropertyChanged();
    }

//----------------------------- ActionParticipantFonction -----------------------------------


    private void AddParticipantAction() {
        if (!SelectedParticipants.Contains(SelectedUser) && SelectedUser != null) {
            SelectedParticipants.Add(SelectedUser);
            ParticipantListVM.Add(new ParticipantListViewModel(SelectedUser, PollToEdit));
            ParticipantListVM = SelectedParticipants.Select(u => new ParticipantListViewModel(u, PollToEdit))
                .OrderBy(vm => vm.User.FullName).ToList();
            Participation newParticipation = new Participation {
                User = SelectedUser, Poll = _pollToEdit
            };
            SavedParticipants.Add(newParticipation);
            Participants.Remove(SelectedUser);
        }
        RaisePropertyChanged(nameof(IsParticipantSelected));
        RaisePropertyChanged(nameof(AddEverybodyBtn));

    }

    private void AddMyselfAction() {
        if (!SelectedParticipants.Contains(CurrentUser)) {
            _selectedParticipants.Add(CurrentUser);
            ParticipantListVM = SelectedParticipants.Select(u => new ParticipantListViewModel(u, PollToEdit)).ToList();
            Participation newParticipation = new Participation {
                User = CurrentUser, Poll = _pollToEdit
            };
            SavedParticipants.Add(newParticipation);
            Participants.Remove(CurrentUser);
        }
        RaisePropertyChanged(nameof(IsParticipantSelected));
        RaisePropertyChanged(nameof(AddMySelfBtn));
        RaisePropertyChanged(nameof(AddEverybodyBtn));
    }

    private void AddEverybodyAction() {
        foreach (User user in Participants) {
            if (!SelectedParticipants.Contains(user)) {
                SelectedParticipants.Add(user);
                ParticipantListVM = SelectedParticipants.Select(u => new ParticipantListViewModel(u, PollToEdit)).ToList();
                Participation newParticipation = new Participation {
                    User = user,
                    Poll = _pollToEdit
                };
                SavedParticipants.Add(newParticipation);
            }
        }
        foreach (User user in SelectedParticipants) {
            Participants.Remove(user);
        }
        RaisePropertyChanged(nameof(IsParticipantSelected));
        RaisePropertyChanged(nameof(AddMySelfBtn));
        RaisePropertyChanged(nameof(AddEverybodyBtn));

    }

    private void DeleteParticipantAction(User participant) {
        if (participant.GetVoteCountForPoll(PollToEdit.PollId) < 1) {
            DeleteParticipant(participant);
        } else {
            var result = MessageBox
                .Show("This participant have at least one vote. If you proceed, this user and his vote will be deleted ?",
                    "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                DeleteParticipant(participant);
            }
        }
        RaisePropertyChanged(nameof(AddMySelfBtn));
        RaisePropertyChanged(nameof(AddEverybodyBtn));
    }

    private void DeleteParticipant(User participant) {
        SelectedParticipants.Remove(participant);
        ParticipantListVM = SelectedParticipants.Select(u => new ParticipantListViewModel(u, PollToEdit)).ToList();
        SavedParticipants.Clear();
        foreach (User user in SelectedParticipants) {
            Participation newParticipation = new Participation {
                User = user,
                Poll = _pollToEdit
            };
            SavedParticipants.Add(newParticipation);
        }
        Participants.Add(participant);
        RaisePropertyChanged(nameof(IsParticipantSelected));
    }


//----------------------------- ChoiceActionFonction -----------------------------------


    private void AddChoiceAction() {
        if (!ChoiceText.IsNullOrEmpty() && !IsEditChoice) {
            Choice = new Choice { ChoiceText = ChoiceText };
            Choices.Add(Choice);
            ChoiceListVM = Choices.Select(c => new ChoiceListViewModel(PollToEdit, c)).ToList() ;
            Choice savedChoice = new Choice { ChoiceText = Choice.ChoiceText };
            SavedChoices.Add(savedChoice);
            RaisePropertyChanged(nameof(IsChoiceSelected));
            ChoiceText = "";
        }
        else if (!ChoiceText.IsNullOrEmpty() && IsEditChoice) {
            var result = MessageBox
                .Show("If you save this modification, you can only go back with a new edition of this choice, are you agree  ?",
                    "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                IsEditChoice = false;
                int index = Choices.IndexOf(Choice);
                Choice.ChoiceText = ChoiceText;
                Choices[index] = Choice;
                ChoiceListVM = Choices.Select(c => new ChoiceListViewModel(PollToEdit, c)).ToList();
                ChoiceText = "";
                RaisePropertyChanged();
            }
        }
    }

    private void DeleteChoiceAction(Choice choice) {
        if (choice.VoteCount < 1) {
            DeleteChoice(choice);
        } else {
            var result = MessageBox
                .Show("This choice has at least one vote, do you want to proceed and delete this choice ?",
                    "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                DeleteChoice(choice);
            }
        }
    }

    private void DeleteChoice(Choice choice) {
        Choices.Remove(choice);
        ChoiceListVM = Choices.Select(c => new ChoiceListViewModel(PollToEdit, c)).ToList();
        SavedChoices.Remove(choice);
        RaisePropertyChanged(nameof(IsChoiceSelected));
        RaisePropertyChanged(Choices);
    }




//----------------------------- PollActionFonction -----------------------------------


    private void SavePollAction() {
        if (!EditPollMode) {
            PollToEdit.Creator = CurrentUser;
            SavePoll();
            Context.Polls.Add(_pollToEdit);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_CLOSE_NEW_POLL_TAB);
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, PollToEdit);
        }
        else {
            SavePoll();
            Context.SaveChanges();
            RaisePropertyChanged(nameof(_pollDetailViewModel));
            RaisePropertyChanged();
            NotifyColleagues(App.Messages.MSG_CLOSE_EDIT_TAB, PollToEdit);
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, PollToEdit);
        }
        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
    }

    private void SavePoll() {
        PollToEdit.Choices = SavedChoices;
        PollToEdit.Title = PollName;
        PollToEdit.Participations = SavedParticipants;
        PollToEdit.Type = PollType == 1 ? Model.PollType.Single : Model.PollType.Multiple;
        PollToEdit.IsClosed = IsChecked;
    }

    private void CancelNewPollAction() {
        if (EditPollMode) {
            ClearErrors();
            NotifyColleagues(App.Messages.MSG_CLOSE_EDIT_TAB, PollToEdit);
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, PollToEdit);

        } else {
            ClearErrors();
            NotifyColleagues(App.Messages.MSG_CLOSE_NEW_POLL_TAB);
        }
    }

    public override bool Validate() {
        ClearErrors();
        if (string.IsNullOrEmpty(PollName)) {
            AddError(nameof(PollName), "This field is required");
        }
        if (PollName.Length < 3) {
            AddError(nameof(PollName), "Title must be > 3 character");
        }
        return !HasErrors;
    }
}
