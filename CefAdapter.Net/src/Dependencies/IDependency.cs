
namespace CefAdapter.Dependencies
{
    interface IDependency    
    {
        string Name { get; }

        bool IsInstalled();

        bool IsDownloaded();

        void Download();

        void Install();
    }
}