using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText; // 故事文本
    [TextArea(3, 3)][SerializeField] string[] options; // 选项文本
    [SerializeField] State[] nextStates; // 下一个状态
    [SerializeField] Sprite stateImage; // 状态图片

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

    public Sprite GetStateImage()
    {
        return stateImage; // 返回状态图片
    }
}
