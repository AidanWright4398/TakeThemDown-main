using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] string gamePlayScene;

    [SerializeField] Button startButton, optionButton, optionCloseButton, tutorialButton,tutorialCloseButton, quitButton;

    // Start is called before the first frame update
    void Start()
    {
        optionPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        startButton.Select();

        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(OpenOptionPanel);
        optionCloseButton.onClick.AddListener(CloseOptionPanel);
        tutorialButton.onClick.AddListener(OpenTutorialPanel);
        tutorialCloseButton.onClick.AddListener(CloseTutorialPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        if (optionPanel.activeInHierarchy)
            return;
        if (tutorialPanel.activeInHierarchy)
            return;

        SceneManager.LoadScene(gamePlayScene);
    }

    void OpenOptionPanel()
    {
        optionCloseButton.Select();
        optionPanel.SetActive(true);
    }

    void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
        optionButton.Select();
    }

    void OpenTutorialPanel()
    {
        tutorialCloseButton.Select();
        tutorialPanel.SetActive(true);
    }

    void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
        optionButton.Select();
    }
    void QuitGame()
    {
        Application.Quit();
    }
}
