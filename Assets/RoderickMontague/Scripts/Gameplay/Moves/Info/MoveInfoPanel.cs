using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A panel for the moves.
namespace RM_BBTS
{
    public class MoveInfoPanel : MonoBehaviour
    {
        // The id of the move being represented.
        private moveId id;

        // Move Title
        public TMPro.TMP_Text nameText;

        // Move Attributes
        public TMPro.TMP_Text rankText;
        public TMPro.TMP_Text powerText;
        public TMPro.TMP_Text accuracyText;
        public TMPro.TMP_Text energyText;

        // Move Description
        public TMPro.TMP_Text description;

        // Gets the move id.
        public moveId Id
        {
            get { return id; }
        }

        // Loads the move into the move info pnael.
        public void LoadMoveInfo(Move move)
        {
            // Id
            id = move.Id;

            // Name
            nameText.text = move.Name;

            // Rank
            rankText.text = move.Rank.ToString();

            // Power
            powerText.text = move.GetPowerAsString();

            // Accuracy
            accuracyText.text = move.GetAccuracyAsString();

            // Energy
            energyText.text = move.GetEnergyUsageAsString();

            // Description
            description.text = move.description;

        }
    }
}