public interface IAssetVisualManager<TAsset>
{
    TAsset Asset { get; set; }

    void ApplyLookFromAsset();
}