using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Start()
    {
        // 设置为 4:3 分辨率
        int width = 1400;
        int height = 1050;

        // 设置窗口模式为窗口化且不能调整大小
        Screen.SetResolution(width, height, false); // false 表示窗口模式
    }
}
