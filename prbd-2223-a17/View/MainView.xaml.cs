using System.Windows;
using System.Windows.Controls;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;


namespace MyPoll.View;

public partial class MainView : WindowBase {

    public MainView() {
        InitializeComponent();

        Register<Poll>(App.Messages.MSG_DISPLAY_POLL,
            poll => DoDisplayPoll(poll));

        Register(App.Messages.MSG_NEW_POLL, () => {
                Poll poll = new Poll { Title = "", Creator = App.CurrentUser };
                OpenTab("New Poll", "New Poll", () => new AddPollView(new PollDetailViewModel(poll)));
            }
        );

        Register<Poll>(App.Messages.MSG_CLOSE_EDIT_TAB,
            poll => CloseTab(poll.PollId.ToString()));

        Register(App.Messages.MSG_CLOSE_NEW_POLL_TAB, () => {
            CloseTab("New Poll");
        });

        Register<Poll>(App.Messages.MSG_DELETE_POLL,
            poll => CloseTab(poll.PollId.ToString()));
    }

    private void DoDisplayPoll(Poll poll) {
        if (poll != null)
            OpenTab(poll.Title, poll.PollId.ToString(), () => new PollDetailView(poll));
    }

    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null)
            tabControl.Add(createView(), header, tag);
        else
            tabControl.SetFocus(tab);
    }

    private void CloseTab(string tab) {
        tabControl.CloseByTag(tab);
    }
}
