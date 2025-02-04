using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    [TextArea(14, 10)][SerializeField] string storyText; // Story text
    [TextArea(3, 3)][SerializeField] string[] options; // Option texts
    [SerializeField] State[] nextStates; // Next states
    [SerializeField] Sprite stateImage; // State image
    [SerializeField] bool hasMusicChoice = false; // Whether this state includes a music choice
    [SerializeField] AudioClip[] musicChoices; // Music choices for this state

    /// <summary>
    /// Returns the story text of this state.
    /// </summary>
    public string GetStateStory()
    {
        return storyText;
    }

    /// <summary>
    /// Returns the option texts for this state.
    /// </summary>
    public string[] GetOptions()
    {
        return options;
    }

    /// <summary>
    /// Returns the next states associated with this state.
    /// </summary>
    public State[] GetNextStates()
    {
        return nextStates;
    }

    /// <summary>
    /// Returns the image associated with this state.
    /// </summary>
    public Sprite GetStateImage()
    {
        return stateImage;
    }

    /// <summary>
    /// Checks if this state has music choices.
    /// </summary>
    public bool HasMusicChoice()
    {
        return hasMusicChoice && musicChoices != null && musicChoices.Length > 0;
    }

    /// <summary>
    /// Returns the music choice for the given index, if valid.
    /// </summary>
    public AudioClip GetSelectedMusic(int index)
    {
        if (HasMusicChoice() && index >= 0 && index < musicChoices.Length)
        {
            return musicChoices[index];
        }

        return null; // Return null if no valid music is found
    }
}
