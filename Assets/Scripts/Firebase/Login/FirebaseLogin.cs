using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseLogin : MonoBehaviour
{
    [SerializeField] private FirebaseConfig config;

    [Header("Panels")]
    [SerializeField] private GameObject panel_start;
    [SerializeField] private GameObject panel_login;
    [SerializeField] private GameObject panel_forgotPassword;
    [SerializeField] private GameObject panel_register;
    [SerializeField] private GameObject panel_updateName;


    [Header("Login")]
    [SerializeField] private TMP_InputField login_email;
    [SerializeField] private GameObject login_emailMessage;
    private bool login_emailCheck = false;

    [SerializeField] private TMP_InputField login_password;
    private bool login_passwordCheck = false;

    [SerializeField] private Button login_btnLogin;


    [Header("Forgot password")]
    [SerializeField] private TMP_InputField forgotPassword_email;
    [SerializeField] private GameObject forgotPassword_emailMessage;

    [SerializeField] private Button forgotPassword_btnSend;


    [Header("Register")]
    [SerializeField] private TMP_InputField register_email;
    [SerializeField] private GameObject register_emailMessage;
    private bool register_emailCheck = false;

    [SerializeField] private TMP_InputField register_password1;
    [SerializeField] private TMP_InputField register_password2;
    [SerializeField] private GameObject register_passwordMessage;
    private bool register_passwordCheck = false;

    [SerializeField] private Button register_btnCreate;


    [Header("Update name")]
    [SerializeField] private TMP_InputField updateName_name;
    [SerializeField] private Button updateName_btnUpdate;

    private FirebaseAuth auth;

    private void Awake()
    {
        panel_start.SetActive(true);
        panel_register.SetActive(false);
        panel_login.SetActive(false);
        panel_forgotPassword.SetActive(false);
        panel_updateName.SetActive(false);
    }

    private void Start()
    {
        FirebaseApp.Create(config.GetConfig());
        config.ClearUserInfo();
    }

    #region Login
    public void Login_SetActive()
    {
        panel_login.SetActive(true);

        login_emailMessage.SetActive(false);
        login_password.contentType = TMP_InputField.ContentType.Password;

        Login_CheckEmail(login_email.text);
        Login_CheckPassword(login_password.text);
        Login_CheckAll();
    }

    public void Login_CheckEmail(string email)
    {
        if (CheckEmail(email))
        {
            login_emailCheck = true;
            login_emailMessage.SetActive(false);
        }
        else
        {
            login_emailCheck = false;
            login_emailMessage.SetActive(true);
        }
    }

    public void Login_CheckPassword(string password)
    {
        if (password.Length > 0)
        {
            login_passwordCheck = true;
        }
        else
        {
            login_passwordCheck = false;
        }
    }

    public void Login_ViewPassword(bool on)
    {
        if (on)
        {
            login_password.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            login_password.contentType = TMP_InputField.ContentType.Password;
        }

        login_password.enabled = false;
        login_password.enabled = true;
    }

    public void Login_CheckAll()
    {
        if (login_emailCheck && login_passwordCheck)
        {
            login_btnLogin.interactable = true;
        }
        else
        {
            login_btnLogin.interactable = false;
        }
    }

    public void Login_Login()
    {
        PopUpManager.Instance.OpenPopUp("Iniciando sesión...");

        SignInWithEmailAndPasswordAsync(login_email.text, login_password.text, () =>
        {
            PopUpManager.Instance.OpenPopUp("Sesion iniciada!", 2);

            panel_login.SetActive(false);
            Debug.LogWarning("All Ready!");
        }).Forget();
    }
    #endregion

    #region Forgot password
    public void ForgotPassword_SetActive()
    {
        panel_forgotPassword.SetActive(true);

        ForgotPassword_CheckEmail(forgotPassword_email.text);
    }

    public void ForgotPassword_CheckEmail(string email)
    {
        if (CheckEmail(email))
        {
            forgotPassword_emailMessage.SetActive(false);
            forgotPassword_btnSend.interactable = true;
        }
        else
        {
            forgotPassword_emailMessage.SetActive(true);
            forgotPassword_btnSend.interactable = false;
        }
    }

    public void ForgotPassword_Send()
    {
        PopUpManager.Instance.OpenPopUp("Enviando correo...");

        SendPasswordResetEmailAsync(forgotPassword_email.text, () =>
        {
            PopUpManager.Instance.OpenPopUp("Correo enviado!", 2);

            panel_forgotPassword.SetActive(false);
            Login_SetActive();
        }).Forget();
    }
    #endregion

    #region Register
    public void Register_SetActive()
    {
        panel_register.SetActive(true);

        register_emailMessage.SetActive(false);
        register_passwordMessage.SetActive(false);
        register_password1.contentType = TMP_InputField.ContentType.Password;
        register_password2.contentType = TMP_InputField.ContentType.Password;

        Register_CheckEmail(register_email.text);
        Register_CheckPasswords();
        Register_CheckAll();
    }

    public void Register_CheckEmail(string email)
    {
        if (CheckEmail(email))
        {
            register_emailCheck = true;
            register_emailMessage.SetActive(false);
        }
        else
        {
            register_emailCheck = false;
            register_emailMessage.SetActive(true);
        }
    }

    public void Register_CheckPasswords()
    {
        if (register_password1.text == register_password2.text)
        {
            register_passwordCheck = true;
            register_passwordMessage.SetActive(false);
        }
        else
        {
            register_passwordCheck = false;
            register_passwordMessage.SetActive(true);
        }
    }

    public void Register_ViewPassword1(bool on)
    {
        if (on)
        {
            register_password1.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            register_password1.contentType = TMP_InputField.ContentType.Password;
        }

        register_password1.enabled = false;
        register_password1.enabled = true;
    }

    public void Register_ViewPassword2(bool on)
    {
        if (on)
        {
            register_password2.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            register_password2.contentType = TMP_InputField.ContentType.Password;
        }

        register_password2.enabled = false;
        register_password2.enabled = true;
    }

    public void Register_CheckAll()
    {
        if (register_emailCheck && register_passwordCheck)
        {
            register_btnCreate.interactable = true;
        }
        else
        {
            register_btnCreate.interactable = false;
        }
    }

    public void Register_CreateAccount()
    {
        PopUpManager.Instance.OpenPopUp("Creando usuario...");

        CreateUserWithEmailAndPasswordAsync(register_email.text, register_password1.text, () =>
        {
            PopUpManager.Instance.OpenPopUp("Usuario Creado!", 2);

            panel_register.SetActive(false);
            UpdateName_SetActive();
        }).Forget();
    }
    #endregion

    #region Update name
    public void UpdateName_SetActive()
    {
        panel_updateName.SetActive(true);

        UpdateName_CheckName(updateName_name.text);
    }

    public void UpdateName_CheckName(string name)
    {
        if (name.Length > 0)
        {
            updateName_btnUpdate.interactable = true;
        }
        else
        {
            updateName_btnUpdate.interactable = false;
        }
    }

    public void UpdateName_Update()
    {
        PopUpManager.Instance.OpenPopUp("Actualizando nombre...");

        UpdateUserProfileAsync(updateName_name.text, () =>
        {
            PopUpManager.Instance.OpenPopUp("Datos actualizados!", 2);

            panel_updateName.SetActive(false);
            Debug.LogWarning("All Ready!");
        }).Forget();
    }
    #endregion

    #region Commond Functions
    private bool CheckEmail(string _email)
    {
        if (_email.Contains("@") && _email.Contains(".") && _email.Length > 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [ContextMenu("Create User With Email And Password")]
    public async UniTaskVoid CreateUserWithEmailAndPasswordAsync(string email, string password, Action OnComplete = null)
    {
        auth = FirebaseAuth.DefaultInstance;

        try
        {
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            config.SetUserInfo(result.User);

            OnComplete?.Invoke();
        }
        catch (FirebaseException e)
        {
            PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

            Debug.LogError((AuthError)e.ErrorCode);
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("SignIn With Email And Password")]
    public async UniTask SignInWithEmailAndPasswordAsync(string email, string password, Action OnComplete = null)
    {
        auth = FirebaseAuth.DefaultInstance;

        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            config.SetUserInfo(result.User);

            OnComplete?.Invoke();
        }
        catch (FirebaseException e)
        {
            PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

            Debug.LogError((AuthError)e.ErrorCode);
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("Update User Profile")]
    public async UniTask UpdateUserProfileAsync(string name, Action OnComplete = null)
    {
        FirebaseUser user = auth.CurrentUser;

        UserProfile profile = new()
        {
            DisplayName = name,
            //PhotoUrl = new System.Uri("https://example.com/jane-q-user/profile.jpg")
        };

        try
        {
            await user.UpdateUserProfileAsync(profile);

            config.SetUserInfo(auth.CurrentUser);

            OnComplete?.Invoke();
        }
        catch (FirebaseException e)
        {
            PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

            Debug.LogError((AuthError)e.ErrorCode);
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("Send Email Verification")]
    public async UniTask SendEmailVerificationAsync(Action OnComplete = null)
    {
        FirebaseUser user = auth.CurrentUser;

        try
        {
            await user.SendEmailVerificationAsync();

            OnComplete?.Invoke();
        }
        catch (FirebaseException e)
        {
            PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

            Debug.LogError((AuthError)e.ErrorCode);
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("Update Password")]
    public async UniTask UpdatePasswordAsync(string password, Action OnComplete = null)
    {
        FirebaseUser user = auth.CurrentUser;

        try
        {
            await user.UpdatePasswordAsync(password);

            OnComplete?.Invoke();
        }
        catch (FirebaseException e)
        {
            PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

            Debug.LogError((AuthError)e.ErrorCode);
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("Send Password Reset Email")]
    public async UniTask SendPasswordResetEmailAsync(string email, Action OnComplete = null)
    {
        auth = FirebaseAuth.DefaultInstance;

        try
        {
            await auth.SendPasswordResetEmailAsync(email);

            OnComplete?.Invoke();
        }
        catch (FirebaseException e)
        {
            PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

            Debug.LogError((AuthError)e.ErrorCode);
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("Sing Out")]
    public void SingOut()
    {
        auth.SignOut();
        Debug.Log("SingOut");
    }
    #endregion
}