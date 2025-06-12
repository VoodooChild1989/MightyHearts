using UnityEngine;

public enum CardType
{
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
            public int damageAmount;
            public ProjectileSO cardProjectile;
            public int attackWaves = 1;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public CardType curCardType;

    #endregion
    
}