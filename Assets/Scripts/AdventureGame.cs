using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TMP_Text storyTextComponent; // Display story text
    [SerializeField] GameObject buttonPrefab; // Button prefab
    [SerializeField] Transform buttonContainer; // Parent object for buttons
    [SerializeField] State startingState; // Initial state
    [SerializeField] Image stateImageComponent; // State image component
    [SerializeField] AudioSource backgroundMusic; // Background music
    [SerializeField] AudioSource gameMusic; // Game music
    [SerializeField] float typingSpeed = 0.05f; // Typing speed
    [SerializeField] Transform finalPhotoContainer; // Container for final photos
    [SerializeField] GameObject photoImagePrefab; // Prefab for displaying photos

    private State state; // Current state
    private bool isTyping = false; // Prevent duplicate typing
    private bool isCustomMusicPlaying = false; // Track custom music state
    private AudioClip currentCustomMusic = null; // Current custom music
    private List<Sprite> takenPhotos = new List<Sprite>(); // List of taken photos
    private List<string> takenPhotoDescriptions = new List<string>(); // List of photo descriptions

    void Start()
    {
        state = startingState;
        UpdateStateImage(); // Update the image
        backgroundMusic.Play(); // Play background music
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // Start story
    }

    private IEnumerator PlayTypewriterEffect(string fullText, System.Action onComplete = null)
    {
        isTyping = true;
        storyTextComponent.text = "";
        HideOptions();

        foreach (char letter in fullText)
        {
            storyTextComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        onComplete?.Invoke();
    }

    private void ShowOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        var options = state.GetOptions();
        var nextStates = state.GetNextStates();

        for (int i = 0; i < options.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = "";

            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionSelected(index);
            });

            StartCoroutine(PlayTypewriterEffectForButton(buttonText, options[i]));
        }

        buttonContainer.gameObject.SetActive(true);
    }

    private void HideOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        buttonContainer.gameObject.SetActive(false);
    }

    private IEnumerator PlayTypewriterEffectForButton(TMP_Text buttonText, string optionText)
    {
        foreach (char letter in optionText)
        {
            buttonText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void OnOptionSelected(int index)
    {
        if (isTyping) return;

        State nextState = state.GetNextStates()[index];

        // Record photos if applicable
        if (state.IsPhotoOption(index))
        {
            Sprite photoSprite = state.GetPhotoSprite(index);
            string photoDescription = state.GetPhotoDescription(index);

            if (photoSprite != null)
            {
                takenPhotos.Add(photoSprite); // Add photo
            }

            if (!string.IsNullOrEmpty(photoDescription))
            {
                takenPhotoDescriptions.Add(photoDescription); // Add description
            }
        }

        // Handle music
        if (state.HasMusicChoice())
        {
            AudioClip selectedMusic = state.GetSelectedMusic(index);
            PlayCustomMusic(selectedMusic);
        }
        else
        {
            StopCustomMusic();
        }

        state = nextState;

        // Check if it's the final state
        if (state.IsFinalState())
        {
            DisplayFinalPhotosAndText();
        }
        else
        {
            UpdateStateImage();
            StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions));
        }
    }

    private void UpdateStateImage()
    {
        if (state.IsFinalState())
        {
            // 如果是最终状态，隐藏状态图片
            stateImageComponent.sprite = null;
            stateImageComponent.gameObject.SetActive(false);
        }
        else if (state.GetStateImage() != null)
        {
            stateImageComponent.sprite = state.GetStateImage();
            stateImageComponent.gameObject.SetActive(true);
        }
        else
        {
            stateImageComponent.gameObject.SetActive(false);
        }
    }


    private void PlayCustomMusic(AudioClip selectedMusic)
    {
        if (selectedMusic == currentCustomMusic) return;

        currentCustomMusic = selectedMusic;
        isCustomMusicPlaying = true;

        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        gameMusic.clip = selectedMusic;
        gameMusic.Play();
    }

    private void StopCustomMusic()
    {
        if (isCustomMusicPlaying)
        {
            isCustomMusicPlaying = false;
            currentCustomMusic = null;
            gameMusic.Stop();

            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.UnPause();
            }
        }
    }

    private void DisplayFinalPhotosAndText()
    {
        // 清空之前的图片
        foreach (Transform child in finalPhotoContainer)
        {
            Destroy(child.gameObject);
        }

        // 拼接照片文字描述
        string photosText = "Today I looked back at my journey. Here are the photos I took:\n\n";

        if (takenPhotoDescriptions.Count > 0)
        {
            photosText += string.Join("\n", takenPhotoDescriptions);
        }
        else
        {
            photosText += "No photos taken.";
        }
        storyTextComponent.text = photosText;

        // 清空之前的状态图片
        stateImageComponent.sprite = null;
        stateImageComponent.gameObject.SetActive(false);

        // 动态生成照片
        int maxPhotos = Mathf.Min(takenPhotos.Count, 3); // 最多显示三张
        for (int i = 0; i < maxPhotos; i++)
        {
            GameObject photoImage = Instantiate(photoImagePrefab, finalPhotoContainer);
            Image imageComponent = photoImage.GetComponent<Image>();

            // 设置照片 Sprite 数据
            imageComponent.sprite = takenPhotos[i];
        }

        // 显示选项按钮（Play Again）
        ShowFinalOptions();
    }


    private void ShowFinalOptions()
    {
        // Clear previous buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create "Play Again" button
        GameObject playAgainButton = Instantiate(buttonPrefab, buttonContainer);
        TMP_Text buttonText = playAgainButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "> Play Again";

        playAgainButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            RestartGame();
        });

        buttonContainer.gameObject.SetActive(true);
    }

    private void RestartGame()
    {
        // Reset the game state
        state = startingState;
        takenPhotos.Clear();
        takenPhotoDescriptions.Clear();

        // Clear final photos
        foreach (Transform child in finalPhotoContainer)
        {
            Destroy(child.gameObject);
        }

        // Restart game
        UpdateStateImage();
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions));
    }
}
