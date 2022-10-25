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

        [Header("Battle")]

        // The player.
        public Player player;

        // The opponent for the player.
        // TODO: should a new opponent be generated everytime? Should really just cycle through some pre-build objects.
        public BattleEntity opponent;


        // The sprite for the opponent.
        public SpriteRenderer opponentSprite;

        [Header("UI")]
        // The user interface.
        public GameObject ui;

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

        // Start is called before the first frame update
        void Start()
        {
            // ...
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
            if(ui != null)
                ui.SetActive(false);
        }

        // Initializes the overworld.
        public override void Initialize()
        {
            initialized = true;

            // Sets the battle entity from the door.
            opponent = door.battleEntity;
            
            if(opponent != null)
                opponentSprite.sprite = opponent.sprite;

            // Checks move activity to see if the player can use it or not.
            // Also changes the move name on the display.

            // Move 0
            move0Button.interactable = player.Move0 != null;
            move0Text.text = (player.Move0 != null) ? player.Move0.Name : "-";

            // Move 1
            move1Button.interactable = player.Move1 != null;
            move1Text.text = (player.Move1 != null) ? player.Move1.Name : "-";

            // Move 2
            move2Button.interactable = player.Move2 != null;
            move2Text.text = (player.Move2 != null) ? player.Move2.Name : "-";

            // Move 3
            move3Button.interactable = player.Move3 != null;
            move3Text.text = (player.Move3 != null) ? player.Move3.Name : "-";

            // Checks if the player has a full charge.
            chargeButton.interactable = !player.HasFullCharge();
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

        // Called when the player attempts to run away.
        public void RunAway()
        {
            // Becomes 'true' if the run attempt was successful.
            bool success = false;

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
                gameManager.EnterOverworld();
            else
                Debug.Log("Run failed.");
        }

        // Update is called once per frame
        void Update()
        {

        }

        
    }
}