using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText; // �����ı�
    [TextArea(3, 3)][SerializeField] string[] options; // ѡ���ı�
    [SerializeField] State[] nextStates; // ��һ��״̬
    [SerializeField] Sprite stateImage; // ״̬ͼƬ

    public string GetStateStory()
    {
        return storyText; // ���ع����ı�
    }

    public string[] GetOptions()
    {
        return options; // ����ѡ���ı�
    }

    public State[] GetNextStates()
    {
        return nextStates; // ������һ��״̬
    }

    public Sprite GetStateImage()
    {
        return stateImage; // ����״̬ͼƬ
    }
}
