using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Supabase;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public List<question> responseList;
    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;
    public List<string> _answers = new List<string>();
    public bool queryCalled;

    private int _points;
    private int _maxAttempts = 10;
    public int _numQuestionAnswered = 0;
    private int _totalQuestions = 10;
    private bool _isGameOver = false;

    string _correctAnswer;
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private TextMeshProUGUI timerText;
    private float _timeLimit = 10f;
    private float _timeRemaining;

    private DatabaseManager databaseManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartTrivia();
        queryCalled = false;

        string playerName = PlayerPrefs.GetString("PlayerName");
        Debug.Log("Player Name: " + playerName);

        _numQuestionAnswered = 0;
        _isGameOver = false;
        _points = 0;
        UpdatePointsText();

        databaseManager = FindObjectOfType<DatabaseManager>();
    }

    void StartTrivia()
    {
        // Cargar la trivia desde la base de datos
    }

    public void CategoryAndQuestionQuery(bool isCalled)
    {
        isCalled = UIManagment.Instance.queryCalled;

        if (!isCalled && !_isGameOver)
        {
            randomQuestionIndex = Random.Range(0, responseList.Count);
            _correctAnswer = responseList[randomQuestionIndex].CorrectOption;

            _answers.Clear();
            _answers.Add(responseList[randomQuestionIndex].Answer1);
            _answers.Add(responseList[randomQuestionIndex].Answer2);
            _answers.Add(responseList[randomQuestionIndex].Answer3);

            _answers.Shuffle();

            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];
                int index = i;
                UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
            }

            UIManagment.Instance.queryCalled = true;
            StartCoroutine(TimerCoroutine());
        }
    }

    private IEnumerator TimerCoroutine()
    {
        _timeRemaining = _timeLimit;
        while (_timeRemaining > 0 && !_isGameOver)
        {
            _timeRemaining -= Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }

        if (!_isGameOver)
        {
            HandleTimeOut();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Tiempo: " + Mathf.Ceil(_timeRemaining).ToString();
        }
    }

    public void HandleAnswer(bool isCorrect)
    {
        if (_isGameOver) return;

        StopCoroutine(TimerCoroutine());
        if (isCorrect)
        {
            int score = Mathf.Max(0, 10 - (int)(_timeLimit - _timeRemaining));
            _points += Mathf.Clamp(score, 0, 10);
            _numQuestionAnswered++;
            UpdatePointsText();

            if (_numQuestionAnswered >= _totalQuestions)
            {
                EndGame("¡Felicidades! Has respondido todas las preguntas.");
            }
            else
            {
                Invoke("NextQuestion", 2f);
            }
        }
        else
        {
            EndGame("Juego terminado. Respuesta incorrecta.");
        }
    }

    private void HandleTimeOut()
    {
        EndGame("Juego terminado. Tiempo agotado.");
    }

    [SerializeField] private GameObject endGameBG;
    private void EndGame(string message)
    {
        _isGameOver = true;
        if(endGameBG != null)
        {
            endGameBG.SetActive(true);
        }
        if (endGameText != null)
        {
            endGameText.text = message;
        }
        SaveScore();
    }

    private void UpdatePointsText()
    {
        if (pointsText != null)
        {
            pointsText.text = "Puntos: " + _points;
        }
    }

    private void NextQuestion()
    {
        UIManagment.Instance.queryCalled = false;
    }

    private void Update()
    {
        
    }
    private async void SaveScore()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");
        var scoreData = new puntos
        {
            NombreUsuario = playerName,
            Puntos = _points
        };

        var response = await databaseManager.SaveScoreToSupabase(scoreData);
        if (response)
        {
            Debug.Log("Puntaje guardado exitosamente.");
        }
        else
        {
            Debug.LogError("Error al guardar el puntaje en Supabase.");
        }
    }

}
