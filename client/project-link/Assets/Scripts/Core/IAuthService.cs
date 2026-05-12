using System;

namespace ProjectLink.Core
{
    public interface IAuthService
    {
        bool HasStoredSession { get; }
        string Provider { get; }
        void EnsureAuth(Action<bool, string> onComplete);
        void LoginGuest(Action<bool, string> onComplete);
        void LoginGoogle(string idToken, string nonce, Action<bool, string> onComplete);
        void Refresh(Action<bool, string> onComplete);
        void Logout(Action<bool, string> onComplete);
        string GetToken();
        void SetToken(string token);
        void ClearToken();
    }
}
