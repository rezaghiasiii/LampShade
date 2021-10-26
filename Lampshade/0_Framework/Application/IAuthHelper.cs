namespace _0_Framework.Application
{
    public interface IAuthHelper
    {
        AuthViewModel CurrentAccountInfo();
        string CurrentAccountRole();
        bool IsAuthenticated();
        void Signin(AuthViewModel account);
        void SignOut();
    }
}