using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_BBTS
{
    // The save data for doors.
    [System.Serializable]
    public struct DoorSaveData
    {
        // TODO: include identifier for sprite.

        // Determines if this is a boss door or not.
        public bool isBossDoor;

        // Determines if this is a treasure door or not.
        public bool isTreasureDoor;

        // Says whether the door is locked or not.
        public bool locked;

        // The battle entity behind the door.
        public BattleEntitySaveData battleEntity;
        
    }

    // The door for entering a battle class.
    public class Door : MonoBehaviour
    {
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

        // Awake is called when the script instance is loaded.
        private void Awake()
        {
            // Switch out the sprite.
            Locked = locked;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Called to lock/unlock the door.
        public bool Locked
        {
            get { return locked; }

            set
            {
                locked = value;

                // Changes the sprite.
                if (locked) // closed
                    sprite.sprite = lockedSprite;
                else // open
                    sprite.sprite = unlockedSprite;
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
        }

        // The mouse_touch class will be used to send the door to the overworld manager.

        // // Update is called once per frame
        // void Update()
        // {
        // 
        // }
    }
}