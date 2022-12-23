using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace RM_BBTS
{
    // The save data for doors.
    [System.Serializable]
    public struct DoorSaveData
    {
        // Determines if this is a boss door or not.
        public bool isBossDoor;

        // Determines if this is a treasure door or not.
        public bool isTreasureDoor;

        // The battle entity behind the door.
        public BattleEntitySaveData battleEntity;

        // Says whether the door is locked or not.
        public bool locked;

        // The type of the door for saving the sprite.
        public int doorType;

        // Saves the position of the door.
        // This is needed in case the door position changes from a game over.
        public Vector3 position;
        
    }

    // The door for entering a battle class.
    public class Door : MonoBehaviour
    {
        // The overworld for the game.
        public OverworldManager overworld;

        // The sprite that the door uses.
        public SpriteRenderer sprite;

        // The sprite for the door being open.
        public Sprite unlockedSprite;

        // The sprite for the door being closed.
        public Sprite lockedSprite;

        // Determines if this is a boss door or not.
        public bool isBossDoor = false;

        // Determines if this is a treasure door or not.
        public bool isTreasureDoor = false;

        // The battle entity behind the door.
        public BattleEntityGameData battleEntity;

        // Says whether the door is locked or not.
        private bool locked = false;

        [Header("Animations")]
        // If set to 'true', the door gets animated.
        public bool animate = true;

        // The animator for the door.
        public Animator animator;

        // This is used to set the animation for the door.
        // For some reason setting it when generating the room didn't work.
        public int doorType = 0;

        // Awake is called when the script instance is loaded.
        private void Awake()
        {
            // Animator enabled set.
            animator.enabled = !animate;

            // Switch out the sprite.
            Locked = locked;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Blue (Test)
            // animator.SetInteger("doorType", 2);
            // SetDoorOpenAnimation(2);
        }

        // Called to lock/unlock the door.
        public bool Locked
        {
            get { return locked; }

            set
            {
                locked = value;

                // Updates the sprite for the door.
                UpdateSprite();

                // // Changes the sprite.
                // if (locked) // closed
                // {
                //     sprite.sprite = lockedSprite;
                // }
                // else // open
                // {
                //     sprite.sprite = unlockedSprite;
                // }
            }
        }

        // OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.
        private void OnMouseDown()
        {
            // This sound plays to indicate that a door is locked.
            if (locked && overworld != null)
            {
                // If the mouse touch input is disabled, then a UI element must be open.
                // As such, this audio clip shouldn't play since the door is likely blocked.
                if(overworld.gameManager.mouseTouchInput.isActiveAndEnabled)
                    overworld.PlayDoorLockedSfx();
            }
                
        }

        // Sets the door animation.
        public void SetDoorOpenAnimation(int doorType)
        {
            // This won't show unless the door is open.

            // Sets the integer for the door type.
            animator.SetInteger("doorType", doorType);
        }

        // Updates the sprite.
        public void UpdateSprite()
        {
            sprite.sprite = (locked) ? lockedSprite : unlockedSprite;

            // Checks to see if the door should be animated.
            if(animate)
            {
                // Checks to see if the door is locked.
                if (locked) // Turn off animation.
                {
                    animator.enabled = false;
                }
                else // Turn on the animation.
                {
                    animator.enabled = true;
                }
            }
            else
            {
                animator.enabled = false;
            }
            
        }

        // Generates the save data.
        public DoorSaveData GenerateSaveData()
        {
            // The save data.
            DoorSaveData saveData = new DoorSaveData();

            // Save the values.
            saveData.isBossDoor = isBossDoor;
            saveData.isTreasureDoor = isTreasureDoor;
            saveData.locked = locked;

            // Battle Entity Save
            saveData.battleEntity = BattleEntity.ConvertBattleEntityGameDataToSaveData(battleEntity);

            // Saves the door type for animations.
            saveData.doorType = doorType;

            // Save the door's position.
            saveData.position = transform.position;

            // Returns the save data.
            return saveData;
        }


        // Loads the save data to overwrite the door's cureent values.
        public void LoadSaveData(DoorSaveData data)
        {
            // Implement the door settings.
            isBossDoor = data.isBossDoor;
            isTreasureDoor = data.isTreasureDoor;
            Locked = data.locked;

            // Implement the battle entity.
            battleEntity = BattleEntity.ConvertBattleEntitySaveDataToGameData(data.battleEntity);

            // Save the door type and change the sprites.
            doorType = data.doorType;
            overworld.SetDoorSpritesByDoorType(this);

            // Sets the door's position from the save data in case it got changed.
            transform.position = data.position;

            UpdateSprite();
        }

        // The mouse_touch class will be used to send the door to the overworld manager.

        // Update is called once per frame
        void Update()
        {
            // If animating the door, and it is enabled.
            if(animate && animator.enabled)
            {
                // Sets the right animation.
                if (doorType != animator.GetInteger("doorType"))
                    animator.SetInteger("doorType", doorType);
            }
        }
    }
}