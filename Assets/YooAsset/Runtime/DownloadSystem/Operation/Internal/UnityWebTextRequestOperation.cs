using UnityEngine.Networking;
using UnityEngine;

namespace YooAsset
{
    internal class UnityWebTextRequestOperation : UnityWebRequestOperation
    {
        protected enum ESteps
        {
            None,
            CreateRequest,
            Download,
            Done,
        }

        private UnityWebRequestAsyncOperation _requestOperation;
        private ESteps _steps = ESteps.None;

        /// <summary>
        /// 请求结果
        /// </summary>
        public string Result { private set; get; }


        internal UnityWebTextRequestOperation(string url, int timeout = 60) : base(url, timeout)
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
                    CheckRequestTimeout();
                    return;
                }

                if (CheckRequestResult())
                {
                    var fileText = _webRequest.downloadHandler.text;
                    if (string.IsNullOrEmpty(fileText))
                    {
                        _steps = ESteps.Done;
                        Status = EOperationStatus.Failed;
                        Error = $"URL : {_requestURL} Download handler text is null or empty !";
                    }
                    else
                    {
                        _steps = ESteps.Done;
                        Result = fileText;
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