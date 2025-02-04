using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText; // 故事文本
    [TextArea(3, 3)][SerializeField] string[] options; // 选项文本
    [SerializeField] State[] nextStates; // 下一个状态
    [SerializeField] Sprite stateImage; // 状态图片
    [SerializeField] bool hasMusicChoice = false; // 是否有音乐选项
    [SerializeField] AudioClip[] musicChoices; // 音乐选项
    [SerializeField] bool[] isPhotoOptions; // 是否是拍照选项
    [SerializeField] Sprite[] photoSprites; // 照片图片
    [SerializeField] string[] photoDescriptions; // 照片描述
    [SerializeField] bool isFinalState = false; // 是否为最终状态

    public string GetStateStory() => storyText;

    public string[] GetOptions() => options;

    public State[] GetNextStates() => nextStates;

    public Sprite GetStateImage() => stateImage;

    public bool HasMusicChoice() => hasMusicChoice && musicChoices.Length > 0;

    public AudioClip GetSelectedMusic(int index) =>
        HasMusicChoice() && index >= 0 && index < musicChoices.Length ? musicChoices[index] : null;

    public bool IsPhotoOption(int index) =>
        index >= 0 && index < isPhotoOptions.Length && isPhotoOptions[index];

    public Sprite GetPhotoSprite(int index) =>
        index >= 0 && index < photoSprites.Length ? photoSprites[index] : null;

    public string GetPhotoDescription(int index) =>
        index >= 0 && index < photoDescriptions.Length ? photoDescriptions[index] : null;

    public bool IsFinalState() => isFinalState;
}
