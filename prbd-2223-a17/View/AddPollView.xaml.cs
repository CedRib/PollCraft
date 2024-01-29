using System.Windows.Controls;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll.View;

public partial class AddPollView : UserControlBase {

    public AddPollView() {
        InitializeComponent();
    }

    public AddPollView(PollDetailViewModel pdvm) {
        InitializeComponent();
        DataContext = new AddPollViewModel(pdvm);
    }
}

