using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class CharacterSO : ScriptableObject
{
    
    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("CHARACTER")]
            
            [Header("Data")]
            public int maxHealth = 10;
            public int maxStamina = 20;
            public int maxCooldown = 3;
            public int waitReward = 5;
            public MovementType curMovementType;

        [Header("ANIMATIONS")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;
            public Sprite[] runningSprites;
            public Sprite[] attackSprites;
            public Sprite[] damagedSprites;

        [Header("CARDS")]
            
            [Header("Card One")]
            public CardSO cardOne;

            [Header("Card Two")]
            public CardSO cardTwo;

            [Header("Card Three")]
            public CardSO cardThree;

    #endregion
    
}