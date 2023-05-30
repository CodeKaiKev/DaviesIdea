using Microsoft.AspNetCore.Mvc;

namespace DaviesIdeas.Services.UserService
{
    public interface IUserService
    {
        string GetMyName();
        string GetMyRole();
        string GetMyEmail();
        string GetMyId();
    }
}
