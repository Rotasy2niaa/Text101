using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdventureGame : MonoBehaviour
{
    [SerializeField] TMP_Text storyTextComponent; // ��ʾ�����ı�
    [SerializeField] GameObject buttonPrefab; // ��ťԤ����
    [SerializeField] Transform buttonContainer; // ��ť�ĸ��������ڲ��֣�
    [SerializeField] State startingState; // ��ʼ״̬
    [SerializeField] Image stateImageComponent; // ״̬ͼƬ���
    [SerializeField] AudioSource backgroundMusic; // �������� AudioSource
    [SerializeField] AudioSource gameMusic; // ��Ϸ���� AudioSource
    [SerializeField] float typingSpeed = 0.05f; // �����ٶ�

    private State state; // ��ǰ״̬
    private bool isTyping = false; // ��ֹ�ظ���������Ч��
    private bool isCustomMusicPlaying = false; // �ж��Ƿ񲥷��Զ�������

    void Start()
    {
        state = startingState;
        UpdateStateImage(); // ���³�ʼͼƬ
        backgroundMusic.Play(); // ȷ���������ֳ�ʼ����
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // ��ʼ��ʾ��ʼ����
    }

    /// <summary>
    /// ����Ч������ʾ�����ı�
    /// </summary>
    private IEnumerator PlayTypewriterEffect(string fullText, System.Action onComplete = null)
    {
        isTyping = true;
        storyTextComponent.text = ""; // ��չ����ı�
        HideOptions(); // ����ѡ�ť

        foreach (char letter in fullText)
        {
            storyTextComponent.text += letter; // ������ʾ
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        onComplete?.Invoke(); // ������ɺ�ص�
    }

    /// <summary>
    /// ��ʾѡ�ť
    /// </summary>
    private void ShowOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject); // ��վɰ�ť
        }

        var options = state.GetOptions();
        var nextStates = state.GetNextStates();

        for (int i = 0; i < options.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer); // ������ť
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = ""; // ��ʼ��Ϊ��

            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionSelected(index); // ��ӵ���¼�
            });

            StartCoroutine(PlayTypewriterEffectForButton(buttonText, options[i])); // Ϊ��ť�ı���Ӵ���Ч��
        }

        buttonContainer.gameObject.SetActive(true); // ��ʾ��ť����
    }

    /// <summary>
    /// ����ѡ�ť
    /// </summary>
    private void HideOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject); // ��վɰ�ť
        }
        buttonContainer.gameObject.SetActive(false); // ���ذ�ť����
    }

    /// <summary>
    /// Ϊѡ�ť�ı���Ӵ���Ч��
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
    /// ����ѡ�����¼�
    /// </summary>
    private void OnOptionSelected(int index)
    {
        if (isTyping) return; // ������ڴ��֣���ֹ�л�״̬

        // �л���ѡ�е�״̬
        State nextState = state.GetNextStates()[index];

        // ��������ѡ��
        if (state.HasMusicChoice())
        {
            AudioClip selectedMusic = state.GetSelectedMusic(index); // ����ѡ���ȡ��Ӧ����
            PlayCustomMusic(selectedMusic); // ����ѡ�е�����
        }
        else
        {
            StopCustomMusic(); // ֹͣ�Զ������֣��ָ���������
        }

        state = nextState; // ���µ�ǰ״̬
        UpdateStateImage(); // ����״̬ͼƬ
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // ��ʾ�µĹ����ı�����ʾѡ��
    }

    /// <summary>
    /// ����״̬ͼƬ
    /// </summary>
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

    /// <summary>
    /// �����Զ�������
    /// </summary>
    private void PlayCustomMusic(AudioClip selectedMusic)
    {
        if (selectedMusic == null) return; // û������ʱ����

        isCustomMusicPlaying = true;

        // ��ͣ��������
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        // �����Զ�������
        gameMusic.clip = selectedMusic;
        gameMusic.Play();
    }

    /// <summary>
    /// ֹͣ�Զ������ֲ��ָ���������
    /// </summary>
    private void StopCustomMusic()
    {
        if (isCustomMusicPlaying)
        {
            isCustomMusicPlaying = false;
            gameMusic.Stop(); // ֹͣ�Զ�������

            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.UnPause(); // �ָ���������
            }
        }
    }
}
