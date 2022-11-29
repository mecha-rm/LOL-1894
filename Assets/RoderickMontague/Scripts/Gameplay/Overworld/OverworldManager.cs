using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RM_BBTS
{
    // The manager for the overworld.
    public class OverworldManager : GameState
    {
        // Becomes 'true' when the overworld is initialized.
        public bool initialized = false;

        // The gameplay manager.
        public GameplayManager gameManager;



        // The object that was selected in the overworld.
        // public GameObject selectedObject;

        // Gets set to 'true' when the game is in a game over state.
        // If 'true', the game over function will be called when the player returns to the overworld.
        public bool gameOver;

        [Header("Doors")]
        // The list of doors.
        public List<Door> doors = new List<Door>();

        // The boss door.
        public Door bossDoor = null;

        // THe treasure doors.
        public List<Door> treasureDoors = null; 

        // The amount of the doors.
        public const int DOOR_COUNT = 18;

        // The amount of treasures for the game.
        public const int TREASURE_COUNT = 3;

        // // The door prefab to be instantiated.
        // public GameObject doorPrefab;
        // 
        // // TODO: remove these variables when the door is finished.
        // // The rows and colums for the doors in the overworld.
        // // Currently set to a 6 x 3 (cols, rows) setup.
        // public const int ROWS = 3;
        // public const int COLUMNS = 6;
        // 
        // // The reference object for placing a door.
        // public GameObject doorParent;
        // 
        // // the position offset for placing doors.
        // public Vector3 doorPosOffset = new Vector3(2.0F, -2.0F, 0);

        [Header("Doors/Sprites")]

        // The list of unlocked and locked door sprites (does NOT include the boss door).
        public List<Sprite> doorUnlockedSprites;
        public List<Sprite> doorLockedSprites;

        // The unlocked and locked boss door sprites.
        public Sprite bossDoorUnlockedSprite;
        public Sprite bossDoorLockedSprite;

        [Header("UI")]
        
        // The user interface.
        public GameObject ui;

        // The score text for the overworld.
        public TMP_Text scoreText;

        // The amount of digits the score has. If it surpasses this amount it will add more digits.
        // It can probably just be 5 digits.
        public const int SCORE_DIGITS = 10;

        // Start is called before the first frame update
        void Start()
        {
            // ...
        }

        // This function is called when the object becomes enabled and active
        private void OnEnable()
        {
            if(ui != null)
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
            // Initializes the doors (normal, treasure, and boss)
            {
                // Door initialization list.
                List<Door> doorInitList = new List<Door>(doors);

                // A random index.
                int randIndex = 0;

                // BOSS
                // The boss door
                randIndex = Random.Range(0, doorInitList.Count); // Grabs the random index.
                bossDoor = doorInitList[randIndex]; // Grab random door to make boss door.
                doorInitList.RemoveAt(randIndex); // Remove boss door from list.
                
                // Replaces the sprite in the 'GenerateRoom' function for consistency.
                bossDoor.isBossDoor = true; // This is a boss door.
                GenerateRoom(bossDoor); // Generates the room.

                // TREASURES
                // The treasure doors.
                treasureDoors = new List<Door>();

                // While there are still treasure doors.
                while(treasureDoors.Count < TREASURE_COUNT && treasureDoors.Count < doorInitList.Count)
                {
                    randIndex = Random.Range(0, doorInitList.Count); // random index
                    
                    Door treasureDoor = doorInitList[randIndex]; // Grab the door.
                    treasureDoors.Add(treasureDoor); // Add to list.
                    
                    doorInitList.RemoveAt(randIndex); // Remove the door from the overall list.
                    treasureDoor.isTreasureDoor = true; // This is a treasure door.
                    GenerateRoom(treasureDoor); // Generates the room.
                }

                // Initialize the rest of the doors.
                foreach(Door door in doorInitList)
                {
                    GenerateRoom(door);
                }

            }

            // Updates the UI.
            UpdateUI();

            // Plays the overworld BGM.
            gameManager.PlayOverworldBgm();

            initialized = true;
        }

        // Called when the mouse hovers over an object.
        public override void OnMouseHovered(GameObject hoveredObject)
        {
            // ... highlight
        }

        // Called when the mouse interacts with an entity.
        public override void OnMouseInteract(GameObject heldObject)
        {
            // selectedObject = heldObject;

            OnInteractReceive(heldObject);
        }

        // Called when the user's touch interacts with an entity.
        public override void OnTouchInteract(GameObject touchedObject, Touch touch)
        {
            // selectedObject = touchedObject;

            // If the object is not set to null.

            // This is the first time the object has been tapped.
            if (touch.tapCount > 1)
            {
                // ENTER
            }
            // This is the first tap.
            else if (touch.tapCount == 1)
            {
                // HIGHLIGHT
            }

            OnInteractReceive(touchedObject);
        }

        // Called with the object that was received with the interaction.
        protected override void OnInteractReceive(GameObject gameObject)
        {
            // Door object.
            Door door = null;

            // Tries to grab the door component.
            if(gameObject.TryGetComponent<Door>(out door))
            {
                // Enters the battle.
                gameManager.EnterBattle(door);
            }
        }

        // A function to call when a tutorial starts.
        public override void OnTutorialStart()
        {

        }

        // A function to call when a tutorial ends.
        public override void OnTutorialEnd()
        {

        }

        // Generates a room for the door.
        private void GenerateRoom(Door door)
        {
            // Unlock the door.
            door.Locked = false;

            // Checks the door type.
            if(door.isBossDoor) // Boss Door
            {
                // TODO: randomize between the three bosses.
                door.battleEntity = BattleEntityList.Instance.GenerateBattleEntityData(battleEntityId.combatbot);

                // Replaces the sprites.
                bossDoor.unlockedSprite = bossDoorUnlockedSprite;
                bossDoor.lockedSprite = bossDoorLockedSprite;
            }
            else if(door.isTreasureDoor) // Treasure Door
            {
                door.battleEntity = BattleEntityList.Instance.GenerateBattleEntityData(battleEntityId.treasure);
            }
            else // Normal Door
            {
                // Test
                // door.battleEntity = BattleEntityList.Instance.GenerateBattleEntityData(battleEntityId.ufo);

                // Generates a random enemy (base version).
                
                // TODO: switch to the final version after implementing more enemies.
                // FINAL
                // door.battleEntity = BattleEntityList.Instance.GenerateRandomEnemy(true, true, true);

                // TESTING 
                door.battleEntity = BattleEntityList.Instance.GenerateRandomEnemy(false, false, true);

            }

            // Randomizes the door image if it's not a boss door.
            if (!door.isBossDoor && doorLockedSprites.Count != 0 && doorLockedSprites.Count == doorUnlockedSprites.Count)
            {
                // Generates a random door image.
                int index = Random.Range(0, doorLockedSprites.Count);

                // Replaces the sprites.
                door.unlockedSprite = doorUnlockedSprites[index];
                door.lockedSprite = doorLockedSprites[index];
            }

            // Sets the level.
            door.battleEntity = BattleEntity.LevelUpData(door.battleEntity, 
                (uint)Random.Range(1, gameManager.roomsPerLevelUp + 1));


            // TODO: randomize the enemy being placed behind the door.
            // door.battleEntity = BattleEntityList.Instance.GenerateBattleEntityData(battleEntityId.unknown);
            // door.battleEntity = BattleEntityList.Instance.GenerateBattleEntityData(battleEntityId.treasure);
            // door.battleEntity = BattleEntityList.Instance.GenerateBattleEntityData(battleEntityId.boss);

            // Make sure the battle entity is parented to the door.
            // TODO: have algorithm for generating enemies.

        }

        // Updates the UI for the overworld.
        public void UpdateUI()
        {
            // Set the base score.
            scoreText.text = gameManager.score.ToString("D" + SCORE_DIGITS.ToString());
        }

        // Called when returning to the overworld.
        public void OnOverworldReturn()
        {
            // Rearranges the doors.
            if (gameOver)
                OnOverworldReturnGameOver();

            // Evolves the entities.
            int phase = gameManager.GetGamePhase();

            // Time to level up enemies if 'true'
            if(gameManager.roomsCompleted % gameManager.roomsPerLevelUp == 0)
            {
                // The enemies haven't been leveled up yet.
                if(gameManager.lastEnemyLevelUps < gameManager.roomsCompleted)
                {
                    // Goes through each door.
                    foreach(Door door in doors)
                    {
                        // Only level up unlocked doors.
                        if(!door.Locked)
                        {
                            // Levels up the entity by the amount of battles per level up (the value is the same).
                            door.battleEntity = BattleEntity.LevelUpData(door.battleEntity, (uint)gameManager.roomsPerLevelUp);
                        }
                    }


                    gameManager.lastEnemyLevelUps = gameManager.roomsCompleted;
                }
            }

            // If in the middle phase, and no evolutions have happened.
            // If in the end phase, and the evolutions have not been run a second time.
            if ((phase == 2 && gameManager.evolveWaves == 0) || (phase == 3 && gameManager.evolveWaves == 1))
            {
                // Goes through each door.
                foreach(Door door in doors)
                {
                    // Only evolve the entity if the door is unlocked.
                    // It helps save on evolution time.
                    if (!door.Locked)
                    {
                        door.battleEntity = BattleEntity.EvolveData(door.battleEntity);

                        // Restore health and energy levels to max even if the entity didn't evolve.
                        door.battleEntity.health = door.battleEntity.maxHealth;
                        door.battleEntity.energy = door.battleEntity.maxEnergy;

                    }
                        
                }

                gameManager.evolveWaves++;
            }

            // Update the UI for the overworld.
            UpdateUI();

            // Plays the overworld BGM.
            gameManager.PlayOverworldBgm();

        }
        
        
        // Rearranges the doors.
        public void OnOverworldReturnGameOver()
        {
            // The new positions
            List<Vector3> doorLocs = new List<Vector3>();

            // Geta all door positions.
            foreach(Door door in doors)
            {
                // Restore health and energy to entity.
                door.battleEntity.health = door.battleEntity.maxHealth;
                door.battleEntity.energy = door.battleEntity.maxEnergy;

                // Adds the position to the list.
                doorLocs.Add(door.gameObject.transform.position);
            }

            // Goes through each door again.
            for(int i = 0; i < doors.Count && doorLocs.Count != 0; i++)
            {
                // Grabs a random index.
                int randIndex = Random.Range(0, doorLocs.Count);

                // Re-positions the door.
                doors[i].transform.position = doorLocs[randIndex];

                // Removes position from list.
                doorLocs.RemoveAt(randIndex);
            }

            // Randomize player moves
            Player player = gameManager.player;
            
            // Go through each move.
            for(int i = 0; i < player.moves.Length; i++)
            {
                // Move has been set.
                if (player.moves[i] != null)
                {
                    // Grabs the rank.
                    int rank = player.moves[i].Rank;

                    // Replaces the move.
                    switch(rank)
                    {
                        case 1: // R1
                            player.moves[i] = MoveList.Instance.GetRandomRank1Move();
                            break;
                        case 2: // R2
                            player.moves[i] = MoveList.Instance.GetRandomRank2Move();
                            break;
                        case 3: // R3
                            player.moves[i] = MoveList.Instance.GetRandomRank3Move();
                            break;
                    }
                }
            }


            gameOver = false;

        }


        // Update is called once per frame
        void Update()
        {

        }

        
    }
}