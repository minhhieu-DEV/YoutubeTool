using System.Threading;
using System.Threading.Tasks;
using YoutobeTool.Models;

namespace YoutobeTool.Interfaces
{
    public interface IChatGPTService
    {
        Task<bool> IsCheckApiChatGPT();
        Task CreatePromtFromFileText(ImageModel imageModel, SemaphoreSlim semaphore);
    }
}
