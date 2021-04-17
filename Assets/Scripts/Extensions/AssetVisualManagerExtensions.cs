public static class AssetVisualManagerExtensions
{
    public static void LoadFromCardAsset<TAsset>(this IAssetVisualManager<TAsset> cardAssetVisual, TAsset asset)
    {
        if (cardAssetVisual == null || asset == null)
            return;

        cardAssetVisual.Asset = asset;
        cardAssetVisual.ApplyLookFromAsset();
    }
}
