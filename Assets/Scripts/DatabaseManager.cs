using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;


public class DatabaseManager : MonoBehaviour
{
    string supabaseUrl = "https://qevvgnrxwpplmroedayg.supabase.co"; 
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFldnZnbnJ4d3BwbG1yb2VkYXlnIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTcyMTE4MzMwOSwiZXhwIjoyMDM2NzU5MzA5fQ.-6ROUTg9gHK2Y_3ao98PFSyyAn06tTp4mu02aQ_M0ws"; //COMPLETAR

    Supabase.Client clientSupabase;

    public int index;

    //UI

    [SerializeField] private TMP_Text rankingText;


    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);
        
        index = PlayerPrefs.GetInt("SelectedIndex");

        //print(_selectedTrivia);

        await LoadTriviaData(index);
    }

    async Task LoadTriviaData(int index)
    {
        var response = await clientSupabase
            .From<question>()
            .Where(question => question.trivia_id == index)
            .Select("id, question, answer1, answer2, answer3, correct_answer, trivia_id, trivia(id, category)")
            .Get();

        GameManager.Instance.currentTriviaIndex = index;

        GameManager.Instance.responseList = response.Models;

        print("Response from query: "+ response.Models.Count);
        print("ResponseList from GM: "+ GameManager.Instance.responseList.Count);  
    }

    public async Task<bool> SaveScoreToSupabase(puntos scoreData)
    {
        var response = await clientSupabase
            .From<puntos>()
            .Insert(new List<puntos> { scoreData });

        return response != null && response.Models != null && response.Models.Count > 0;
    }


    /*public async Task<List<Score>> GetRankingByTrivia(int triviaId)
    {
        var response = await clientSupabase
            .From<Score>()
            .Select("NombreUsuario, Puntos")
            .Where(score => score.TriviaId == triviaId) // Asegúrate de ajustar el campo correctamente
            //.Order("Puntos", SortOrder.Descending)
            .Get();

        return response.Models;
    }*/

    public async Task<List<puntos>> GetGeneralRanking()
    {
        var response = await clientSupabase
            .From<puntos>()
            .Select("nombre_usuario, puntos")
            //.Order("Puntos", SortOrder.Descending)
            .Get();

        return response.Models;
    }

    public async Task<float> GetAveragePoints()
    {
        // Consulta para obtener todos los puntos
        var response = await clientSupabase
            .From<puntos>()
            .Select("puntos")
            .Get();

        if (response == null || response.Models == null || response.Models.Count == 0)
        {
            return 0;
        }

        float totalPoints = 0;
        foreach (var score in response.Models)
        {
            totalPoints += score.Puntos;
        }

        return totalPoints / response.Models.Count;
    }


    public async Task<int> GetUserCount()
    {
        var response = await clientSupabase
            .From<puntos>()
            .Select("id") // Selecciona un campo cualquiera para contar los registros
            .Get();

        return response.Models.Count;
    }

}
