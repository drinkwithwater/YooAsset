using UnityEngine.Networking;
using UnityEngine;

namespace YooAsset
{
    internal class UnityWebDataRequestOperation : UnityWebRequestOperation
    {
        protected enum ESteps
        {
            None,
            CreateRequest,
            Download,
            Done,
        }

        private UnityWebRequestAsyncOperation _requestOperation;
        private bool _checkTimeout = true;
        private ESteps _steps = ESteps.None;

        /// <summary>
        /// 请求结果
        /// </summary>
        public byte[] Result { private set; get; }


        internal UnityWebDataRequestOperation(string url, int timeout = 60) : base(url, timeout)
        {
        }
        internal override void InternalStart()
        {
            _steps = ESteps.CreateRequest;
        }
        internal override void InternalUpdate()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.CreateRequest)
            {
                ResetTimeout();
                CreateWebRequest();
                _steps = ESteps.Download;
            }

            if (_steps == ESteps.Download)
            {
                DownloadProgress = _webRequest.downloadProgress;
                DownloadedBytes = (long)_webRequest.downloadedBytes;
                Progress = _requestOperation.progress;
                if (_requestOperation.isDone == false)
                {
                    if (_checkTimeout)
                        CheckRequestTimeout();
                    return;
                }

                if (CheckRequestResult())
                {
                    var fileData = _webRequest.downloadHandler.data;
                    if (fileData == null || fileData.Length == 0)
                    {
                        _steps = ESteps.Done;
                        Status = EOperationStatus.Failed;
                        Error = $"URL : {_requestURL} Download handler data is null or empty !";
                    }
                    else
                    {
                        _steps = ESteps.Done;
                        Result = fileData;
                        Status = EOperationStatus.Succeed;
                    }
                }
                else
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                }

                // 注意：最终释放请求器
                DisposeRequest();
            }
        }

        /// <summary>
        /// 不检测超时
        /// </summary>
        public void DontCheckTimeout()
        {
            _checkTimeout = false;
        }

        private void CreateWebRequest()
        {
            _webRequest = DownloadSystemHelper.NewUnityWebRequestGet(_requestURL);
            DownloadHandlerBuffer handler = new DownloadHandlerBuffer();
            _webRequest.downloadHandler = handler;
            _webRequest.disposeDownloadHandlerOnDispose = true;
            _requestOperation = _webRequest.SendWebRequest();
        }
    }
}