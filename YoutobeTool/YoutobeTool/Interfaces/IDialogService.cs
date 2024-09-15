using System.Threading.Tasks;

namespace YoutobeTool.Interfaces
{
    public interface IDialogService
    {
        Task ShowMessageAsync(string message);
        Task<bool> ShowConfirmationAsync(string message);
    }
}
