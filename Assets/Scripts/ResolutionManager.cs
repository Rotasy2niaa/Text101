using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Start()
    {
        // ����Ϊ 4:3 �ֱ���
        int width = 800;
        int height = 600;

        // ���ô���ģʽΪ���ڻ��Ҳ��ܵ�����С
        Screen.SetResolution(width, height, false); // false ��ʾ����ģʽ
    }
}
