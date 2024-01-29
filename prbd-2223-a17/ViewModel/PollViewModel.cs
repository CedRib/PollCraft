using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;


namespace MyPoll.ViewModel;

public class PollViewModel : ViewModelCommon {

    public ICommand ClearFilter { get; set; }
    public ICommand DisplayPollDetails { get; set; }
    public ICommand NewPollCommand { get; set; }

    private ObservableCollection<Poll> _polls;
    public ObservableCollection<Poll> Polls {
        get => _polls;
        set  => SetProperty(ref _polls, value);
    }

    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value, ApplyFilter);
    }

    public PollViewModel() {
        OnRefreshData();

        ClearFilter = new RelayCommand(() => Filter = "");
        DisplayPollDetails = new RelayCommand<Poll>((poll) => {
            NotifyColleagues(App.Messages.MSG_DISPLAY_POLL, poll);
        });
        NewPollCommand = new RelayCommand(() => {
            NotifyColleagues(App.Messages.MSG_NEW_POLL);
        });
    }

    protected sealed override void OnRefreshData() {
        if (IsAdmin) {
            Polls = new ObservableCollection<Poll>(Context.Polls.OrderBy(p=> p.Title));
        } else {
            Polls = new ObservableCollection<Poll>(CurrentUser.Participations.Select(p => p.Poll)
                .Union(Context.Polls.Where(p => p.Creator == CurrentUser)).OrderBy(p => p.Title));
        }
        ApplyFilter();
    }

    private void ApplyFilter() {
        if (Filter.IsNullOrEmpty()) {
            return;
        }
        var query = from p in CurrentUser.Participations.Select((p => p.Poll)).Union(Context.Polls.Where(p => p.Creator == CurrentUser)) where
            p.Title.Contains(Filter) ||
            p.Participations.Select(u => u.User).Any(u => u.FullName.Contains(Filter)) ||
            p.Choices.Any(c => c.ChoiceText.Contains(Filter)) ||
            p.Creator.FullName.Contains(Filter) select p;

        Polls = new ObservableCollection<Poll>(query);
    }
}
