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
    [SerializeField] AudioSource backgroundMusic; // �������� AudioSource
    [SerializeField] AudioSource gameMusic; // ��Ϸ���� AudioSource
    [SerializeField] float typingSpeed = 0.05f; // �����ٶ�

    private State state; // ��ǰ״̬
    private bool isTyping = false; // ��ֹ�ظ���������Ч��
    private bool isCustomMusicPlaying = false; // �ж��Ƿ񲥷��Զ�������
    private AudioClip currentCustomMusic = null; // ��ǰ���ŵ��Զ�������

    void Start()
    {
        state = startingState;
        backgroundMusic.Play(); // ȷ���������ֳ�ʼ����
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // ��ʼ��ʾ��ʼ����
    }

    /// <summary>
    /// ����Ч����������ʾ�����ı�
    /// </summary>
    private IEnumerator PlayTypewriterEffect(string fullText, System.Action onComplete = null)
    {
        isTyping = true; // ���Ϊ���ڴ���
        storyTextComponent.text = ""; // ��չ����ı�

        // ����ѡ�ť
        HideOptions();

        foreach (char letter in fullText)
        {
            storyTextComponent.text += letter; // ������ʾ
            yield return new WaitForSeconds(typingSpeed); // ÿ���ַ��ļ��
        }

        isTyping = false; // ���ֽ���
        onComplete?.Invoke(); // ������ɻص�����ʾѡ�
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
            GameObject button = Instantiate(buttonPrefab, buttonContainer); // ���ɰ�ť
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = ""; // ��ʼ��Ϊ��

            int index = i; // ��ֹ�հ�����
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnOptionSelected(index); // ��ӵ���¼�
            });

            StartCoroutine(PlayTypewriterEffectForButton(buttonText, options[i])); // Ϊ��ť������Ӵ���Ч��
        }

        buttonContainer.gameObject.SetActive(true); // ��ʾ��ť����
    }

    private void HideOptions()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject); // ��վɰ�ť
        }
        buttonContainer.gameObject.SetActive(false); // ���ذ�ť����
    }

    private IEnumerator PlayTypewriterEffectForButton(TMP_Text buttonText, string optionText)
    {
        foreach (char letter in optionText)
        {
            buttonText.text += letter; // ������ʾ
            yield return new WaitForSeconds(typingSpeed); // ÿ���ַ��ļ��
        }
    }

    private void OnOptionSelected(int index)
    {
        if (isTyping) return; // ������ڴ��֣���ֹ�л�״̬

        // �л�����һ��״̬
        State nextState = state.GetNextStates()[index];

        // ����Ƿ���Ҫ����������
        if (state.HasMusicChoice())
        {
            AudioClip selectedMusic = state.GetSelectedMusic(index);
            PlayCustomMusic(selectedMusic); // ����ѡ�������
        }
        else
        {
            StopCustomMusic(); // û������ѡ��ʱֹͣ�Զ������֣��ָ���������
        }

        state = nextState; // ���µ�ǰ״̬
        StartCoroutine(PlayTypewriterEffect(state.GetStateStory(), ShowOptions)); // ��ʾ�µĹ����ı�����ʾѡ��
    }

    private void PlayCustomMusic(AudioClip selectedMusic)
    {
        if (selectedMusic == null) return; // ���û�����֣���ִ��

        isCustomMusicPlaying = true;
        currentCustomMusic = selectedMusic; // ���µ�ǰ���ŵ��Զ�������

        // ֹͣ��������
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        // �л��������Զ�������
        gameMusic.clip = selectedMusic;
        gameMusic.Play();
    }

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
