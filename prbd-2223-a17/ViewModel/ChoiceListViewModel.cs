namespace MyPoll.ViewModel;
using MyPoll.Model;
using PRBD_Framework;


public class ChoiceListViewModel : ViewModelCommon {

    private Poll _poll;

    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private Choice _choice;

    public Choice Choice {
        get => _choice;
        set => SetProperty(ref _choice, value);
    }

    public ChoiceListViewModel(Poll poll, Choice choice) {
        Poll = poll;
        Choice = choice;
    }



}
