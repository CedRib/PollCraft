using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Navigation;
using MyPoll.Model;
using MyPoll.View;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class SignupViewModel : ViewModelCommon {

    public ICommand CancelCommand { get; set; }
    public ICommand SignupCommand { get; set; }

    public SignupViewModel() {
        CancelCommand = new RelayCommand(CancelBtnAction);
        SignupCommand = new RelayCommand(SignupAction);
    }

    private string _email;
    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    private string _fullname;
    public string FullName {
        get => _fullname;
        set => SetProperty(ref _fullname, value, () => Validate());
    }

    private string _password;
    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }

    private string _confirmpassword;
    public string ConfirmPassword {
        get => _confirmpassword;
        set => SetProperty(ref _confirmpassword, value, () => Validate());
    }

    private static void CancelBtnAction() {
        NotifyColleagues(App.Messages.MSG_FIRSTWINDOW);
    }

    private void SignupAction() {
        if (Validate()) {
            User user = new User { Email = Email, FullName = FullName, Password = SecretHasher.Hash(Password) };
            Context.Users.Add(user);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_FIRSTWINDOW);
        }
    }

    public override bool Validate() {
        ClearErrors();

        if (string.IsNullOrEmpty(Email)) {
            AddError(nameof(Email), "This field is required");
        }
        else if (!(Regex.IsMatch(Email, @"^[a-zA-Z0-9]{1,20}@([a-zA-Z0-9]{1,15}\.)+[a-z]{1,7}$"))) {
            AddError(nameof(Email), "This is not a valid Email adress");
        }
        if (User.AllAppEmail.Contains(Email)) {
            AddError(nameof(Email), "This Email already exists");
        }
        if (string.IsNullOrEmpty(FullName)) {
            AddError(nameof(FullName), "This field is required");
        }
        if (string.IsNullOrEmpty(Password)) {
            AddError(nameof(Password), "This field is required");
        }
        else if (string.IsNullOrEmpty(ConfirmPassword)) {
            AddError(nameof(ConfirmPassword), "This field is required");
        }
        else if (Password != ConfirmPassword) {
            AddError(nameof(ConfirmPassword), "Passwords must be the same");
        }
        return !HasErrors;
    }




}
