using PRBD_Framework;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MyPoll.ViewModel;
public class LoginViewModel : ViewModelCommon {
    public ICommand LoginCommand { get; set; }
    public ICommand LoginAsDefaultCommand { get; set; }
    public ICommand SignupCommand { get; set; }

    private string _email;
    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    private string _password;
    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }

    public LoginViewModel() {
        LoginCommand = new RelayCommand(LoginAction,
            () => { return _email != null && _password != null && !HasErrors; });
        LoginAsDefaultCommand = new RelayCommand<string>(LoginAsDefaultAction);
        SignupCommand = new RelayCommand(SignupAction);
    }

    private void LoginAction() {
        if (Validate()) {
            var user = Context.Users.FirstOrDefault(u => u.Email == Email);
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
    }

    private void LoginAsDefaultAction(string email) {
        var user = Context.Users.FirstOrDefault(u => u.Email == email);
        NotifyColleagues(App.Messages.MSG_LOGIN, user);
    }

    private void SignupAction() {
        NotifyColleagues(App.Messages.MSG_SIGNUP);
    }

    public override bool Validate() {
        ClearErrors();
        var user = Context.Users.FirstOrDefault(u => u.Email == Email);

        if (string.IsNullOrEmpty(Email)) {
            AddError(nameof(Email), "This field is required");
        }
        else if (!(Regex.IsMatch(Email, @"^[a-zA-Z0-9]{1,20}@([a-zA-Z0-9]{1,15}\.)+[a-z]{1,7}$"))) {
            AddError(nameof(Email), "This is not a valid Email adress");
        }
        else if (user == null) {
            AddError(nameof(Email), "User does not exist");
        }
        else {
            if (string.IsNullOrEmpty(Password)) {
                AddError(nameof(Password), "This field is required");
            }
            else if (!SecretHasher.Verify(Password, user.Password)) {
                AddError(nameof(Password), "wrong password");
            }
        }
        return !HasErrors;
    }
}

