using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    public static class AssetBundleSimulateBuilder
    {

        /// <summary>
        /// 模拟构建
        /// </summary>
        public static Alter.PackageInvokeBuildResult SimulateBuild(string packageName)
        {
            var buildParameters = new EditorSimulateBuildParameters();
            buildParameters.BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
            buildParameters.BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
            buildParameters.BuildPipeline = EBuildPipeline.EditorSimulateBuildPipeline.ToString();
            buildParameters.BuildBundleType = (int)Alter.EBuildBundleType.VirtualBundle;
            buildParameters.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            buildParameters.PackageName = packageName;
            buildParameters.PackageVersion = "Simulate";
            buildParameters.FileNameStyle = Alter.EFileNameStyle.HashName;
            buildParameters.BuildinFileCopyOption = EBuildinFileCopyOption.None;
            buildParameters.BuildinFileCopyParams = string.Empty;
            buildParameters.UseAssetDependencyDB = true;

            var pipeline = new EditorSimulateBuildPipeline();
            BuildResult buildResult = pipeline.Run(buildParameters, false);
            if (buildResult.Success)
            {
                var reulst = new Alter.PackageInvokeBuildResult();
                reulst.PackageRootDirectory = buildResult.OutputPackageDirectory;
                return reulst;
            }
            else
            {
                Debug.LogError(buildResult.ErrorInfo);
                throw new System.Exception($"{nameof(EditorSimulateBuildPipeline)} build failed !");
            }
        }
    }
}