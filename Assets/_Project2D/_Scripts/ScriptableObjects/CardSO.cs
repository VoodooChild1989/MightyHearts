using UnityEngine;

public enum CardType
{
    None,
    Poison,
    PushBack, 
    Pull
}

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class CardSO : ScriptableObject
{
    
    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public Sprite cardIcon;
            public string cardName;
            public string cardDescription;
            public int staminaCost;
            public int attackWaves = 1;
            public CardType curCardType = CardType.None;
            public bool canInteract;

            [Header("Projectile")]
            public int damageAmount = 1;
            public float speed = 5f;
            public int maxBlocks = 4;
            public Sprite[] idleSprites;

    #endregion
    
}