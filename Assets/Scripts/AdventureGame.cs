using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TMP_Text storyTextComponent; // 显示故事文本
    [SerializeField] GameObject buttonPrefab; // 按钮预制体
    [SerializeField] Transform buttonContainer; // 按钮的父对象（用于布局）
    [SerializeField] State startingState; // 初始状态
    [SerializeField] AudioSource backgroundMusic; // 背景音乐 AudioSource
    [SerializeField] AudioSource gameMusic; // 游戏音乐 AudioSource
    [SerializeField] float typingSpeed = 0.05f; // 打字速度

    private State state; // 当前状态
    private bool isTyping = false; // 防止重复触发打字效果
    private bool isCustomMusicPlaying = false; // 判断是否播放自定义音乐
    private AudioClip currentCustomMusic = null; // 当前播放的自定义音乐

    void Start()
    {
        state = startingState;
        backgroundMusic.Play(); // 确保背景音乐初始播放
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // 开始显示初始故事
    }

    /// <summary>
    /// 打字效果，用于显示故事文本
    /// </summary>
    private IEnumerator PlayTypewriterEffect(string fullText, System.Action onComplete = null)
    {
        isTyping = true; // 标记为正在打字
        storyTextComponent.text = ""; // 清空故事文本

        // 隐藏选项按钮
        HideOptions();

        foreach (char letter in fullText)
        {
            storyTextComponent.text += letter; // 按字显示
            yield return new WaitForSeconds(typingSpeed); // 每个字符的间隔
        }

        isTyping = false; // 打字结束
        onComplete?.Invoke(); // 调用完成回调（显示选项）
    }

    /// <summary>
    /// 显示选项按钮
    /// </summary>
    private void ShowOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject); // 清空旧按钮
        }

        var options = state.GetOptions();
        var nextStates = state.GetNextStates();

        for (int i = 0; i < options.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer); // 生成按钮
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = ""; // 初始化为空

            int index = i; // 防止闭包问题
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionSelected(index); // 添加点击事件
            });

            StartCoroutine(PlayTypewriterEffectForButton(buttonText, options[i])); // 为按钮文字添加打字效果
        }

        buttonContainer.gameObject.SetActive(true); // 显示按钮容器
    }

    private void HideOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject); // 清空旧按钮
        }
        buttonContainer.gameObject.SetActive(false); // 隐藏按钮容器
    }

    private IEnumerator PlayTypewriterEffectForButton(TMP_Text buttonText, string optionText)
    {
        foreach (char letter in optionText)
        {
            buttonText.text += letter; // 按字显示
            yield return new WaitForSeconds(typingSpeed); // 每个字符的间隔
        }
    }

    private void OnOptionSelected(int index)
    {
        if (isTyping) return; // 如果正在打字，禁止切换状态

        // 切换到下一个状态
        State nextState = state.GetNextStates()[index];

        // 检查是否需要播放新音乐
        if (state.HasMusicChoice())
        {
            AudioClip selectedMusic = state.GetSelectedMusic(index);
            PlayCustomMusic(selectedMusic); // 播放选择的音乐
        }
        else
        {
            StopCustomMusic(); // 没有音乐选择时停止自定义音乐，恢复背景音乐
        }

        state = nextState; // 更新当前状态
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // 显示新的故事文本并显示选项
    }

    private void PlayCustomMusic(AudioClip selectedMusic)
    {
        if (selectedMusic == null) return; // 如果没有音乐，不执行

        isCustomMusicPlaying = true;
        currentCustomMusic = selectedMusic; // 更新当前播放的自定义音乐

        // 停止背景音乐
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        // 切换并播放自定义音乐
        gameMusic.clip = selectedMusic;
        gameMusic.Play();
    }

    private void StopCustomMusic()
    {
        if (isCustomMusicPlaying)
        {
            isCustomMusicPlaying = false;
            gameMusic.Stop(); // 停止自定义音乐

            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.UnPause(); // 恢复背景音乐
            }
        }
    }
}
