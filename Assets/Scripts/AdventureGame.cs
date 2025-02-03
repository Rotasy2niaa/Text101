using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TMP_Text storyTextComponent; // 显示故事文本
    [SerializeField] GameObject buttonPrefab; // 按钮预制体
    [SerializeField] Transform buttonContainer; // 按钮的父对象（用于布局）
    [SerializeField] State startingState; // 初始状态
    [SerializeField] Image stateImageComponent; // 状态图片组件
    [SerializeField] float typingSpeed = 0.05f; // 打字速度

    private State state; // 当前状态
    private bool isTyping = false; // 防止重复触发打字效果

    void Start()
    {
        state = startingState;
        UpdateStateImage(); // 更新初始图片
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // 开始显示初始故事
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

        state = state.GetNextStates()[index];
        UpdateStateImage(); // 更新图片
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions));
    }

    private void UpdateStateImage()
    {
        if (state.GetStateImage() != null)
        {
            stateImageComponent.sprite = state.GetStateImage(); // 设置图片
            stateImageComponent.gameObject.SetActive(true); // 显示图片
        }
        else
        {
            stateImageComponent.gameObject.SetActive(false); // 隐藏图片
        }
    }
}
