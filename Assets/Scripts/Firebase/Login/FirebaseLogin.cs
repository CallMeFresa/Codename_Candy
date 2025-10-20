using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

public class FirebaseLogin : MonoBehaviour
{
    [SerializeField] private FirebaseConfig config;

    [Space]
    [SerializeField] private string email;
    [SerializeField] private string password;
    [SerializeField] private string newName;

    private Firebase.Auth.FirebaseAuth auth;

    private void Start()
    {
        FirebaseApp.Create(config.GetConfig());
        config.ClearUserInfo();
    }

    [ContextMenu("Create User With Email And Password")]
    public async UniTask CreateUserWithEmailAndPasswordAsync()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        try
        {
            Firebase.Auth.AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            config.SetUserInfo(result.User);
            Debug.LogFormat($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CreateUserWithEmailAndPasswordAsync encountered an error: {ex}");
        }
    }

    [ContextMenu("SignIn With Email And Password")]
    public async UniTask SignInWithEmailAndPasswordAsync()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        try
        {
            Firebase.Auth.AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            config.SetUserInfo(result.User);
            Debug.LogFormat($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SignInWithEmailAndPasswordAsync encountered an error: {ex}");
            throw;
        }
    }

    [ContextMenu("Update User Profile")]
    public async UniTask UpdateUserProfileAsync()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new()
            {
                DisplayName = newName,
                //PhotoUrl = new System.Uri("https://example.com/jane-q-user/profile.jpg")
            };

            try
            {
                await user.UpdateUserProfileAsync(profile);

                Debug.Log("User profile updated successfully.");
                config.SetUserInfo(auth.CurrentUser);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"UpdateUserProfileAsync encountered an error: {ex}");
            }
        }
    }

    [ContextMenu("Send Email Verification")]
    public async UniTask SendEmailVerificationAsync()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                await user.SendEmailVerificationAsync();

                Debug.Log("Email sent successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SendEmailVerificationAsync encountered an error: {ex}");
            }
        }
    }

    [ContextMenu("Update Password")]
    public async UniTask UpdatePasswordAsync()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                await user.UpdatePasswordAsync(password);

                Debug.Log("Password updated successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"UpdatePasswordAsync encountered an error: {ex}");
            }
        }
    }

    [ContextMenu("Send Password Reset Email")]
    public async UniTask SendPasswordResetEmailAsync()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            try
            {
                await auth.SendPasswordResetEmailAsync(email);

                Debug.Log("Password reset email sent successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SendPasswordResetEmailAsync encountered an error: {ex}");
            }
        }
    }

    [ContextMenu("Sing Out")]
    public void SingOut()
    {
        auth.SignOut();
        Debug.Log("SingOut");
    }
}