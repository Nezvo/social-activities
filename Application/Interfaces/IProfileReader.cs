using System.Threading.Tasks;
using Application.Profiles;

namespace Application.Interfaces
{
    public interface IProfileReader
    {
        Task<Profile> ReadProfile(string userName);
    }
}