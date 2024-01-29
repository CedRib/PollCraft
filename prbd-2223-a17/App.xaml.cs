using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll;

public partial class App : ApplicationBase<User, MyPollContext> {

    public enum Messages {
        MSG_LOGIN,
        MSG_DISPLAY_POLL,
        MSG_SIGNUP,
        MSG_FIRSTWINDOW,
        MSG_LOGOUT,
        MSG_NEW_POLL,
        MSG_CLOSE_EDIT_TAB,
        MSG_CLOSE_NEW_POLL_TAB,
        MSG_DELETE_POLL
    }
    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();

        Register<User>(this, Messages.MSG_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });
        Register(this, Messages.MSG_SIGNUP, () => {
            NavigateTo<SignupViewModel, User,  MyPollContext>();
        });
        Register(this, Messages.MSG_FIRSTWINDOW, () => {
            NavigateTo<LoginViewModel, User, MyPollContext>();
        });
        Register(this, Messages.MSG_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, MyPollContext>();
        } );
    }

    protected sealed override void OnRefreshData() {
        CurrentUser = Context.Users.Find(CurrentUser.UserId);
    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Cold start
        Console.Write("Cold starting database... ");
        //Context.Users.Find(0);
        Console.WriteLine("done");
    }
}
