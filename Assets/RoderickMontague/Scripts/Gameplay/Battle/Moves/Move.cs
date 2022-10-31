using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_BBTS
{
    // A class for a move.
    public class Move
    {
        // The number of the move.
        protected moveId id = 0;

        // The name of the move.
        protected string name;

        // The rank of the move.
        protected int rank;

        // The power that a move has.
        protected float power;

        // The accuracy of a move (0.0 - 1.0)
        protected float accuracy;

        // The amount of energy a move uses.
        protected float energy;

        // TODO: add space for animation.

        // The description of a move.
        public string description = "";

        // TODO: replace name with file citation for translation.
        // Move constructor
        public Move(moveId id, string name, int rank, float power, float accuracy, float energy)
        {
            this.id = id;
            this.name = name;
            this.rank = rank;
            this.power = power;
            this.accuracy = accuracy;
            this.energy = energy;

            // Default message.
            description = "No information available";
        }



        // Returns the name of the move.
        public string Name
        {
            get { return name; }
        }

        // Returns the power of the move.
        public float Power
        {
            get { return power; }
        }

        // Returns the accuracy of the move (0-1 range).
        public float Accuracy
        {
            get { return accuracy; }
        }

        // Returns the energy the move uses.
        public float Energy
        {
            get { return energy; }
        }

        // Called to play the move animation.
        public void PlayAnimation()
        {
            // TODO: implement.
        }

        // Called when the move is being performed.
        public virtual bool Perform(BattleEntity user, BattleEntity target, BattleManager battle)
        {
            // The move inserts a message after the current page in the text box.

            // If there isn't enough energy to use the move, nothing happens.
            if (user.Energy < energy)
            {
                battle.textBox.pages.Insert(battle.textBox.CurrentPageIndex + 1, new Page(user.name + " does not have enough power!"));
                return false;
            }
                
            // If the move hit successfully.
            if(Random.Range(0.0F, 1.0F) <= accuracy)
            {
                // Does damage.
                target.Health -= 1.0F; // power * user.Attack;

                // Uses energy.
                user.Energy -= energy;

                // Adds the new page.
                battle.textBox.pages.Insert(battle.textBox.CurrentPageIndex + 1, new Page("The move hit!"));

                // TODO: maybe move this to the battle script?
                // Checks if the user is the player or not.
                if(user is Player) // Is the player.
                {
                    battle.gameManager.UpdatePlayerEnergyUI();
                    battle.UpdateUI(); // Updates enemy health bar.
                }
                else // Not the player.
                {
                    battle.gameManager.UpdatePlayerHealthUI();
                }

                return true;
            }
            else
            {
                // Failure.
                battle.textBox.pages.Insert(battle.textBox.CurrentPageIndex + 1, new Page("The move failed."));

                return false;
            }

            
            
        }
    }
}