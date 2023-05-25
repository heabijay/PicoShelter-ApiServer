using System.Threading.Tasks;

namespace PicoShelter_ApiServer.BLL.Interfaces
{
    public interface ICommentNotifier
    {
        Task OnCommentAddedAsync(string imageCode);

        Task OnCommentDeletedAsync(string imageCode, int commentId);
    }
}