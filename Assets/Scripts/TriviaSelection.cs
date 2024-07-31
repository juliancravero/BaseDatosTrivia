using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TriviaSelection : MonoBehaviour
{
    string supabaseUrl = "https://qevvgnrxwpplmroedayg.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFldnZnbnJ4d3BwbG1yb2VkYXlnIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTcyMTE4MzMwOSwiZXhwIjoyMDM2NzU5MzA5fQ.-6ROUTg9gHK2Y_3ao98PFSyyAn06tTp4mu02aQ_M0ws"; //COMPLETAR

    Supabase.Client clientSupabase;

    List<trivia> trivias = new List<trivia>();
    [SerializeField] TMP_Dropdown _dropdown;

    public DatabaseManager databaseManager;

    async void Start()
    {
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        await SelectTrivias();
        PopulateDropdown();
    }

    async Task SelectTrivias()
    {
        var response = await clientSupabase
            .From<trivia>()
            .Select("*")
            .Get();

        if (response != null)
        {
            trivias = response.Models;
            //Debug.Log("Trivias seleccionadas: " + trivias.Count);
            //foreach (var trivia in trivias)
            //{
            //    Debug.Log("ID: " + trivia.id + ", Categor�a: " + trivia.category);
            //}
        }

    }

    void PopulateDropdown()
    {
        
        _dropdown.ClearOptions();

        List<string> categories = new List<string>();

        foreach (var trivia in trivias)
        {
            categories.Add(trivia.category);
        }

        _dropdown.AddOptions(categories);
    }

    public void OnStartButtonClicked()
    {
        int selectedIndex = _dropdown.value;
        string selectedTrivia = _dropdown.options[selectedIndex].text;

        PlayerPrefs.SetInt("SelectedIndex", selectedIndex+1);
        PlayerPrefs.SetString("SelectedTrivia", selectedTrivia);


        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
