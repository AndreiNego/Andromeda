using Andromeda.Content;

namespace Andromeda.Editors
{
     interface IAssetEditor
    {
        Asset Asset { get; }

        void SetAsset(Asset asset);
    }
}