using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TMP_Text storyTextComponent; // 显示故事文本
    [SerializeField] GameObject buttonPrefab; // 按钮预制体
    [SerializeField] Transform buttonContainer; // 按钮的父对象
    [SerializeField] State startingState; // 初始状态
    [SerializeField] Image stateImageComponent; // 状态图片组件
    [SerializeField] AudioSource backgroundMusic; // 背景音乐
    [SerializeField] AudioSource gameMusic; // 游戏音乐
    [SerializeField] float typingSpeed = 0.05f; // 打字速度

    private State state; // 当前状态
    private bool isTyping = false; // 防止重复打字
    private bool isCustomMusicPlaying = false; // 自定义音乐状态
    private AudioClip currentCustomMusic = null; // 当前自定义音乐
    private List<string> takenPhotos = new List<string>(); // 存储拍摄的照片描述

    void Start()
    {
        state = startingState;
        UpdateStateImage(); // 更新图片
        backgroundMusic.Play(); // 播放背景音乐
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // 显示初始文本
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

        // 记录照片信息
        if (state.IsPhotoOption(index))
        {
            string photoDescription = state.GetPhotoDescription(index);
            if (!string.IsNullOrEmpty(photoDescription))
            {
                takenPhotos.Add(photoDescription);
            }
        }

        // 音乐逻辑
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

        // 如果是最终状态，显示照片
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
        // 动态生成最终状态的文本
        string photosText = "Today I looked back at my journey. Here are the photos I took:\n\n";

        if (takenPhotos.Count > 0)
        {
            photosText += string.Join("\n", takenPhotos);
        }
        else
        {
            photosText += "No photos taken.";
        }

        storyTextComponent.text = photosText; // 更新故事文本
        ShowOptions(); // 显示按钮
    }
}
