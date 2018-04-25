using System;
using System.Threading.Tasks;

namespace Reviewer.Core
{
    public interface IVideoConversion
    {
        Task<string> ConvertToMP4(string videoLocation);
    }
}
