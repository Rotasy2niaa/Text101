using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText; // 故事文本
    [TextArea(3, 3)][SerializeField] string[] options; // 选项文本
    [SerializeField] State[] nextStates; // 下一个状态
    [SerializeField] bool hasMusicChoice = false; // 是否有音乐选择
    [SerializeField] AudioClip[] musicChoices; // 音乐选择数组

    public string GetStateStory()
    {
        return storyText; // 返回故事文本
    }

    public string[] GetOptions()
    {
        return options; // 返回选项文本
    }

    public State[] GetNextStates()
    {
        return nextStates; // 返回下一个状态
    }

    public bool HasMusicChoice()
    {
        return hasMusicChoice; // 返回是否有音乐选择
    }

    public AudioClip GetSelectedMusic(int index)
    {
        if (hasMusicChoice && index >= 0 && index < musicChoices.Length)
        {
            return musicChoices[index]; // 返回对应的音乐
        }
        return null; // 没有音乐或索引错误，返回 null
    }
}
