using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

namespace RM_BBTS
{
    // The player stats window.
    public class PlayerStatsWindow : MonoBehaviour
    {
        // The gameplay manager.
        public GameplayManager gameManager;

        //  Player object.
        public Player player;

        // The charge and run moves for showing descriptions.
        private ChargeMove chargeMove;
        private RunMove runMove;

        [Header("Text")]
        // Text windows.
        public TMP_Text levelText;
        public TMP_Text healthText;
        public TMP_Text attackText;
        public TMP_Text defenseText;
        public TMP_Text speedText;
        public TMP_Text energyText;

        // // String labels for each piece of text.
        // private string levelString = "<Level>";
        // private string healthString = "<Health>";
        // private string attackString = "<Attack>";
        // private string defenseString = "<Defense>";
        // private string speedString = "<Speed>";
        // private string energyString = "<Energy>";

        [Header("Buttons")]
        // Moves
        public Button move0Button;
        public TMP_Text move0ButtonText;

        public Button move1Button;
        public TMP_Text move1ButtonText;

        public Button move2Button;
        public TMP_Text move2ButtonText;

        public Button move3Button;
        public TMP_Text move3ButtonText;

        // Charge
        public Button chargeButton;
        public TMP_Text chargeButtonText;

        // Run
        public Button runButton;
        public TMP_Text runButtonText;

        // Back Button Text
        public TMP_Text backButtonText;

        [Header("Move Info")]
        // Move Info
        public TMP_Text moveNameText;

        public TMP_Text moveRankText;
        public TMP_Text movePowerText;
        public TMP_Text moveAccuracyText;
        public TMP_Text moveEnergyText;

        public TMP_Text moveDescriptionText;

        // String labels
        private string rankString = "<Rank>";
        private string powerString = "<Power>";
        private string accuracyString = "<Accuracy>";
        // The energy string is reused from the gameManager.
        private string descriptionString = "<Description>";

        // Awake is called when the script is being loaded.
        private void Awake()
        {
            // Load moves.
            chargeMove = new ChargeMove();
            runMove = new RunMove();
        }

        // Start is called before the first frame update
        void Start()
        {

            // Updates the window to start off.
            // UpdateWindow();

            // Translation
            JSONNode defs = SharedState.LanguageDefs;

            // Language definitions set.
            if(defs != null)
            {
                // levelString = defs["kwd_level"];
                // healthString = defs["kwd_health"];
                // attackString = defs["kwd_attack"];
                // defenseString = defs["kwd_defense"];
                // speedString = defs["kwd_speed"];
                // energyString = defs["kwd_energy"];

                backButtonText.text = defs["kwd_back"];

                rankString = defs["kwd_rank"];
                powerString = defs["kwd_power"];
                accuracyString = defs["kwd_accuracy"];
                descriptionString = defs["kwd_description"];
            }
        }

        // This function is called when the object becomes enabled and active.
        private void OnEnable()
        {
            UpdatePlayerInfo();
        }

        // // Toggles the visibility of the player stat window.
        // public void ToggleVisibility()
        // {
        //     gameObject.SetActive(!gameObject.activeSelf);
        // 
        //     // Disables/Enables Certain Functions
        //     gameManager.mouseTouchInput.gameObject.SetActive(!gameObject.activeSelf);
        // }

        // Upates the UI for the stats window.
        public void UpdatePlayerInfo()
        {
            // TEXT
            // Level
            levelText.text = gameManager.LevelString + ": " + player.Level.ToString();

            // Stats
            healthText.text = gameManager.HealthString + ": " + Mathf.Ceil(player.Health).ToString() + "/" + Mathf.Ceil(player.MaxHealth).ToString();
            attackText.text = gameManager.AttackString + ": " + Mathf.Ceil(player.Attack).ToString();
            defenseText.text = gameManager.DefenseString + ": " + Mathf.Ceil(player.Defense).ToString();
            speedText.text = gameManager.SpeedString + ": " + Mathf.Ceil(player.Speed).ToString();

            // Energy
            // Now shows as a percentage.
            // energyText.text = gameManager.EnergyString + ": " + Mathf.Ceil(player.Energy).ToString() + "/" + Mathf.Ceil(player.MaxEnergy).ToString();
            energyText.text = gameManager.EnergyString + ": " +
                (player.Energy / player.MaxEnergy * 100.0F).ToString("F" + GameplayManager.ENERGY_DECIMAL_PLACES.ToString()) + "%";

            // BUTTONS
            // M0
            move0Button.interactable = player.Move0 != null;
            move0ButtonText.text = (player.Move0 != null) ? player.Move0.Name : "-";

            // M1
            move1Button.interactable = player.Move1 != null;
            move1ButtonText.text = (player.Move1 != null) ? player.Move1.Name : "-";

            // M2
            move2Button.interactable = player.Move2 != null;
            move2ButtonText.text = (player.Move2 != null) ? player.Move2.Name : "-";

            // M3
            move3Button.interactable = player.Move3 != null;
            move3ButtonText.text = (player.Move3 != null) ? player.Move3.Name : "-";

            // Charge and Run Buttons
            chargeButtonText.text = MoveList.Instance.ChargeMove.Name;
            runButtonText.text = MoveList.Instance.RunMove.Name;

            // These moves can always be interacted with.
            chargeButton.interactable = true;
            runButton.interactable = true;

            // Default showing.
            UpdateMoveInfo(4);
        }

        // Updates the move info.
        // [0 - 3] Player Moves, [4] - Charge, [5] - Run
        public void UpdateMoveInfo(int moveNumber)
        {
            // Move object
            Move move = null;

            // Checks move number
            switch(moveNumber)
            {
                case 0: // Move 0
                    move = player.Move0;
                    break;

                case 1: // Move 1
                    move = player.Move1;
                    break;

                case 2: // Move 2
                    move = player.Move2;
                    break;

                case 3: // Move 3
                    move = player.Move3;
                    break;

                case 4: // Charge
                    move = chargeMove;
                    break;

                case 5: // Run
                    move = runMove;
                    break;

                default:
                    return;

            }

            // Updates the visuals.
            moveNameText.text = move.Name;

            moveRankText.text = rankString + ": " + move.Rank.ToString();
            movePowerText.text = powerString + ": " + move.Power.ToString();
            moveAccuracyText.text = accuracyString + ": " + Mathf.Round(move.Accuracy * 100.0F).ToString() + "%";
            moveEnergyText.text = gameManager.EnergyString + ": " + (move.EnergyUsage * 100.0F).ToString() + "%";

            moveDescriptionText.text = descriptionString + ": " + move.description.ToString();

        }

        
        // Switch selected move to move 0.
        public void SwitchToMove0()
        {
            UpdateMoveInfo(0);

        }

        // Switch selected move to move 1.
        public void SwitchToMove1()
        {
            UpdateMoveInfo(1);
        }

        // Switch selected move to move 2.
        public void SwitchToMove2()
        {
            UpdateMoveInfo(2);
        }

        // Switch selected move to move 3.
        public void SwitchToMove3()
        {
            UpdateMoveInfo(3);
        }

        // Switch selected move to charge move.
        public void SwitchToChargeMove()
        {
            UpdateMoveInfo(4);
        }

        // Switch selected move to run move.
        public void SwitchToRunMove()
        {
            UpdateMoveInfo(5);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}