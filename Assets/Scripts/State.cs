using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText; // �����ı�
    [TextArea(3, 3)][SerializeField] string[] options; // ѡ���ı�
    [SerializeField] State[] nextStates; // ��һ��״̬
    [SerializeField] Sprite stateImage; // ״̬ͼƬ
    [SerializeField] bool hasMusicChoice = false; // �Ƿ�������ѡ��
    [SerializeField] AudioClip[] musicChoices; // ����ѡ��
    [SerializeField] bool[] isPhotoOptions; // �Ƿ�������ѡ��
    [SerializeField] Sprite[] photoSprites; // ��ƬͼƬ
    [SerializeField] string[] photoDescriptions; // ��Ƭ����
    [SerializeField] bool isFinalState = false; // �Ƿ�Ϊ����״̬

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
