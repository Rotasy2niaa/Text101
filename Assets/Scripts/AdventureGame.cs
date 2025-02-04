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

    private State state; // ��ǰ״̬
    private bool isTyping = false; // ��ֹ�ظ�����
    private bool isCustomMusicPlaying = false; // �Զ�������״̬
    private AudioClip currentCustomMusic = null; // ��ǰ�Զ�������
    private List<string> takenPhotos = new List<string>(); // �洢�������Ƭ����

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

        // ��¼��Ƭ��Ϣ
        if (state.IsPhotoOption(index))
        {
            string photoDescription = state.GetPhotoDescription(index);
            if (!string.IsNullOrEmpty(photoDescription))
            {
                takenPhotos.Add(photoDescription);
            }
        }

        // �����߼�
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

        // ���������״̬����ʾ��Ƭ
        if (state.IsFinalState())
        {
            DisplayFinalPhotos();
        }
        else
        {
            UpdateStateImage();
            StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions));
        }
    }

    private void UpdateStateImage()
    {
        if (state.GetStateImage() != null)
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

    private void DisplayFinalPhotos()
    {
        // ��̬��������״̬���ı�
        string photosText = "Today I looked back at my journey. Here are the photos I took:\n\n";

        if (takenPhotos.Count > 0)
        {
            photosText += string.Join("\n", takenPhotos);
        }
        else
        {
            photosText += "No photos taken.";
        }

        storyTextComponent.text = photosText; // ���¹����ı�
        ShowOptions(); // ��ʾ��ť
    }
}
