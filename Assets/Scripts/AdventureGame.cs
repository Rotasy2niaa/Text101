using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TMP_Text storyTextComponent; // ��ʾ�����ı�
    [SerializeField] GameObject buttonPrefab; // ��ťԤ����
    [SerializeField] Transform buttonContainer; // ��ť�ĸ�����
    [SerializeField] State startingState; // ��ʼ״̬
    [SerializeField] Image stateImageComponent; // ״̬ͼƬ���
    [SerializeField] AudioSource backgroundMusic; // ��������
    [SerializeField] AudioSource gameMusic; // ��Ϸ����
    [SerializeField] float typingSpeed = 0.05f; // �����ٶ�
    [SerializeField] Transform finalPhotoContainer; // ����������Ƭ�ĸ�����
    [SerializeField] GameObject photoImagePrefab; // ��ƬͼƬԤ����

    private State state; // ��ǰ״̬
    private bool isTyping = false; // ��ֹ�ظ�����
    private bool isCustomMusicPlaying = false; // �Զ�������״̬
    private AudioClip currentCustomMusic = null; // ��ǰ�Զ�������
    private List<Sprite> takenPhotos = new List<Sprite>(); // �洢��������ͼƬ
    private List<string> takenPhotoDescriptions = new List<string>(); // �洢������������

    void Start()
    {
        state = startingState;
        UpdateStateImage(); // ����ͼƬ
        backgroundMusic.Play(); // ���ű�������
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // ��ʾ��ʼ�ı�
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

        // �������Ƭѡ���¼��Ƭ
        if (state.IsPhotoOption(index))
        {
            Sprite photoSprite = state.GetPhotoSprite(index);
            string photoDescription = state.GetPhotoDescription(index);

            if (photoSprite != null)
            {
                takenPhotos.Add(photoSprite); // �����Ƭ
            }

            if (!string.IsNullOrEmpty(photoDescription))
            {
                // �������������ƴ���������ı��У�
                takenPhotoDescriptions.Add(photoDescription);
            }
        }

        // �����߼�
        if (state.HasMusicChoice())
        {
            AudioClip selectedMusic = state.GetSelectedMusic(index);
            if (selectedMusic != null)
            {
                PlayCustomMusic(selectedMusic); // ����������
            }
        }

        state = nextState;

        // ���������״̬����ʾ��Ƭ������
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
        // �����ǰ״̬������״̬������ͼƬ���
        if (state.IsFinalState())
        {
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
        if (selectedMusic == currentCustomMusic)
        {
            return; // �����ǰ�����Ѿ��ڲ��ţ������κθı�
        }

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
        // ���֮ǰ��ͼƬ
        foreach (Transform child in finalPhotoContainer)
        {
            Destroy(child.gameObject);
        }

        // ����״̬ͼƬ����ֹ��ʾ��һ�� State ��ͼƬ
        stateImageComponent.sprite = null;
        stateImageComponent.gameObject.SetActive(false);

        // ƴ����Ƭ��������
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

        // ��̬������Ƭ
        int maxPhotos = Mathf.Min(takenPhotos.Count, 3); // �����ʾ����
        for (int i = 0; i < maxPhotos; i++)
        {
            GameObject photoImage = Instantiate(photoImagePrefab, finalPhotoContainer);
            Image imageComponent = photoImage.GetComponent<Image>();

            // ʹ����ȷ�� Sprite ����
            imageComponent.sprite = takenPhotos[i];
        }

        // ��ʾѡ�ť��Play Again ��������
        ShowFinalOptions();
    }


    private void ShowFinalOptions()
    {
        // ���֮ǰ�İ�ť
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // ���� "Play Again" ��ť
        GameObject playAgainButton = Instantiate(buttonPrefab, buttonContainer);
        TMP_Text buttonText = playAgainButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "> Play Again";

        // ��ӵ���¼������¿�ʼ��Ϸ
        playAgainButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            RestartGame();
        });

        // ��ʾ��ť����
        buttonContainer.gameObject.SetActive(true);
    }

    private void RestartGame()
    {
        // ������Ϸ״̬
        state = startingState;
        takenPhotos.Clear();
        takenPhotoDescriptions.Clear();

        // �����Ƭ����
        foreach (Transform child in finalPhotoContainer)
        {
            Destroy(child.gameObject);
        }

        // �ָ���ʼ״̬
        UpdateStateImage();
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions));

        // ��������
        StopCustomMusic();
        backgroundMusic.Play();
    }
}
