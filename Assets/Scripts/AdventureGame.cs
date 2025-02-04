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
    [SerializeField] Transform finalPhotoContainer; // 容纳最终照片的父对象
    [SerializeField] GameObject photoImagePrefab; // 照片图片预制体

    private State state; // 当前状态
    private bool isTyping = false; // 防止重复打字
    private bool isCustomMusicPlaying = false; // 自定义音乐状态
    private AudioClip currentCustomMusic = null; // 当前自定义音乐
    private List<Sprite> takenPhotos = new List<Sprite>(); // 存储玩家拍摄的图片
    private List<string> takenPhotoDescriptions = new List<string>(); // 存储玩家拍摄的描述

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

        // 如果是照片选项，记录照片
        if (state.IsPhotoOption(index))
        {
            Sprite photoSprite = state.GetPhotoSprite(index);
            string photoDescription = state.GetPhotoDescription(index);

            if (photoSprite != null)
            {
                takenPhotos.Add(photoSprite); // 添加照片
            }

            if (!string.IsNullOrEmpty(photoDescription))
            {
                // 添加描述（用于拼接在最终文本中）
                takenPhotoDescriptions.Add(photoDescription);
            }
        }

        // 音乐逻辑
        if (state.HasMusicChoice())
        {
            AudioClip selectedMusic = state.GetSelectedMusic(index);
            if (selectedMusic != null)
            {
                PlayCustomMusic(selectedMusic); // 播放新音乐
            }
        }

        state = nextState;

        // 如果是最终状态，显示照片和文字
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
        // 如果当前状态是最终状态，隐藏图片组件
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
            return; // 如果当前音乐已经在播放，不做任何改变
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
        // 清空之前的图片
        foreach (Transform child in finalPhotoContainer)
        {
            Destroy(child.gameObject);
        }

        // 隐藏状态图片，防止显示上一个 State 的图片
        stateImageComponent.sprite = null;
        stateImageComponent.gameObject.SetActive(false);

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

        // 动态生成照片
        int maxPhotos = Mathf.Min(takenPhotos.Count, 3); // 最多显示三张
        for (int i = 0; i < maxPhotos; i++)
        {
            GameObject photoImage = Instantiate(photoImagePrefab, finalPhotoContainer);
            Image imageComponent = photoImage.GetComponent<Image>();

            // 使用正确的 Sprite 数据
            imageComponent.sprite = takenPhotos[i];
        }

        // 显示选项按钮（Play Again 或其他）
        ShowFinalOptions();
    }


    private void ShowFinalOptions()
    {
        // 清空之前的按钮
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建 "Play Again" 按钮
        GameObject playAgainButton = Instantiate(buttonPrefab, buttonContainer);
        TMP_Text buttonText = playAgainButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "> Play Again";

        // 添加点击事件，重新开始游戏
        playAgainButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            RestartGame();
        });

        // 显示按钮容器
        buttonContainer.gameObject.SetActive(true);
    }

    private void RestartGame()
    {
        // 重置游戏状态
        state = startingState;
        takenPhotos.Clear();
        takenPhotoDescriptions.Clear();

        // 清空照片容器
        foreach (Transform child in finalPhotoContainer)
        {
            Destroy(child.gameObject);
        }

        // 恢复初始状态
        UpdateStateImage();
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions));

        // 重置音乐
        StopCustomMusic();
        backgroundMusic.Play();
    }
}
