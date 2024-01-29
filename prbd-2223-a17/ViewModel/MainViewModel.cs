using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class MainViewModel : ViewModelCommon {


    public ICommand LogoutCommand { get; set; }
    public string AppTitle => "MyPoll Application" + " (" + App.CurrentUser.FullName +")";

    public MainViewModel() {
        LogoutCommand = new RelayCommand(LogoutAction);
    }

    private void LogoutAction() {
        NotifyColleagues(App.Messages.MSG_LOGOUT);
    }
}

