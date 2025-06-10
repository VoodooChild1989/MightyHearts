using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Scriptable Objects/Projectile")]
public class ProjectileSO : ScriptableObject
{
    
    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public float speed = 5f;
            public int maxBlocks = 4;

        [Header("ANIMATIONS")]
            
            [Header("Basic Variables")]
            public Sprite[] idleSprites;

    #endregion

}