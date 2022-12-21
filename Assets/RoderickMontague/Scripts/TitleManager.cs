using LoLSDK;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// the manager for the title scene.
namespace RM_BBTS
{
    // The manager for the title scene.
    public class TitleManager : MonoBehaviour
    {
        // The save text for the title manager.
        // If the user presses "save and quit", this text will be disabled on the title screen when the scene switches over.
        // This is because saving won't be done by the time the scene switches, so make sure this text is set.
        public TMP_Text saveFeedbackText;

        // The name of the game scene.
        public const string GAME_SCENE_NAME = "GameScene";

        [Header("Main Menu")]
        // Menu
        public GameObject mainMenu;

        // Start
        public Button newGameButton;
        public TMP_Text newGameButtonText;
        public Button continueButton;
        public TMP_Text continueButtonText;

        // Controls
        public GameObject controlsMenu;
        public TMP_Text controlsButtonText;

        // Settings
        public GameObject settingsMenu;
        public TMP_Text settingsButtonText;

        // Copyright
        public GameObject creditsMenu;
        public TMP_Text creditsButtonText;

        [Header("Controls Submenu")]
        // The controls title text.
        public TMP_Text controlsTitleText;

        // The text for the controls description.
        public TMP_Text controlsInstructText;

        // The text for the controls description.
        public TMP_Text controlsDescText;

        // THe key for the description text.
        private string controlsDescTextKey = "mnu_controls_desc";

        // The back button text for the controls sebmenu.
        public TMP_Text controlsBackButtonText;

        [Header("Animations")]
        // If 'true', transition animations are used.
        public bool useTransitions = true;

        // The transition object.
        public SceneTransition sceneTransition;

        // Awake is called when the script instance is loaded.
        private void Awake()
        {
            // Checks if LOL SDK has been initialized.
            GameSettings settings = GameSettings.Instance;

            // Gets an instance of the LOL manager.
            LOLManager lolManager = LOLManager.Instance;

            // Language
            JSONNode defs = SharedState.LanguageDefs;

            // Translate text.
            if(defs != null)
            {
                // Main Menu
                // startButtonText.text = defs["kwd_start"];
                newGameButtonText.text = defs["kwd_newGame"];
                continueButtonText.text = defs["kwd_continue"];

                controlsButtonText.text = defs["kwd_controls"];
                settingsButtonText.text = defs["kwd_settings"];
                creditsButtonText.text = defs["kwd_licenses"];

                // Controls Menu
                controlsTitleText.text = defs["kwd_controls"];
                controlsInstructText.text = defs["mnu_controls_instruct"];
                controlsDescText.text = defs[controlsDescTextKey];
                controlsBackButtonText.text = defs["kwd_back"];

            }

            // Use the tutorial for the game.
            settings.UseTutorial = true;

            // Checks for initialization
            if (!settings.InitializedLOLSDK)
            {
                Debug.LogError("LOL SDK NOT INITIALIZED.");

                // Do not allow the button to be used.
                continueButton.interactable = false;
                
                settings.AdjustAllAudioLevels();
            }
            else
            {
                // NOTE: the buttons disappear for a frame if there is no save state.
                // It doesn't effect anything, but it's jarring visually.
                // Initialize the game.
                if(newGameButton != null && continueButton != null)
                    lolManager.saveSystem.Initialize(newGameButton, continueButton);


                // Enables/disables the continue button based on if there is loaded data or not.
                continueButton.interactable = lolManager.saveSystem.HasLoadedData();

                // LOLSDK.Instance.SubmitProgress();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //  SceneManager.LoadScene("ResultsScene");

            // Sets the save text.
            if (saveFeedbackText != null && LOLSDK.Instance.IsInitialized)
            {
                saveFeedbackText.text = string.Empty;
                LOLManager.Instance.saveSystem.feedbackText = saveFeedbackText;
            }
            else
            {
                // Just empty out the string.
                saveFeedbackText.text = string.Empty;
            }
        }

        // Starts the game (general function for moving to the GameScene).
        public void StartGame()
        {
            // If transitions should be used, do a delayed game start.
            if (useTransitions)
                sceneTransition.LoadScene(GAME_SCENE_NAME);
            else
                SceneManager.LoadScene(GAME_SCENE_NAME);

        }

        // Starts a new game.
        public void StartNewGame()
        {
            // Clear out the loaded data if the LOLSDK has been initialized.
            if(LOLSDK.Instance.IsInitialized)
                LOLManager.Instance.saveSystem.loadedData = null;

            StartGame();
        }

        // Continues a saved game.
        public void ContinueGame()
        {
            // Checks if there is loaded data.
            if(!LOLManager.Instance.saveSystem.HasLoadedData()) // No data.
            {
                Debug.LogWarning("No save data found. New game to be loaded.");
                StartNewGame();
            }
            else // Loaded data will be inplemented by the gameplay manager when entering the scene.
            {
                StartGame();
            }            
        }

        // Toggles the controls menu.
        public void ToggleControlsMenu()
        {
            bool active = !controlsMenu.gameObject.activeSelf;
            controlsMenu.gameObject.SetActive(active);
            mainMenu.gameObject.SetActive(!active);

            // If the controls menu has been opened.
            if(active)
            {
                // Play the controls description.
                if(LOLSDK.Instance.IsInitialized && GameSettings.Instance.UseTextToSpeech && controlsDescTextKey != "")
                {
                    // Voice the text.
                    LOLManager.Instance.textToSpeech.SpeakText(controlsDescTextKey);
                }
            }
        }

        // Toggles the settings menu.
        public void ToggleSettingsMenu()
        {
            settingsMenu.gameObject.SetActive(!settingsMenu.gameObject.activeSelf);
            mainMenu.gameObject.SetActive(!mainMenu.gameObject.activeSelf);
        }

        // Toggles the credits menu.
        public void ToggleCreditsMenu()
        {
            creditsMenu.gameObject.SetActive(!creditsMenu.gameObject.activeSelf);
            mainMenu.gameObject.SetActive(!mainMenu.gameObject.activeSelf);
        }

        // Clears out the save.
        // TODO: This is only for testing, and the button for this should not be shown in the final game.
        public void ClearSave()
        {
            LOLManager.Instance.saveSystem.lastSave = null;
            LOLManager.Instance.saveSystem.loadedData = null;

            continueButton.interactable = false;
        }

        // Update is called once per frame
        void Update()
        {
            // The transitions block the UI input, so these buttons cannot be pressed once the scene starts transitioning.
            // Makes sure the new game button is kept on when the save system turns it off.
            if(!newGameButton.gameObject.activeSelf)
                newGameButton.gameObject.SetActive(true);

            // Makes sure the continue button is kept on when the save system turns it off.
            if (!continueButton.gameObject.activeSelf)
                continueButton.gameObject.SetActive(true);
        }
    }
}