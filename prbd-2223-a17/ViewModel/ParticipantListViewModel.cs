using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class ParticipantListViewModel : ViewModelCommon {

    private int _voteCount;
    public int VoteCount {
        get => _voteCount;
        set => SetProperty(ref _voteCount, value);
    }

    private User _user;
    public User User {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    public ParticipantListViewModel(User user, Poll poll) {
        User = user;
        Poll = poll;
        OnRefreshData();
    }

    protected sealed override void OnRefreshData() {
        VoteCount = User.GetVoteCountForPoll(Poll.PollId);
    }
}
