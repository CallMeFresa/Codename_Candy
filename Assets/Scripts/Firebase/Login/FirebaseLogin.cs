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
    [SerializeField] private TMP_InputField login_password;

    [Header("Forgot password")]
    [SerializeField] private TMP_InputField forgot_email;

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

    private FirebaseAuth auth;

    private void Awake()
    {
        panel_start.SetActive(true);
        panel_register.SetActive(false);
        panel_login.SetActive(false);
        panel_forgotPassword.SetActive(false);
    }

    private void Start()
    {
        FirebaseApp.Create(config.GetConfig());
        config.ClearUserInfo();
    }

    #region Login

    #endregion

    #region Forgot password

    #endregion

    #region Register
    public void Register_SetActive()
    {
        panel_register.SetActive(true);

        register_emailMessage.SetActive(false);
        register_passwordMessage.SetActive(false);
        register_password1.contentType = TMP_InputField.ContentType.Password;
        register_password2.contentType = TMP_InputField.ContentType.Password;

        Register_CheckAll();
    }

    public void Register_CheckEmail(string email)
    {
        if (email.Contains("@") && email.Contains(".") && email.Length > 4)
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
            register_btnCreate.enabled = true;
        }
        else
        {
            register_btnCreate.enabled = false;
        }
    }

    public void Register_CreateAccount()
    {
        PopUpManager.Instance.OpenPopUp("Creando usuario...");

        CreateUserWithEmailAndPasswordAsync(register_email.text, register_password1.text, () =>
        {
            PopUpManager.Instance.OpenPopUp("Usuario Creado!", 2);

            panel_register.SetActive(false);
            panel_updateName.SetActive(true);
        }).Forget();
    }
    #endregion

    #region Commond Functions
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
    public async UniTask SignInWithEmailAndPasswordAsync(string email, string password)
    {
        auth = FirebaseAuth.DefaultInstance;

        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            config.SetUserInfo(result.User);
            Debug.LogFormat($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
        }
        catch (FirebaseException e)
        {
            PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

            Debug.LogError((AuthError)e.ErrorCode);
            Debug.LogError(e.Message);
        }
    }

    [ContextMenu("Update User Profile")]
    public async UniTask UpdateUserProfileAsync(string name)
    {
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            UserProfile profile = new()
            {
                DisplayName = name,
                //PhotoUrl = new System.Uri("https://example.com/jane-q-user/profile.jpg")
            };

            try
            {
                await user.UpdateUserProfileAsync(profile);

                Debug.Log("User profile updated successfully.");
                config.SetUserInfo(auth.CurrentUser);
            }
            catch (FirebaseException e)
            {
                PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

                Debug.LogError((AuthError)e.ErrorCode);
                Debug.LogError(e.Message);
            }
        }
    }

    [ContextMenu("Send Email Verification")]
    public async UniTask SendEmailVerificationAsync()
    {
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                await user.SendEmailVerificationAsync();

                Debug.Log("Email sent successfully.");
            }
            catch (FirebaseException e)
            {
                PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

                Debug.LogError((AuthError)e.ErrorCode);
                Debug.LogError(e.Message);
            }
        }
    }

    [ContextMenu("Update Password")]
    public async UniTask UpdatePasswordAsync(string password)
    {
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                await user.UpdatePasswordAsync(password);

                Debug.Log("Password updated successfully.");
            }
            catch (FirebaseException e)
            {
                PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

                Debug.LogError((AuthError)e.ErrorCode);
                Debug.LogError(e.Message);
            }
        }
    }

    [ContextMenu("Send Password Reset Email")]
    public async UniTask SendPasswordResetEmailAsync(string email)
    {
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                await auth.SendPasswordResetEmailAsync(email);

                Debug.Log("Password reset email sent successfully.");
            }
            catch (FirebaseException e)
            {
                PopUpManager.Instance.OpenPopUp(AuthErrorMessages.GetMessage(e.ErrorCode), 3);

                Debug.LogError((AuthError)e.ErrorCode);
                Debug.LogError(e.Message);
            }
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