public static class PHQ9Data
{
    public struct Question
    {
        public string key;
        public string english;
        public string zulu;
        public string afrikaans;
        public string swahili;
    }

    public struct UIStrings
    {
        public string welcomeTitle;
        public string welcomeBody;
        public string startButton;
        public string questionFormat;   // "{0}" = number, "{1}" = total  e.g. "QUESTION {0} OF {1}"
        public string progressFormat;   // "{0}" = percent  e.g. "{0}% Complete"
        public string backButton;
        public string nextButton;
        public string viewReportButton;
        public string resultsTotalLabel;
        public string reportTitle;
        public string reportResponsesHeading;
        public string reportColQuestion;
        public string reportColAnswer;
        public string reportColScore;
        public string reportGuidanceLabel;
        public string languagePanelTitle;
        public string changeLanguageButton;
    }

    public static readonly Question[] Questions =
    {
        new Question
        {
            key        = "phq9_q1",
            english    = "Little interest or pleasure in doing things",
            zulu       = "Ukushoda intshisekelo noma ukuthokoza ekwenzeni izinto",
            afrikaans  = "Min belangstelling of plesier in dinge doen",
            swahili    = "Kupungua kwa hamu au furaha katika kufanya mambo",
        },
        new Question
        {
            key        = "phq9_q2",
            english    = "Feeling down, depressed, or hopeless",
            zulu       = "Ukuzwa ukudangala, ukuphelelwa ithemba, noma ukuzwa buhlungu",
            afrikaans  = "Gevoel van neerslagtigheid, depressiwiteit, of hopeloosheid",
            swahili    = "Kuhisi chini, unyogovu, au kutokuwa na matumaini",
        },
        new Question
        {
            key        = "phq9_q3",
            english    = "Trouble falling or staying asleep, or sleeping too much",
            zulu       = "Inkinga yokulala noma ukulala kakhulu",
            afrikaans  = "Moeite om aan die slaap te raak of aan die slaap te bly, of om te veel te slaap",
            swahili    = "Ugumu wa kulala au kulala kupita kiasi",
        },
        new Question
        {
            key        = "phq9_q4",
            english    = "Feeling tired or having little energy",
            zulu       = "Ukuzwa ukukhathala noma ukusweleka kwamandla",
            afrikaans  = "Moeg voel of min energie hê",
            swahili    = "Kuhisi uchovu au kukosa nguvu",
        },
        new Question
        {
            key        = "phq9_q5",
            english    = "Poor appetite or overeating",
            zulu       = "Ukudla okuncane noma ukudla ngokweqile",
            afrikaans  = "Swak eetlus of oormatige eetlus",
            swahili    = "Kukosa hamu ya kula au kula kupita kiasi",
        },
        new Question
        {
            key        = "phq9_q6",
            english    = "Feeling bad about yourself — or that you are a failure or have let yourself or your family down",
            zulu       = "Ukuzizwa kabi ngawe uqobo — noma ukucabanga ukuthi awuphumelelanga noma ukwehliselile wena noma umndeni wakho",
            afrikaans  = "Sleg voel oor jouself — of dat jy 'n mislukking is of jouself of jou gesin in die steek gelaat het",
            swahili    = "Kujisikia vibaya kuhusu wewe mwenyewe — au kwamba umeshindwa au kumkatisha tamaa mwenzako au familia yako",
        },
        new Question
        {
            key        = "phq9_q7",
            english    = "Trouble concentrating on things, such as reading the newspaper or watching television",
            zulu       = "Inkinga yokugxila ezintweni, njengo kufunda iphephandaba noma ukubukela umabonakude",
            afrikaans  = "Moeite om te konsentreer op dinge soos om die koerant te lees of televisie te kyk",
            swahili    = "Ugumu wa kuzingatia mambo kama vile kusoma gazeti au kutazama televisheni",
        },
        new Question
        {
            key        = "phq9_q8",
            english    = "Moving or speaking so slowly that other people could have noticed. Or the opposite — being so fidgety or restless that you have been moving around a lot more than usual",
            zulu       = "Ukuhamba noma ukukhuluma kancane ukuthi abanye bangake bakubone. Noma okuphambene — ukukhathazeka noma ukungaphumuli kakhulu ukuthi uhamba kakhulu",
            afrikaans  = "Stadiger beweeg of praat as gewoonlik sodat ander mense dit kon opmerk. Of die teenoorgestelde — so rusteloos of senuweeagtig gevoel dat jy baie meer as gewoonlik rondbeweeg het",
            swahili    = "Kutembea au kuzungumza polepole kiasi kwamba watu wengine wangeweza kutambua. Au kinyume chake — kuwa na wasiwasi au kutokuwa na utulivu hadi umetembea zaidi ya kawaida",
        },
        new Question
        {
            key        = "phq9_q9",
            english    = "Thoughts that you would be better off dead, or of hurting yourself in some way",
            zulu       = "Imicabango yokuthi ungcono ukufa, noma yokuzilimaza ngandlela thile",
            afrikaans  = "Gedagtes dat jy beter af sou wees as jy dood was, of gedagtes om jouself seer te maak",
            swahili    = "Mawazo kwamba ungekuwa bora zaidi ukiwa umekufa, au mawazo ya kujidhuru kwa njia yoyote",
        },
    };

    // ─── Answer labels ────────────────────────────────────────────────────────

    private static readonly string[] _answersEn = { "Not at all", "Several days", "More than half the days", "Nearly every day" };
    private static readonly string[] _answersZu = { "Akukho nhlobo", "Izinsuku ezimbalwa", "Ngaphezu kwehhalf yezinsuku", "Cishe izinsuku zonke" };
    private static readonly string[] _answersAf = { "Glad nie", "Verskeie dae", "Meer as die helfte van die dae", "Amper elke dag" };
    private static readonly string[] _answersSw = { "Kabisa hapana", "Siku kadhaa", "Zaidi ya nusu ya siku", "Karibu kila siku" };

    // ─── Severity labels ──────────────────────────────────────────────────────
    // Index maps to: 0=none, 1=minimal, 2=mild, 3=moderate, 4=mod-severe, 5=severe

    private static readonly string[] _severityEn = { "No depression", "Minimal depression", "Mild depression", "Moderate depression", "Moderately severe depression", "Severe depression" };
    private static readonly string[] _severityZu = { "Akukho ukudangala", "Ukudangala okuncane kakhulu", "Ukudangala okuncane", "Ukudangala okumaphakathi", "Ukudangala okumaphakathi okukhulu", "Ukudangala okukhulu" };
    private static readonly string[] _severityAf = { "Geen depressie", "Minimale depressie", "Ligte depressie", "Matige depressie", "Matig ernstige depressie", "Ernstige depressie" };
    private static readonly string[] _severitySw = { "Hakuna unyogovu", "Unyogovu mdogo sana", "Unyogovu mdogo", "Unyogovu wa wastani", "Unyogovu mkubwa wa wastani", "Unyogovu mkubwa" };

    // ─── Guidance messages ────────────────────────────────────────────────────

    private static readonly string[] _guidanceEn =
    {
        "No concerns detected. Keep taking care of yourself.",
        "You're doing well. Keep taking care of yourself.",
        "It's okay to reach out. Small steps can make a big difference.",
        "You deserve support. Consider speaking with a healthcare provider.",
        "You are not alone. We encourage you to seek professional help soon.",
        "Please reach out to a mental health professional as soon as possible. You matter.",
    };

    private static readonly string[] _guidanceZu =
    {
        "Akukho inkinga etholiwe. Qhubeka uzinakekela.",
        "Wenza kahle. Qhubeka uzinakekela.",
        "Kulungile ukufuna usizo. Izinyathelo ezincane zingenza umahluko omkhulu.",
        "Ufanelwe usizo. Cabanga ukukhuluma nomhlinzeki wezempilo.",
        "Awukho wedwa. Sikhuthaza ukuba ufune usizo lochwepheshe maduze nje.",
        "Siza uthintane nomhlinzeki wezempilo yokuphepha ngokushesha okukhulu. Ubalulekile.",
    };

    private static readonly string[] _guidanceAf =
    {
        "Geen probleme gevind nie. Hou aan om vir jouself te sorg.",
        "Jy doen goed. Hou aan om vir jouself te sorg.",
        "Dit is okay om uit te reik. Klein stappe kan 'n groot verskil maak.",
        "Jy verdien ondersteuning. Oorweeg om met 'n gesondheidswerker te praat.",
        "Jy is nie alleen nie. Ons moedig jou aan om binnekort professionele hulp te soek.",
        "Reik asseblief so gou moontlik na 'n geestesgesondheidswerker uit. Jy is belangrik.",
    };

    private static readonly string[] _guidanceSw =
    {
        "Hakuna wasiwasi uliogundulika. Endelea kujitunza.",
        "Unafanya vizuri. Endelea kujitunza.",
        "Ni sawa kutafuta msaada. Hatua ndogo zinaweza kuleta tofauti kubwa.",
        "Unastahili msaada. Fikiria kuzungumza na mtoa huduma wa afya.",
        "Huko peke yako. Tunakuhimiza utafute msaada wa kitaalamu hivi karibuni.",
        "Tafadhali wasiliana na mtaalamu wa afya ya akili haraka iwezekanavyo. Una thamani.",
    };

    // ─── UI string tables ─────────────────────────────────────────────────────

    private static readonly UIStrings _uiEn = new UIStrings
    {
        welcomeTitle = "PHQ-9 Assessment",
        welcomeBody = "This questionnaire asks 9 questions about how you have been feeling over the past 2 weeks.",
        startButton = "Begin Assessment",
        questionFormat = "QUESTION {0} OF {1}",
        progressFormat = "{0}% Complete",
        backButton = "Back",
        nextButton = "Next",
        viewReportButton = "View Report",
        resultsTotalLabel = "Total PHQ-9 Score",
        reportTitle = "PHQ-9 Depression Assessment",
        reportResponsesHeading = "Responses",
        reportColQuestion = "Question",
        reportColAnswer = "Answer",
        reportColScore = "Score",
        reportGuidanceLabel = "Guidance",
        languagePanelTitle = "Select Language",
        changeLanguageButton = "Change Language",
    };

    private static readonly UIStrings _uiZu = new UIStrings
    {
        welcomeTitle = "Ukuhlolwa kwe-PHQ-9",
        welcomeBody = "Le mibuzo izobuza imibuzo engu-9 ngendlela ozizwe ngayo ezinsukwini eziyishumi nane ezedlule.",
        startButton = "Qala Ukuhlola",
        questionFormat = "UMBUZO {0} KWEZI {1}",
        progressFormat = "{0}% Kuqedwe",
        backButton = "Emuva",
        nextButton = "Umbuzo Olandelayo",
        viewReportButton = "Buka Umbiko",
        resultsTotalLabel = "Isikoro sePHQ-9",
        reportTitle = "Ukuhlolwa Kokudangala kwe-PHQ-9",
        reportResponsesHeading = "Izimpendulo",
        reportColQuestion = "Umbuzo",
        reportColAnswer = "Impendulo",
        reportColScore = "Isikoro",
        reportGuidanceLabel = "Izeluleko",
        languagePanelTitle = "Khetha Ulimi",
        changeLanguageButton = "Shintsha Ulimi",
    };

    private static readonly UIStrings _uiAf = new UIStrings
    {
        welcomeTitle = "PHQ-9 Assessering",
        welcomeBody = "Hierdie vraelys stel 9 vrae oor hoe jy die afgelope 2 weke gevoel het.",
        startButton = "Begin Assessering",
        questionFormat = "VRAAG {0} VAN {1}",
        progressFormat = "{0}% Voltooi",
        backButton = "Terug",
        nextButton = "Volgende Vraag",
        viewReportButton = "Bekyk Verslag",
        resultsTotalLabel = "Totale PHQ-9 Telling",
        reportTitle = "PHQ-9 Depressie-assessering",
        reportResponsesHeading = "Antwoorde",
        reportColQuestion = "Vraag",
        reportColAnswer = "Antwoord",
        reportColScore = "Telling",
        reportGuidanceLabel = "Leiding",
        languagePanelTitle = "Kies Taal",
        changeLanguageButton = "Verander Taal",
    };

    private static readonly UIStrings _uiSw = new UIStrings
    {
        welcomeTitle = "Tathmini ya PHQ-9",
        welcomeBody = "Dodoso hili linakuuliza maswali 9 kuhusu hali yako ya kihisia katika wiki 2 zilizopita.",
        startButton = "Anza Tathmini",
        questionFormat = "SWALI {0} KATI YA {1}",
        progressFormat = "{0}% Imekamilika",
        backButton = "Rudi",
        nextButton = "Swali Linalofuata",
        viewReportButton = "Tazama Ripoti",
        resultsTotalLabel = "Jumla ya Alama za PHQ-9",
        reportTitle = "Tathmini ya Unyogovu ya PHQ-9",
        reportResponsesHeading = "Majibu",
        reportColQuestion = "Swali",
        reportColAnswer = "Jibu",
        reportColScore = "Alama",
        reportGuidanceLabel = "Mwongozo",
        languagePanelTitle = "Chagua Lugha",
        changeLanguageButton = "Badilisha Lugha",
    };

    // ─── Public API ───────────────────────────────────────────────────────────

    public static string GetQuestionText(int index, string language = "en")
    {
        var q = Questions[index];
        return language switch
        {
            "zu" => q.zulu,
            "af" => q.afrikaans,
            "sw" => q.swahili,
            _ => q.english,
        };
    }

    public static string GetAnswerLabel(int index, string language = "en")
    {
        var labels = language switch
        {
            "zu" => _answersZu,
            "af" => _answersAf,
            "sw" => _answersSw,
            _ => _answersEn,
        };
        return labels[index];
    }

    public static string GetSeverity(int score, string language = "en")
    {
        var tiers = language switch
        {
            "zu" => _severityZu,
            "af" => _severityAf,
            "sw" => _severitySw,
            _ => _severityEn,
        };
        return tiers[SeverityIndex(score)];
    }

    public static string GetGuidance(int score, string language = "en")
    {
        var msgs = language switch
        {
            "zu" => _guidanceZu,
            "af" => _guidanceAf,
            "sw" => _guidanceSw,
            _ => _guidanceEn,
        };
        return msgs[SeverityIndex(score)];
    }

    public static UIStrings GetUIStrings(string language = "en")
    {
        return language switch
        {
            "zu" => _uiZu,
            "af" => _uiAf,
            "sw" => _uiSw,
            _ => _uiEn,
        };
    }

    private static int SeverityIndex(int score)
    {
        if (score == 0) return 0;
        if (score <= 4) return 1;
        if (score <= 9) return 2;
        if (score <= 14) return 3;
        if (score <= 19) return 4;
        return 5;
    }
}
