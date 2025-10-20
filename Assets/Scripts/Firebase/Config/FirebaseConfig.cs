using Firebase;
using Firebase.Auth;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Firebase Config", menuName = "Scriptable Objects/Firebase/Config", order = 0)]
public class FirebaseConfig : ScriptableObject
{
    [Header("Config")]
    public Uri databaseUrl;
    public string appId;
    public string apiKey;
    public string messageSenderId;
    public string storageBucket;
    public string projectId;

    [Header("User")]
    public string DisplayName;
    public string Email;
    public bool IsAnonymous;
    public bool IsEmailVerified;
    public string PhoneNumber;
    public Uri PhotoUrl;
    public string ProviderId;
    public string UserId;

    public AppOptions GetConfig()
    {
        return new AppOptions
        {
            DatabaseUrl = databaseUrl,
            AppId = appId,
            ApiKey = apiKey,
            MessageSenderId = messageSenderId,
            StorageBucket = storageBucket,
            ProjectId = projectId
        };
    }

    public void ClearUserInfo()
    {
        DisplayName = string.Empty;
        Email = string.Empty;
        IsAnonymous = false;
        IsEmailVerified = false;
        PhoneNumber = string.Empty;
        PhotoUrl = null;
        ProviderId = string.Empty;
        UserId = string.Empty;
    }

    public void SetUserInfo(FirebaseUser user)
    {
        DisplayName = user.DisplayName;
        Email = user.Email;
        IsAnonymous = user.IsAnonymous;
        IsEmailVerified = user.IsEmailVerified;
        PhoneNumber = user.PhoneNumber;
        PhotoUrl = user.PhotoUrl;
        ProviderId = user.ProviderId;
        UserId = user.UserId;
    }
}