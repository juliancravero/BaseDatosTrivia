using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RankingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI triviaRankingText;
    [SerializeField] private TextMeshProUGUI generalRankingText;
    [SerializeField] private TextMeshProUGUI averagePointsText;
    [SerializeField] private TextMeshProUGUI userCountText;
    [SerializeField] private Button backButton;

    private DatabaseManager databaseManager;

    private void Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
       // ShowTriviaRanking();
        ShowGeneralRanking();
        ShowStatistics();
    }

    /*private async void ShowTriviaRanking()
    {
        int selectedTriviaId = PlayerPrefs.GetInt("SelectedIndex");
        var scores = await databaseManager.GetRankingByTrivia(selectedTriviaId);
        DisplayScores(scores, triviaRankingText);
    }*/

    private async void ShowGeneralRanking()
    {
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager is not initialized.");
            return;
        }
        var scores = await databaseManager.GetGeneralRanking();
        DisplayScores(scores, generalRankingText);
    }

    private void DisplayScores(List<puntos> scores, TextMeshProUGUI textComponent)
    {
        textComponent.text = "";
        foreach (var score in scores)
        {
            textComponent.text += $"{score.NombreUsuario}: {score.Puntos}\n";
        }
    }

    public async void ShowStatistics()
    {
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager is not initialized.");
            return;
        }
        float averagePoints = await databaseManager.GetAveragePoints();
        Debug.Log("avg "+averagePoints);
        int userCount = await databaseManager.GetUserCount();

        // Actualizar los textos de UI
        if (averagePointsText != null)
        {
            averagePointsText.text = $"Promedio de puntos: {averagePoints:F2}";
        }
  
        
        if (userCountText != null)
        {
            userCountText.text = $"Cantidad de usuarios: {userCount}";
        }
        else
        {
            Debug.LogError("userCountText is not assigned.");
        }
    }



    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }
}

