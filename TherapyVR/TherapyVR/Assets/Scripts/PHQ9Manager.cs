using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Attach to an empty GameObject named "PHQ9Manager" in Scene 2.
// Wire all panel and UI references in the Inspector.
// SeatTeleportTrigger calls StartQuestionnaire() when the player sits.
public class PHQ9Manager : MonoBehaviour
{
    public static PHQ9Manager Instance { get; private set; }

    public enum State { Idle, LanguageSelect, Intro, Questioning, Results }

    [Header("Language Panel")]
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Button englishButton;
    [SerializeField] private Button zuluButton;
    [SerializeField] private Button afrikaansButton;
    [SerializeField] private Button swahiliButton;

    [Header("Panels")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject resultsPanel;

    [Header("Static Text Labels")]
    [SerializeField] private TMP_Text welcomeTitleText;
    [SerializeField] private TMP_Text welcomeBodyText;
    [SerializeField] private TMP_Text startButtonText;
    [SerializeField] private TMP_Text backButtonText;
    [SerializeField] private TMP_Text nextButtonText;
    [SerializeField] private TMP_Text viewReportButtonText;
    [SerializeField] private TMP_Text resultsTotalLabelText;
    [SerializeField] private TMP_Text changeLanguageLabelText;

    [Header("Progress")]
    [SerializeField] private Slider progressBarFill;      // Image Type=Filled, Horizontal
    [SerializeField] private TMP_Text progressText;       // "11% Complete"

    [Header("Question UI")]
    [SerializeField] private TMP_Text questionNumberText; // "QUESTION 1 OF 9"
    [SerializeField] private TMP_Text questionBodyText;   // large bold question

    [Header("Answer Cards")]
    [SerializeField] private Button[] answerButtons;      // 4 card buttons, order 0-3
    [SerializeField] private TMP_Text[] answerLabelTexts;   // label TMP inside each card
    [SerializeField] private Image[] cardBackgrounds;     // root Image of each card
    [SerializeField] private GameObject[] cardCheckmarks;    // teal filled circle, hidden when unselected
    [SerializeField] private Color cardNormalColor = new(0.95f, 0.95f, 0.95f, 1f);
    [SerializeField] private Color cardSelectedColor = new(0.85f, 0.97f, 0.97f, 1f);

    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button changeLanguageButton;

    [Header("Results UI")]
    [SerializeField] private TMP_Text resultsTotalText;
    [SerializeField] private TMP_Text resultsSeverityText;
    [SerializeField] private TMP_Text resultsGuidanceText;
    [SerializeField] private Button viewReportButton;
    [SerializeField] private PHQ9LocalServer localServer;

    [Header("Safety (question 9)")]
    // Optional: a panel that appears when question 9 is answered above zero.
    // If you leave these empty, the safety message is shown in the results guidance text instead.
    [SerializeField] private GameObject safetyPanel;
    [SerializeField] private TMP_Text safetyMessageText;

    // Question 9 asks about thoughts of self-harm. It is the 9th question, so index 8.
    private const int SelfHarmQuestionIndex = 8;

    // Shown whenever question 9 is above zero. English only until translations are checked
    // by a qualified translator. Check these numbers are still correct before every release.
    public const string SafetyMessage =
        "Thank you for being honest. You do not have to carry this alone.\n\n" +
        "Please take off the headset and talk to your counsellor or someone you trust now.\n\n" +
        "SADAG Suicide Crisis Line: 0800 567 567 (free, 24 hours)\n" +
        "Lifeline South Africa: 0861 322 322\n" +
        "In an emergency call 112 from a cellphone.";

    // True when the last completed questionnaire had question 9 above zero.
    // Other scripts (for example Scene4) can check this.
    public static bool LastNeedsSafetyFollowUp { get; private set; }

    private State _state = State.Idle;
    private int _currentQuestion;
    private int[] _answers;        // committed answers per question (-1 = not yet answered)
    private int _selectedAnswer;   // pending card selection on current question (-1 = none)
    private string _language;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (englishButton)   englishButton.onClick.AddListener(()   => SelectLanguage("en"));
        if (zuluButton)      zuluButton.onClick.AddListener(()      => SelectLanguage("zu"));
        if (afrikaansButton) afrikaansButton.onClick.AddListener(() => SelectLanguage("af"));
        if (swahiliButton)   swahiliButton.onClick.AddListener(()   => SelectLanguage("sw"));

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int idx = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerCardClicked(idx));
        }

        if (nextButton != null)           nextButton.onClick.AddListener(OnNextButtonPressed);
        if (backButton != null)           backButton.onClick.AddListener(OnBackButtonPressed);
        if (changeLanguageButton != null) changeLanguageButton.onClick.AddListener(OnChangeLanguagePressed);

        SetPanels(false, false, false, false);
        if (safetyPanel != null) safetyPanel.SetActive(false);
    }

    // Called by SeatTeleportTrigger when the player sits.
    public void StartQuestionnaire()
    {
        if (_state != State.Idle) return;
        _state = State.LanguageSelect;

        _answers = new int[PHQ9Data.Questions.Length];
        for (int i = 0; i < _answers.Length; i++) _answers[i] = -1;

        SetPanels(true, false, false, false);
    }

    // Called by the 4 language buttons (wired in Start).
    public void SelectLanguage(string langCode)
    {
        _language = langCode;
        ApplyLanguage();
        _state = State.Intro;
        SetPanels(false, true, false, false);
    }

    // Wire to "Begin Assessment" button OnClick in Inspector.
    public void OnStartButtonPressed()
    {
        if (_state != State.Intro) return;
        _state = State.Questioning;
        SetPanels(false, false, true, false);
        ShowQuestion(0);
    }

    private void OnAnswerCardClicked(int index)
    {
        if (_state != State.Questioning) return;
        _selectedAnswer = index;
        UpdateCardVisuals();
        if (nextButton != null) nextButton.interactable = true;
    }

    private void OnNextButtonPressed()
    {
        if (_selectedAnswer < 0) return;

        _answers[_currentQuestion] = _selectedAnswer;
        _currentQuestion++;

        if (_currentQuestion >= PHQ9Data.Questions.Length)
            ShowResults();
        else
            ShowQuestion(_currentQuestion);
    }

    private void OnBackButtonPressed()
    {
        if (_currentQuestion <= 0) return;
        _currentQuestion--;
        ShowQuestion(_currentQuestion);
    }

    private void OnChangeLanguagePressed()
    {
        for (int i = 0; i < _answers.Length; i++) _answers[i] = -1;
        _currentQuestion = 0;
        _state = State.LanguageSelect;
        SetPanels(true, false, false, false);
    }

    private void ShowQuestion(int index)
    {
        _currentQuestion = index;
        _selectedAnswer = _answers[index]; // -1 if not yet answered, restores previous choice

        var ui = PHQ9Data.GetUIStrings(_language);

        questionNumberText.text = string.Format(ui.questionFormat, index + 1, PHQ9Data.Questions.Length);
        questionBodyText.text = PHQ9Data.GetQuestionText(index, _language);

        float progress = (index + 1) / (float)PHQ9Data.Questions.Length;
        if (progressBarFill != null) progressBarFill.value = progress;
        if (progressText != null)
            progressText.text = string.Format(ui.progressFormat, Mathf.RoundToInt(progress * 100));

        if (backButton != null) backButton.interactable = index > 0;
        if (nextButton != null) nextButton.interactable = _selectedAnswer >= 0;

        UpdateCardVisuals();
    }

    private void UpdateCardVisuals()
    {
        for (int i = 0; i < cardBackgrounds.Length; i++)
        {
            bool selected = i == _selectedAnswer;
            if (cardBackgrounds[i] != null)
                cardBackgrounds[i].color = selected ? cardSelectedColor : cardNormalColor;
            if (cardCheckmarks[i] != null)
                cardCheckmarks[i].SetActive(selected);
        }
    }

    private void ShowResults()
    {
        _state = State.Results;

        int total = 0;
        foreach (int a in _answers) if (a >= 0) total += a;

        resultsTotalText.text = $"{total}";
        resultsSeverityText.text = PHQ9Data.GetSeverity(total, _language);
        string guidance = PHQ9Data.GetGuidance(total, _language);

        bool needsSafety = _answers.Length > SelfHarmQuestionIndex && _answers[SelfHarmQuestionIndex] > 0;
        LastNeedsSafetyFollowUp = needsSafety;

        if (needsSafety)
        {
            // The safety message always comes first, whatever the total score is.
            guidance = SafetyMessage + "\n\n" + guidance;
            if (safetyPanel != null)
            {
                if (safetyMessageText != null) safetyMessageText.text = SafetyMessage;
                safetyPanel.SetActive(true);
            }
        }

        resultsGuidanceText.text = guidance;

        SetPanels(false, false, false, true);

        // Nothing is saved to the headset any more. The report lives in memory only,
        // so the next person on a shared headset cannot open it.
        PHQ9ReportGenerator.Generate(_answers, total, _language,
            PHQ9Data.GetSeverity(total, _language),
            guidance);
    }

    // Wire to "View Report" button OnClick in Inspector.
    public void OnViewReportButtonPressed()
    {
        if (localServer == null || string.IsNullOrEmpty(PHQ9ReportGenerator.LastGeneratedHtml)) return;
        localServer.Serve(PHQ9ReportGenerator.LastGeneratedHtml);
        Application.OpenURL("http://127.0.0.1:52879/report");
    }

    // Wire to "Done" button OnClick in Inspector (reserved for future navigation).
    public void OnDoneButtonPressed() { }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private void ApplyLanguage()
    {
        var ui = PHQ9Data.GetUIStrings(_language);

        if (welcomeTitleText)     welcomeTitleText.text     = ui.welcomeTitle;
        if (welcomeBodyText)      welcomeBodyText.text      = ui.welcomeBody;
        if (startButtonText)      startButtonText.text      = ui.startButton;
        if (backButtonText)       backButtonText.text       = ui.backButton;
        if (nextButtonText)       nextButtonText.text       = ui.nextButton;
        if (viewReportButtonText) viewReportButtonText.text = ui.viewReportButton;
        if (resultsTotalLabelText)    resultsTotalLabelText.text    = ui.resultsTotalLabel;
        if (changeLanguageLabelText)  changeLanguageLabelText.text  = ui.changeLanguageButton;

        for (int i = 0; i < answerLabelTexts.Length; i++)
            if (answerLabelTexts[i] != null)
                answerLabelTexts[i].text = PHQ9Data.GetAnswerLabel(i, _language);
    }

    private void SetPanels(bool language, bool welcome, bool question, bool results)
    {
        if (languagePanel != null) languagePanel.SetActive(language);
        if (welcomePanel  != null) welcomePanel.SetActive(welcome);
        if (questionPanel != null) questionPanel.SetActive(question);
        if (resultsPanel  != null) resultsPanel.SetActive(results);
    }
}
