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
            public int cardOneAttackWaves = 1;

            [Header("Card Two")]
            public CardSO cardTwo;
            public int cardTwoAttackWaves = 1;

            [Header("Card Three")]
            public CardSO cardThree;
            public int cardThreeAttackWaves = 1;

    #endregion
    
}