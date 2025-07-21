using UnityEngine;

namespace YooAsset.Alter
{
    internal abstract class DownloadAssetBundleOperation : DefaultDownloadFileOperation
    {
        internal DownloadAssetBundleOperation(PackageBundle bundle, DownloadFileOptions options) : base(bundle, options)
        {
        }

        public AssetBundle Result;
    }
}