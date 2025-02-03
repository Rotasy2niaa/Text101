using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TMP_Text storyTextComponent; // ��ʾ�����ı�
    [SerializeField] GameObject buttonPrefab; // ��ťԤ����
    [SerializeField] Transform buttonContainer; // ��ť�ĸ��������ڲ��֣�
    [SerializeField] State startingState; // ��ʼ״̬
    [SerializeField] Image stateImageComponent; // ״̬ͼƬ���
    [SerializeField] float typingSpeed = 0.05f; // �����ٶ�

    private State state; // ��ǰ״̬
    private bool isTyping = false; // ��ֹ�ظ���������Ч��

    void Start()
    {
        state = startingState;
        UpdateStateImage(); // ���³�ʼͼƬ
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // ��ʼ��ʾ��ʼ����
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
        UpdateStateImage(); // ����ͼƬ
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions));
    }

    private void UpdateStateImage()
    {
        if (state.GetStateImage() != null)
        {
            stateImageComponent.sprite = state.GetStateImage(); // ����ͼƬ
            stateImageComponent.gameObject.SetActive(true); // ��ʾͼƬ
        }
        else
        {
            stateImageComponent.gameObject.SetActive(false); // ����ͼƬ
        }
    }
}
