
namespace YooAsset.Alter
{
    internal abstract class FSLoadSceneOperation : AsyncOperationBase
    {
        public UnityEngine.SceneManagement.Scene Result;

        public abstract void UnSuspendLoad();
    }
}