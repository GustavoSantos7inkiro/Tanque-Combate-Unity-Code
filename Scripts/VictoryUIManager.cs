using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Para carregar a cena novamente

public class VictoryUIManager : MonoBehaviour
{
    public GameObject victoryPanel;    // Referência para o painel de vitória
    public Text victoryText;           // Referência para o texto de vitória
    public Button restartButton;       // Referência para o botão de restart

    void Start()
    {
        // Certifique-se de que o painel de vitória começa desativado
        victoryPanel.SetActive(false);
    }

    // Método para exibir a vitória
public void ShowVictory(GameObject tank)
{
    // Verifica o nome do tanque para determinar o vencedor
    if (tank.name == "Tank") // Nome do seu objeto tanque
    {
        victoryText.text = "Adam Wins!";
    }
    else
    {
        victoryText.text = "Smith Wins!";
    }

    // Exibe o painel de vitória
    victoryPanel.SetActive(true);  // Supondo que você tenha um painel de vitória no UI
}
    void RestartGame()
    {
        // Reinicia a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}