
namespace YooAsset
{
    internal class DWRFSLoadAssetBundleOperation : FSLoadBundleOperation
    {
        private enum ESteps
        {
            None,
            DownloadAssetBundle,
            Done,
        }

        private readonly DefaultWebRemoteFileSystem _fileSystem;
        private readonly PackageBundle _bundle;
        private DownloadAssetBundleOperation _downloadAssetBundleOp;
        private ESteps _steps = ESteps.None;


        internal DWRFSLoadAssetBundleOperation(DefaultWebRemoteFileSystem fileSystem, PackageBundle bundle)
        {
            _fileSystem = fileSystem;
            _bundle = bundle;
        }
        internal override void InternalStart()
        {
            _steps = ESteps.DownloadAssetBundle;
        }
        internal override void InternalUpdate()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.DownloadAssetBundle)
            {
                if (_downloadAssetBundleOp == null)
                {
                    string mainURL = _fileSystem.RemoteServices.GetRemoteMainURL(_bundle.FileName);
                    string fallbackURL = _fileSystem.RemoteServices.GetRemoteFallbackURL(_bundle.FileName);
                    DownloadFileOptions options = new DownloadFileOptions(int.MaxValue, 60);
                    options.SetURL(mainURL, fallbackURL);
                    
                    if (_bundle.Encrypted)
                    {
                        _downloadAssetBundleOp = new DownloadEncryptAssetBundleOperation(_bundle, options, true, _fileSystem.DecryptionServices);
                        _downloadAssetBundleOp.StartOperation();
                        AddChildOperation(_downloadAssetBundleOp);
                    }
                    else
                    {
                        _downloadAssetBundleOp = new DownloadNormalAssetBundleOperation(_bundle, options, _fileSystem.DisableUnityWebCache);
                        _downloadAssetBundleOp.StartOperation();
                        AddChildOperation(_downloadAssetBundleOp);
                    }
                }

                _downloadAssetBundleOp.UpdateOperation();
                DownloadProgress = _downloadAssetBundleOp.DownloadProgress;
                DownloadedBytes = _downloadAssetBundleOp.DownloadedBytes;
                Progress = _downloadAssetBundleOp.Progress;
                if (_downloadAssetBundleOp.IsDone == false)
                    return;

                if (_downloadAssetBundleOp.Status == EOperationStatus.Succeed)
                {
                    var assetBundle = _downloadAssetBundleOp.Result;
                    if (assetBundle == null)
                    {
                        _steps = ESteps.Done;
                        Status = EOperationStatus.Failed;
                        Error = $"{nameof(DownloadAssetBundleOperation)} loaded asset bundle is null !";
                    }
                    else
                    {
                        _steps = ESteps.Done;
                        Result = new AssetBundleResult(_fileSystem, _bundle, assetBundle, null);
                        Status = EOperationStatus.Succeed;
                    }
                }
                else
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = _downloadAssetBundleOp.Error;
                }
            }
        }
        internal override void InternalWaitForAsyncComplete()
        {
            if (_steps != ESteps.Done)
            {
                _steps = ESteps.Done;
                Status = EOperationStatus.Failed;
                Error = "WebGL platform not support sync load method !";
                UnityEngine.Debug.LogError(Error);
            }
        }
    }
}