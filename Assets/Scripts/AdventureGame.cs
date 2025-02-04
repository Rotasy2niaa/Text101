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
    [SerializeField] Image stateImageComponent; // 状态图片组件
    [SerializeField] AudioSource backgroundMusic; // 背景音乐 AudioSource
    [SerializeField] AudioSource gameMusic; // 游戏音乐 AudioSource
    [SerializeField] float typingSpeed = 0.05f; // 打字速度

    private State state; // 当前状态
    private bool isTyping = false; // 防止重复触发打字效果
    private bool isCustomMusicPlaying = false; // 判断是否播放自定义音乐

    void Start()
    {
        state = startingState;
        UpdateStateImage(); // 更新初始图片
        backgroundMusic.Play(); // 确保背景音乐初始播放
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // 开始显示初始故事
    }

    /// <summary>
    /// 打字效果：显示故事文本
    /// </summary>
    private IEnumerator PlayTypewriterEffect(string fullText, System.Action onComplete = null)
    {
        isTyping = true;
        storyTextComponent.text = ""; // 清空故事文本
        HideOptions(); // 隐藏选项按钮

        foreach (char letter in fullText)
        {
            storyTextComponent.text += letter; // 按字显示
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        onComplete?.Invoke(); // 打字完成后回调
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
            GameObject button = Instantiate(buttonPrefab, buttonContainer); // 创建按钮
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = ""; // 初始化为空

            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionSelected(index); // 添加点击事件
            });

            StartCoroutine(PlayTypewriterEffectForButton(buttonText, options[i])); // 为按钮文本添加打字效果
        }

        buttonContainer.gameObject.SetActive(true); // 显示按钮容器
    }

    /// <summary>
    /// 隐藏选项按钮
    /// </summary>
    private void HideOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject); // 清空旧按钮
        }
        buttonContainer.gameObject.SetActive(false); // 隐藏按钮容器
    }

    /// <summary>
    /// 为选项按钮文本添加打字效果
    /// </summary>
    private IEnumerator PlayTypewriterEffectForButton(TMP_Text buttonText, string optionText)
    {
        foreach (char letter in optionText)
        {
            buttonText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    /// <summary>
    /// 处理选项点击事件
    /// </summary>
    private void OnOptionSelected(int index)
    {
        if (isTyping) return; // 如果正在打字，禁止切换状态

        // 切换到选中的状态
        State nextState = state.GetNextStates()[index];

        // 处理音乐选择
        if (state.HasMusicChoice())
        {
            AudioClip selectedMusic = state.GetSelectedMusic(index); // 根据选项获取对应音乐
            PlayCustomMusic(selectedMusic); // 播放选中的音乐
        }
        else
        {
            StopCustomMusic(); // 停止自定义音乐，恢复背景音乐
        }

        state = nextState; // 更新当前状态
        UpdateStateImage(); // 更新状态图片
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // 显示新的故事文本并显示选项
    }

    /// <summary>
    /// 更新状态图片
    /// </summary>
    private void UpdateStateImage()
    {
        if (state.GetStateImage() != null)
        {
            stateImageComponent.sprite = state.GetStateImage(); // 更新图片
            stateImageComponent.gameObject.SetActive(true); // 显示图片
        }
        else
        {
            stateImageComponent.gameObject.SetActive(false); // 隐藏图片
        }
    }

    /// <summary>
    /// 播放自定义音乐
    /// </summary>
    private void PlayCustomMusic(AudioClip selectedMusic)
    {
        if (selectedMusic == null) return; // 没有音乐时跳过

        isCustomMusicPlaying = true;

        // 暂停背景音乐
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        // 播放自定义音乐
        gameMusic.clip = selectedMusic;
        gameMusic.Play();
    }

    /// <summary>
    /// 停止自定义音乐并恢复背景音乐
    /// </summary>
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
