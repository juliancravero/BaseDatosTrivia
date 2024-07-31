using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using Postgrest.Models;

public class RankingManager : MonoBehaviour
{
    string supabaseUrl = "https://qevvgnrxwpplmroedayg.supabase.co"; // Completa con tu URL
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFldnZnbnJ4d3BwbG1yb2VkYXlnIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTcyMTE4MzMwOSwiZXhwIjoyMDM2NzU5MzA5fQ.-6ROUTg9gHK2Y_3ao98PFSyyAn06tTp4mu02aQ_M0ws"; // Completa con tu clave

    Supabase.Client clientSupabase;

    [SerializeField] private TMP_Text rankingText; 
    
    private DatabaseManager databaseManager;

    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await LoadRankings();
    }

    async Task LoadRankings()
    {
        var response = await clientSupabase
            .From<puntos>()
            .Select("nombre_usuario, puntos")
            //.Order("Puntos", Ordering.Descending)// Usa 'ascending: false' para ordenar en orden descendente
            .Get();

        if (response != null && response.Models != null)
        {
            List<puntos> rankings = response.Models;
            DisplayRankings(rankings);
        }
        else
        {
            Debug.Log("No rankings data received.");
        }
    }


    void DisplayRankings(List<puntos> rankings)
    {
        if (rankings == null || rankings.Count == 0)
        {
            rankingText.text = "No rankings available.";
            Debug.Log("No rankings data received.");
            return;
        }

        rankingText.text = "Ranking:\n";
        foreach (var score in rankings)
        {
            rankingText.text += $"{score.NombreUsuario}: {score.Puntos}\n";
            Debug.Log($"Displaying ranking: {score.NombreUsuario} - {score.Puntos}");
        }
    }

}

