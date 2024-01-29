using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;


namespace MyPoll.View;

public partial class PollDetailView : UserControlBase {

    public PollDetailView(Poll poll) {
        InitializeComponent();
        DataContext = new PollDetailViewModel(poll);
    }

}

