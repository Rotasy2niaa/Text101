using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText;
    [TextArea(3, 3)][SerializeField] string[] options;
    [SerializeField] State[] nextStates;
    [SerializeField] Sprite stateImage;
    [SerializeField] bool hasMusicChoice = false;
    [SerializeField] AudioClip[] musicChoices;
    [SerializeField] bool[] isPhotoOptions;
    [TextArea(2, 2)][SerializeField] string[] photoDescriptions;
    [SerializeField] bool isFinalState = false;

    public string GetStateStory() => storyText;

    public string[] GetOptions() => options;

    public State[] GetNextStates() => nextStates;

    public Sprite GetStateImage() => stateImage;

    public bool HasMusicChoice() => hasMusicChoice && musicChoices != null && musicChoices.Length > 0;

    public AudioClip GetSelectedMusic(int index) =>
        (HasMusicChoice() && index >= 0 && index < musicChoices.Length) ? musicChoices[index] : null;

    public bool IsPhotoOption(int index) =>
        isPhotoOptions != null && index >= 0 && index < isPhotoOptions.Length && isPhotoOptions[index];

    public string GetPhotoDescription(int index) =>
        (photoDescriptions != null && index >= 0 && index < photoDescriptions.Length) ? photoDescriptions[index] : null;

    public bool IsFinalState() => isFinalState;
}
