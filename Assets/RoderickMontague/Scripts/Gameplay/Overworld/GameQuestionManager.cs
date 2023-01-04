using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;
using System.ComponentModel.Design.Serialization;

// Namespace.
namespace RM_BBTS
{
    // The system for posing questions in the overworld.
    public class GameQuestionManager : MonoBehaviour
    {
        // The gameplay manager.
        public GameplayManager gameManager;

        // TODO: hide question when in a tutorial.
        // The object of the question window.
        public GameObject questionObject;

        // The title text of the question window.
        public TMP_Text titleText;

        // The amount of questions that have been asked to the player.
        public int questionsAsked = 0;

        // The amount of questions answered correctly.
        public int questionsCorrect = 0;

        [Header("Question Info")]

        // Gets set to 'true' if the question is being asked.
        private bool running = false;

        // Gets set to 'true' if the question has been answered (does not specify if correct or incorrect).
        private bool responded = false;

        // The question text.
        public TMP_Text questionText;

        // The loaded question.
        public GameQuestion currentQuestion;

        // Taken out since it doesn't get initialized in time.
        // private GameQuestions gameQuestions;

        
        // The button text for the four responses.
        // Response 0
        public Button response0Button;
        public GameObject response0Fill;
        public TMP_Text response0Text;

        // Response 1
        public Button response1Button;
        public GameObject response1Fill;
        public TMP_Text response1Text;

        // Respone 2
        public Button response2Button;
        public GameObject response2Fill;
        public TMP_Text response2Text;

        // Response 3
        public Button response3Button;
        public GameObject response3Fill;
        public TMP_Text response3Text;

        // The list of response button.
        // private Button[] responseButtons;

        // The number of the selected response.
        private int selectedResponse = -1;

        // The text for evaluation.
        public TMP_Text evaluationText;

        // The text for the score addition.
        public TMP_Text scorePlusText;

        [Header("Score Plus/Time")]
        // The maximum and minimum for adding to the player's score.
        public int maxScorePlus = 150;
        public int minScorePlus = 5;

        // Gets set to 'true' when the timer is paused.
        public bool pausedTimer = true;

        // What the timer starts at.
        public float startTime = 15.0F;

        // When the time falls below this value, the reward is reduced.
        public float reduceRewardTime = 10.0F;

        // The timer for the game.
        public float timer = 0.0F;

        [Header("Evaluation")]
        // The confirm button.
        public Button confirmButton;
        // The confirm button text.
        public TMP_Text confirmButtonText;

        // The next button.
        public Button finishButton;
        // The finish button text.
        public TMP_Text finishButtonText;

        // The correct and incorrect string.
        private string correctString = "[Correct]";
        private string incorrectString = "[Incorrect]";

        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            // Load question 0 to reset the question manager.
            LoadQuestion(GameQuestions.Instance.GetQuestion(0));
        }

        // Start is called before the first frame update
        void Start()
        {
            // Translation.
            JSONNode defs = SharedState.LanguageDefs;

            // Translate text.
            if(defs != null)
            {
                questionText.text = defs["kwd_questionTime"];
                correctString = defs["kwd_correct"];
                incorrectString = defs["kwd_incorrect"];
                confirmButtonText.text = defs["kwd_confirm"];
                finishButtonText.text = defs["kwd_finish"];
            }
        }

        // Returns the current question.
        public GameQuestion CurrentQuestion
        {
            get { return currentQuestion; }
        }

        // Gets the index of the answer to the loaded question.
        public int AnswerIndex
        { 
            get { return currentQuestion.answerIndex; }
        }

        // Returns 'true' if a question is being asked.
        public bool QuestionIsRunning()
        {
            return running;
        }

        // Returns the answer for the loaded question.
        public string GetAnswer()
        {
            return currentQuestion.GetAnswer();
        }

        // Loads the question into the question manager.
        public void LoadQuestion(GameQuestion question)
        {
            // Sets the question.
            questionText.text = question.question;

            // Sets the response text, and determines if the option is available.
            // Response 0
            response0Text.text = question.Response0;
            response0Button.gameObject.SetActive(question.Response0 != string.Empty);

            // Response 1
            response1Text.text = question.Response1;
            response1Button.gameObject.SetActive(question.Response1 != string.Empty);

            // Response 2
            response2Text.text = question.Response2;
            response2Button.gameObject.SetActive(question.Response2 != string.Empty);

            // Response 3
            response3Text.text = question.Response3;
            response3Button.gameObject.SetActive(question.Response3 != string.Empty);

            {
                // Checks if at least one of the buttons is available.
                bool available = false;

                // Makes an array for this question.
                Button[] responseButtons = new Button[] { response0Button, response1Button, response2Button, response3Button };

                // Checks each button.
                foreach (Button rb in responseButtons)
                {
                    // If one of the buttons is active, stop the check.
                    if (rb.gameObject.activeSelf)
                    {
                        available = true;
                        break;
                    }
                }

                // If no buttons are available, activate all of them.
                if(!available)
                {
                    // Active all the buttons.
                    foreach (Button rb in responseButtons)
                        rb.gameObject.SetActive(true);
                }
            }


            // Deselects the responses.
            DeselectAllResponses();

            // Clear out the two pieces of text.
            evaluationText.text = string.Empty;
            scorePlusText.text = string.Empty;

            // Turns off the buttons.
            confirmButton.interactable = false;
            finishButton.interactable = false;

            // Stop the timer since no question is running yet.
            pausedTimer = true;
            timer = startTime;
        }

        // Loads a random question.
        public void LoadRandomQuestion()
        {
            // TODO: make sure that the same question can't be returned.

            LoadQuestion(GameQuestions.Instance.GetRandomQuestion(true));
        }

        // Asks the loaded question.
        public void AskQuestion()
        {
            // The question is running now, and no response has been given yet.
            running = true;
            responded = false;
            selectedResponse = -1;

            // A question has been asked.
            questionsAsked++;

            // Open the prompt.
            questionObject.SetActive(true);

            // Start the timer.
            timer = startTime;
            pausedTimer = false;

            // Call to signify that a question has been asked.
            gameManager.OnQuestionStart();
        }

        // Ask the new question, loading said question into the game.
        public void AskQuestion(GameQuestion newQuestion)
        {
            LoadQuestion(newQuestion);
            AskQuestion();
        }

        // Loads a random question and then asks it.
        public void AskRandomQuestion()
        {
            LoadRandomQuestion();
            AskQuestion();
        }

        // Deselects all the other responses.
        public void DeselectAllResponses()
        {
            // Turn off the other responses.
            response0Fill.SetActive(false);
            response1Fill.SetActive(false);
            response2Fill.SetActive(false);
            response3Fill.SetActive(false);
        }

        // Selects a response.
        private void SelectResponse(int response)
        {
            // Deselects the other responses.
            DeselectAllResponses();

            // Selects the response.
            selectedResponse = Mathf.Clamp(response, 0, GameQuestions.QUESTION_OPTIONS_MAX - 1);
            
            // Checks which response to select.
            switch (selectedResponse)
            {
                default:
                case 0:
                    response0Fill.SetActive(true); 
                    break;
                case 1:
                    response1Fill.SetActive(true);
                    break;
                case 2:
                    response2Fill.SetActive(true);
                    break;
                case 3:
                    response3Fill.SetActive(true);
                    break;

            }

            // Make the button interactable.
            if (!confirmButton.interactable)
                confirmButton.interactable = true;
        }

        // Select response 0.
        public void SelectResponse0()
        {
            SelectResponse(0);
        }

        // Select response 1.
        public void SelectResponse1()
        {
            SelectResponse(1);
        }

        // Select response 2.
        public void SelectResponse2()
        {
            SelectResponse(2);
        }

        // Select response 3.
        public void SelectResponse3()
        {
            SelectResponse(3);
        }

        // Disables all the response buttons.
        public void DisableAllResponseButtons()
        {
            response0Button.interactable = false;
            response1Button.interactable = false;
            response2Button.interactable = false;
            response3Button.interactable = false;
        }

        // Enables all the response buttons.
        public void EnableAllResponseButtons()
        {
            response0Button.interactable = true;
            response1Button.interactable = true;
            response2Button.interactable = true;
            response3Button.interactable = true;
        }

        // Confirms the user's inputted response.
        public void ConfirmResponse()
        {
            // The question has been responded to.
            responded = true;

            // Gets the correct response.
            bool correct = currentQuestion.CorrectAnswer(selectedResponse);

            // Shows the evaluation text.
            evaluationText.text = (correct) ? correctString : incorrectString;

            // The maximum score addition to be made.
            int scorePlus = 0;

            // Checks if the user got the question right or not.
            if(correct)
            {
                // Pause the timer, and increment the question correct variable.
                pausedTimer = true;
                questionsCorrect++;

                // Adjust score plus.
                scorePlus = Mathf.RoundToInt(maxScorePlus * (timer / reduceRewardTime));
                scorePlus = Mathf.Clamp(scorePlus, minScorePlus, maxScorePlus);
            }
            else
            {
                // TODO: should the player lose points for getting the question wrong?
                scorePlus = 0;
            }

            // Add to the score, and display it.
            gameManager.score += scorePlus;
            gameManager.overworld.UpdateUI(); // Updates the UI to display the new score.
            scorePlusText.text = (scorePlus > 0) ? scorePlus.ToString("#+;#-;0") : "-";

            // Response locked in.
            confirmButton.interactable = false;

            // Question can now be finished/closed.
            finishButton.interactable = true;
        }

        // End the question.
        public void FinishQuestion()
        {
            // The question is no longer running, and thus no response has been given.
            running = false;
            responded = false;

            // Set selected response to default.
            selectedResponse = -1;

            // Turn off the question object.
            questionObject.SetActive(!questionObject.activeSelf);

            // Call to signify that a question has been ended.
            gameManager.OnQuestionEnd();
        }

        // Resets the amount of asked questions.
        public void ResetAskedQuestionCount()
        {
            questionsAsked = 0;
            questionsCorrect = 0;
        }

        // Disables the question (prevents all interaction from the user).
        public void DisableQuestion()
        {
            // Disable the responses (some may not be visible anyway).
            DisableAllResponseButtons();

            // Disable the lock-in options.
            confirmButton.interactable = false;
            finishButton.interactable = false;

            // Pause the timer.
            pausedTimer = true;
        }

        // Enables the question (enables interaction from the user).
        public void EnableQuestion()
        {
            // Enable the responses (some may not be visible anyway).
            EnableAllResponseButtons();

            // Checks if the user has responded to the question.
            if(responded)
            {
                // If the user responded, only the finish button should be enabled.
                confirmButton.interactable = false;
                finishButton.interactable = true;
            }    
            else
            {
                // If the user hasn't responded yet, only the confirm button should be enabled.
                // If the player hasn't selected a response yet, don't make the confirm button interactable.
                confirmButton.interactable = (selectedResponse >= 0);
                finishButton.interactable = false;

                // Unpause the timer if the question is running.
                if (running)
                    pausedTimer = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // If the timer isn't paused, count down.
            if (!pausedTimer)
            {
                // Reduce the timer.
                timer -= Time.deltaTime;

                // Timer is less than 0, so set it to 0.
                if (timer <= 0.0F)
                    timer = 0.0F;

                // Don't count down anymore.
                if (timer == 0.0F)
                    pausedTimer = true;

            }
                
        }
    }
}