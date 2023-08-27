using AssetsTools;

namespace FF6PR2MSU;

public class BundleFileExtractor
{
    private readonly AssetsManager assetsManager;
    private readonly AssetsFileInstance assetInst;
    private readonly List<AssetFileInfoEx> assets;

    public BundleFileExtractor(string filePath)
    {
        assetsManager = new AssetsManager();

        BundleFileInstance bundle = assetsManager.LoadBundleFile(filePath);
        assetInst = assetsManager.LoadAssetsFileFromBundle(bundle, 0, true);

        assets = assetInst.table.GetAssetsOfType(AssetClassID.TextAsset);

        if (assets.Count <= 0)
        {
            throw new Exception();
        }
    }

    public bool GetAsset(int index, out string assetName, out byte[] assetData)
    {
        assetName = null;
        assetData = null;

        if (index < 0 || index >= assets.Count)
        {
            return false;
        }

        var baseField = assetsManager.GetTypeInstance(assetInst, assets[index]).GetBaseField();

        assetName = baseField.Get(0).Value.AsString();
        assetData = baseField.Get(1).Value.AsStringBytes();
        return true;
    }
}