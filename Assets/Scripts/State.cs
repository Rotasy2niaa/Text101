using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText; // �����ı�
    [TextArea(3, 3)][SerializeField] string[] options; // ѡ���ı�
    [SerializeField] State[] nextStates; // ��һ��״̬
    [SerializeField] bool hasMusicChoice = false; // �Ƿ�������ѡ��
    [SerializeField] AudioClip[] musicChoices; // ����ѡ������

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

    public bool HasMusicChoice()
    {
        return hasMusicChoice; // �����Ƿ�������ѡ��
    }

    public AudioClip GetSelectedMusic(int index)
    {
        if (hasMusicChoice && index >= 0 && index < musicChoices.Length)
        {
            return musicChoices[index]; // ���ض�Ӧ������
        }
        return null; // û�����ֻ��������󣬷��� null
    }
}
