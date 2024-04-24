using System.Threading.Tasks;

namespace GeorgaMobileClient.Interface
{
    public interface IWebView : IView
    {
        void LoadUri(string url);
    }
}