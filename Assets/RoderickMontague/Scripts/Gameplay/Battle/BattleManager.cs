using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RM_BBTS
{
    // Manages the battle operations for the game. This becomes active when the game enters a battle state.
    public class BattleManager : GameState
    {
        // Becomes 'true' when the overworld is initialized.
        public bool initialized = false;

        // the manager for the game.
        public GameplayManager gameManager;

        // The door
        public Door door;

        // The textbox
        public TextBox textBox;

        // The prompt for asking the player about the treasure.
        public GameObject treasurePrompt;

        // The panel for learning a new move.
        public LearnMove learnMovePanel;

        [Header("Battle")]

        // The player.
        public Player player;

        // The opponent for the player.
        // TODO: should a new opponent be generated everytime? Should really just cycle through some pre-build objects.
        public BattleEntity opponent;

        // The sprite for the opponent.
        public SpriteRenderer opponentSprite;

        // Base objects that are activated for battle.
        public Enemy enemyBase;
        public Treasure treasureBase;
        public Boss bossBase;

        // [Header("Battle/Mechanics")]
        // // The move the player has selected.
        // public Move playerMove;
        // 
        // // The move the opponent has selected.
        // public Move opponentMove;

        // The chance of learning a new move.
        private float NEW_MOVE_CHANCE = 0.80F;

        // Becomes 'true' when the battle end state has been initialized.
        private bool initBattleEnd = false;

        [Header("UI")]
        // The user interface.
        public GameObject ui;

        // The turn text. Each entry is a different page.
        public List<Page> turnText;

        // The player's move page.
        public Page playerMovePage;

        // The opponent's move page.
        public Page opponentMovePage;

        [Header("UI/Player")]

        // Move 0 (index 0) button.
        [Tooltip("The button for using Player Move 0, which is at index [0].")]
        public Button move0Button;
        public TMP_Text move0Text;

        // Move 1 (index 1) button.
        [Tooltip("The button for using Player Move 1, which is at index [1].")]
        public Button move1Button;
        public TMP_Text move1Text;

        // Move 2 (index 2) button.
        [Tooltip("The button for using Player Move 2, which is at index [2].")]
        public Button move2Button;
        public TMP_Text move2Text;

        // Move 3 (index 3) button.
        [Tooltip("The button for using Player Move 3, which is at index [3].")]
        public Button move3Button;
        public TMP_Text move3Text;

        // Charge Button
        public Button chargeButton;

        // Run Button
        public Button runButton;

        [Header("UI/Opponent")]

        // The health bar for the opponent.
        public ProgressBar opponentHealthBar;

        // TODO: this will not be shown n the final game.
        public TMP_Text opponentHealthText;

        // Start is called before the first frame update
        void Start()
        {
            // // enemy base not set, so make a base.
            // if(enemyBase == null)
            // {
            //     GameObject go = new GameObject("Enemy Base");
            //     enemyBase = go.AddComponent<Enemy>();
            //     go.transform.parent = gameObject.transform;
            // }
            // 
            // // treasure base not set, so make a base.
            // if (treasureBase == null)
            // {
            //     GameObject go = new GameObject("Treasure Base");
            //     treasureBase = go.AddComponent<Treasure>();
            //     go.transform.parent = gameObject.transform;
            // }
            // 
            // // enemy base not set, so make a base.
            // if (bossBase == null)
            // {
            //     GameObject go = new GameObject("Boss Base");
            //     bossBase = go.AddComponent<Boss>();
            //     go.transform.parent = gameObject.transform;
            // }

            // Turns off the bases.
            enemyBase.gameObject.SetActive(false);
            treasureBase.gameObject.SetActive(false);
            bossBase.gameObject.SetActive(false);

            // Initializes the list.
            turnText = new List<Page>();

            // When the textbox disappears the turn is over, so call this function.
            textBox.OnTextBoxFinishedAddCallback(OnTurnOver);

            // Close the textbox when the player is done.
            textBox.closeOnEnd = true;

            // Hide prompt.
            treasurePrompt.gameObject.SetActive(false);

            // Turns off the learn move panel.
            learnMovePanel.windowObject.SetActive(false);
        }

        // This function is called when the object becomes enabled and active
        private void OnEnable()
        {
            if (ui != null)
                ui.SetActive(true);
        }

        // This function is called when the behaviour becomes disabled or inactive
        private void OnDisable()
        {
            if (ui != null)
                ui.SetActive(false);
        }

        // Initializes the overworld.
        public override void Initialize()
        {
            initialized = true;

            // Sets the battle entity from the door.
            // opponent = null; // TODO: comment out.

            // Checks the type of entity.
            switch (door.battleEntity.id)
            {
                case battleEntityId.treasure: // treasure
                    opponent = treasureBase;
                    break;
                case battleEntityId.boss: // boss
                    opponent = bossBase;
                    break;
                default: // enemy
                    opponent = enemyBase;
                    break;

            }

            // Opponent has been set.
            if (opponent != null)
            {
                opponent.LoadBattleData(door.battleEntity);
                opponentSprite.sprite = opponent.sprite;
            }


            // Checks move activity to see if the player can use it or not.
            // Also changes the move name on the display.

            // Move 0
            // move0Button.interactable = player.Move0 != null;
            move0Text.text = (player.Move0 != null) ? player.Move0.Name : "-";

            // Move 1
            // move1Button.interactable = player.Move1 != null;
            move1Text.text = (player.Move1 != null) ? player.Move1.Name : "-";

            // Move 2
            // move2Button.interactable = player.Move2 != null;
            move2Text.text = (player.Move2 != null) ? player.Move2.Name : "-";

            // Move 3
            // move3Button.interactable = player.Move3 != null;
            move3Text.text = (player.Move3 != null) ? player.Move3.Name : "-";

            // Checks if the player has a full charge.
            // chargeButton.interactable = !player.HasFullCharge();

            // Changes the 'interactable' toggle for the buttons.
            RefreshPlayerOptions();

            // Updates the interface.
            UpdateUI();

            // If the opponent is a treasure box.
            if (opponent is Treasure)
            {
                // The player has no battle options since this isn't a fight.
                DisablePlayerOptions();

                // Show treasure prompt.
                treasurePrompt.gameObject.SetActive(true);
            }
            else
            {
                // Hide treasure prompt.
                treasurePrompt.gameObject.SetActive(false);
            }

            // The battle has begun.
            initBattleEnd = false;
        }

        // Called when the mouse hovers over an object.
        public override void OnMouseHovered(GameObject hoveredObject)
        {
            throw new System.NotImplementedException();
        }

        // Called when the mouse interacts with an entity.
        public override void OnMouseInteract(GameObject heldObject)
        {

        }

        // Called when the user's touch interacts with an entity.
        public override void OnTouchInteract(GameObject touchedObject, Touch touch)
        {

        }

        // Called with the object that was received with the interaction.
        protected override void OnInteractReceive(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }

        // Sets player controls to interactable or not. RefreshPlayerOptions is also called to disable buttons that do nothing. 
        public void SetPlayerOptionsAvailable(bool interactable)
        {
            move0Button.interactable = interactable;
            move1Button.interactable = interactable;
            move2Button.interactable = interactable;
            move3Button.interactable = interactable;

            chargeButton.interactable = interactable;
            runButton.interactable = interactable;

            // If all were turned on, check to see if some should stay off.
            if (interactable)
                RefreshPlayerOptions();
        }

        // Enables the player options.
        public void EnablePlayerOptions()
        {
            SetPlayerOptionsAvailable(true);
        }

        // Disables the player options.
        public void DisablePlayerOptions()
        {
            SetPlayerOptionsAvailable(false);
        }

        public void RefreshPlayerOptions()
        {
            // Checks move activity to see if the player can use it or not.
            // Also changes the move name on the display.

            // Enables/disables various buttons.

            // Move 0 
            if (player.Move0 != null)
                move0Button.interactable = player.Move0.Energy <= player.Energy;
            else
                move0Button.interactable = false;

            // Move 1
            if (player.Move1 != null)
                move1Button.interactable = player.Move1.Energy <= player.Energy;
            else
                move1Button.interactable = false;

            // Move 2 
            if (player.Move2 != null)
                move2Button.interactable = player.Move2.Energy <= player.Energy;
            else
                move2Button.interactable = false;

            // Move 3
            if (player.Move3 != null)
                move3Button.interactable = player.Move3.Energy <= player.Energy;
            else
                move3Button.interactable = false;

            // Checks if the player has a full charge.
            chargeButton.interactable = !player.HasFullCharge();
        }

        // Updates the battle visuals.
        // If 'playerTurn' is true, then the update is coming from the player's turn.
        // If false, it's coming from the enemy's turn.
        private void AddVisualUpdateCallbacks(bool playerTurn)
        {
            // If there are pages to attach callbacks too.
            if (turnText.Count > 0)
            {
                if (playerTurn) // Player turn
                {
                    turnText[turnText.Count - 1].OnPageClosedAddCallback(UpdateUI);
                    turnText[turnText.Count - 1].OnPageClosedAddCallback(gameManager.UpdateUI);
                }
                else
                {
                    // turnText[turnText.Count - 1].OnPageClosedAddCallback(gameManager.UpdatePlayerHealthUI);
                    turnText[turnText.Count - 1].OnPageClosedAddCallback(gameManager.UpdateUI);
                }
            }
        }

        // Called to perform the player's move.
        private void PerformPlayerMove()
        {
            player.selectedMove.Perform(player, opponent, this);
        }

        // Called to perform the opponent's move.
        private void PerformOpponentMove()
        {
            opponent.selectedMove.Perform(opponent, player, this);
        }

        // Performs the two moves.
        public void PerformMoves()
        {
            // Both sides have selected a move.
            if (player.selectedMove != null && opponent.selectedMove != null)
            {
                // Checks who goes first.
                bool playerFirst = false;

                // TODO: account for status effects.

                // Clears out the past text.
                turnText.Clear();

                // Determines who goes first.
                if (player.Speed > opponent.Speed) // player first
                    playerFirst = true;
                else if (player.Speed < opponent.Speed) // opponent first
                    playerFirst = false;
                else // random
                    playerFirst = Random.Range(0, 2) == 1;


                // Loads the selected moves.
                // The two pages for the player and the opponent.

                // Adds the player's move.
                playerMovePage = new Page(player.displayName + " used " + player.selectedMove.Name + "!");
                playerMovePage.OnPageOpenedAddCallback(PerformPlayerMove);

                // Adds the opponent's move.
                opponentMovePage = new Page(opponent.displayName + " used " + opponent.selectedMove.Name + "!");
                opponentMovePage.OnPageOpenedAddCallback(PerformOpponentMove);

                // Places the pages in order.
                if (playerFirst)
                {
                    turnText.Add(playerMovePage);
                    turnText.Add(opponentMovePage);
                }
                else
                {
                    turnText.Add(opponentMovePage);
                    turnText.Add(playerMovePage);
                }


                // Show the textbox.
                // TODO: hide player move controls.
                textBox.ReplacePages(turnText);
                textBox.Open();

                // Disable the player options since the textbox is open.
                DisablePlayerOptions();
            }
            else
            {
                // Gets the moves from the player and the opponent.
                player.OnBattleTurn(); // does nothing right now.

                // opponent.
                opponent.OnBattleTurn(); // calculates next move (evenutally)
            }
        }


        // Called when the player attempts to run away. TODO: have the enemy's move still go off if the run fails.
        public void RunAway()
        {
            // Becomes 'true' if the run attempt was successful.
            bool success = false;

            // Overrides the selected move.
            player.selectedMove = MoveList.Instance.RunMove;

            // If there's no opponent then the player can always run away.
            if (opponent == null)
            {
                success = true;
            }
            // If there is an opponent there the player may be unable to leave.
            else
            {
                // There's a 1/2 chance of running away.
                success = (Random.Range(0, 2) == 1);
            }

            // Returns to the overworld if the run was successful.
            if (success)
                ToOverworld();
            else
                Debug.Log("Run failed.");
        }

        // Called when the turn is over.
        private void OnTurnOver()
        {
            player.selectedMove = null;
            opponent.selectedMove = null;

            playerMovePage = null;
            opponentMovePage = null;

            EnablePlayerOptions();
        }

        // Called when the player has won the battle.
        public void OnPlayerBattleWon()
        {
            textBox.OnTextBoxFinishedRemoveCallback(OnPlayerBattleWon);
            player.LevelUp();

            // Completed a Battle
            gameManager.completedBattles++;

            // The door is now locked since the room is cleared.
            door.Locked = true;
            ToOverworld();
        }

        // Called when the player has lost the battle.
        public void OnPlayerBattleLost()
        {
            textBox.OnTextBoxFinishedRemoveCallback(OnPlayerBattleLost);
            gameManager.OnGameOver();
            ToOverworld();
        }

        // Call this function to open the treasure.
        public void OpenTreasure()
        {
            // Hide prompt.
            treasurePrompt.gameObject.SetActive(false);

            // The "battle" is over.
            opponent.Health = 0;
        }

        // Call this function to leave the treasure.
        public void LeaveTreasure()
        {
            // Hide prompt.
            treasurePrompt.gameObject.SetActive(false);

            // Return to the overworld.
            ToOverworld();
        }

        // Called when potentially learning a new move.
        public void OnLearningNewMove()
        {
            // Hide the box gameobject.
            textBox.Close();

            // TODO: implement new move learning.
            // The phase.
            int phase = gameManager.GetGamePhase();

            // The new move.
            Move newMove;

            // Checks the phase.
            switch (phase)
            {
                case 1: // beginning - 1
                    newMove = MoveList.Instance.GetRandomRank1Move();
                    break;
                case 2: // middle - 2
                    newMove = MoveList.Instance.GetRandomRank2Move();
                    break;
                case 3: // end - 3
                default:
                    newMove = MoveList.Instance.GetRandomRank3Move();
                    break;
            }

            // If the player already has this move.
            if (player.HasMove(newMove))
            {
                // Becomes 'true' when the new move is set.
                bool newMoveFound = false;

                // Attempts at generating a new move.
                int attempts = 5;

                // Tries to generate a new move that the player doesn't have 5 times.
                for (int n = 0; n < attempts; n++)
                {
                    // Grabs a new move.
                    newMove = MoveList.Instance.GetRandomMove();
                    newMoveFound = player.HasMove(newMove);

                    // If a new move was found already, break early.
                    if (newMoveFound)
                        break;
                }

                // If the new move was still the same, well then the player will have the chance to get multiples of the same move.
                // This shouldn't happen, but it likely won't happen.

            }

            // If the player has less than 4 moves, automatically learn the move.
            if (player.GetMoveCount() < 4)
            {
                for (int i = 0; i < player.moves.Length; i++)
                {
                    if (player.moves[i] == null)
                    {
                        player.moves[i] = newMove;
                        break;
                    }
                }

                // Remove this callback.
                textBox.CurrentPage.OnPageClosedRemoveCallback(OnLearningNewMove);

                // Removes the placeholder page.
                textBox.pages.RemoveAt(textBox.CurrentPageIndex + 1);

                // Inserts a new page.
                textBox.pages.Insert(textBox.CurrentPageIndex + 1, new Page("The player learned " + newMove.Name));

                // Go onto the next page.
                textBox.Open();
                textBox.NextPage();
            }
            else
            {

                // Update the information.
                learnMovePanel.newMove = newMove;
                learnMovePanel.LoadMoveInformation(); // Happens on enable.

                // Turn on the move panel, which also updates the move list.
                learnMovePanel.windowObject.SetActive(true);
            }
        }

        // Goes to the overworld.
        public void ToOverworld()
        {
            // Clear out the textbox.
            if (textBox.IsVisible())
            {
                textBox.Close();
                textBox.pages.Clear();
            }


            // Go to the overworld.
            gameManager.UpdateUI();
            gameManager.EnterOverworld();
        }

        // Updates the user interface.
        public void UpdateUI()
        {
            // TODO: remove the safety checK??
            opponentHealthBar.SetValue(opponent.Health / opponent.MaxHealth);
            opponentHealthText.text = opponent.Health.ToString() + "/" + opponent.MaxHealth.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            // If the text box is not visible.
            if (!textBox.IsVisible())
            {
                // If both entities are alive do battle calculations.
                if (player.Health > 0 && opponent.Health > 0)
                {
                    // If the opponent isn't a treasure chest try to perform moves.
                    if (!(opponent is Treasure))
                        PerformMoves();
                }
                else
                {
                    // Checks if the battle end has been initialized.
                    if (!initBattleEnd)
                    {
                        // Returns to the overworld. TODO: account for game over.
                        // The player got a game over.
                        if (player.Health <= 0) // game over
                        {
                            textBox.pages.Clear();
                            textBox.pages.Add(new Page("The player has lost the battle!"));
                            textBox.SetPage(0);
                            textBox.OnTextBoxFinishedAddCallback(OnPlayerBattleLost);

                            DisablePlayerOptions();
                            textBox.Open();
                        }
                        else // The player won the fight.
                        {
                            textBox.pages.Clear();

                            // Checks if the opponent was a treasure chest.
                            if (opponent is Treasure) // Is Treasure
                            {
                                textBox.pages.Add(new Page("The player has opened the treasure chest!"));
                            }
                            else // Not Treasure
                            {
                                textBox.pages.Add(new Page("The player has won the battle!"));
                            }

                            // Level up message.                        
                            textBox.pages.Add(new Page("The player got a level up!"));

                            // Checks to see if the player will be learning a new move.
                            // If the opponet was a treasure box the player will always get the chance to learn a new move.
                            if (Random.Range(0.0F, 1.0F) <= NEW_MOVE_CHANCE || opponent is Treasure)
                            {
                                Page movePage = new Page("The player is trying to learn a new move!");
                                movePage.OnPageClosedAddCallback(OnLearningNewMove);
                                textBox.pages.Add(movePage);

                                // Placeholder page.
                                textBox.pages.Add(new Page("..."));
                            }

                            // Set up the textbox.
                            textBox.SetPage(0);
                            textBox.OnTextBoxFinishedAddCallback(OnPlayerBattleWon);

                            DisablePlayerOptions();
                            textBox.Open();
                        }

                        // The battle end has been initialized.
                        initBattleEnd = true;
                    }

                }
            }

        }


    }
}