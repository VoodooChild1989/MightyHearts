using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.Playables;
using TMPro;

public class TilemapManager : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public Tilemap tilemap;
            public GameObject block;
            public GameObject cell;
            public Vector2 gridOffset;
            public bool areCellsVisible;
            public Button cellButton;
            public GameObject[,] collisionMatrixCells;
            public int[,] collisionMatrix;
            public static TilemapManager instance;
            
            [Header("Data")]
            public int width;
            public int height;
            public int startX;
            public int startY;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            SingletonUtility.MakeSingleton(ref instance, this, false);
            
            width = tilemap.cellBounds.size.x;
            height = tilemap.cellBounds.size.y;
            startX = tilemap.cellBounds.xMin;
            startY = tilemap.cellBounds.yMin;
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            collisionMatrix = new int[width, height];
            collisionMatrixCells = new GameObject[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Vector3Int pos = new Vector3Int(startX + i, startY + j, 0);

                    if (tilemap.HasTile(pos))
                    {                        
                        GameObject blockObj = Instantiate(block, tilemap.CellToWorld(pos) + (Vector3)gridOffset, Quaternion.identity);
                        SpriteRenderer sr = blockObj.GetComponent<SpriteRenderer>();
                        sr.sprite = tilemap.GetSprite(pos);
                        blockObj.name = $"Block_{i}_{j}";
                        blockObj.transform.SetParent(GameObject.Find("Blocks").transform);
                    }
                    
                    GameObject cellObj = Instantiate(cell, new Vector3(startX + i, startY + j, 0f) + (Vector3)gridOffset, Quaternion.identity);
                    Cell cellScript = cellObj.GetComponent<Cell>();
                    cellScript.x = i;
                    cellScript.y = j;
                    cellObj.name = $"Cell_{i}_{j}";
                    cellObj.transform.SetParent(GameObject.Find("Cells").transform);
                    collisionMatrixCells[i, j] = cellObj;
                }
            }

            tilemap.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            // Add your per-frame logic here.
            // Example: Move objects, check user input, update animations, etc.
        }

        /// <summary>
        /// Called at fixed intervals, ideal for physics updates.
        /// Use this for physics-related updates like applying forces or handling Rigidbody physics.
        /// </summary>
        private void FixedUpdate()
        {
            // Add physics-related logic here.
            // Example: Rigidbody movement, applying forces, or collision detection.
        }

    #endregion

    #region CUSTOM METHODS

        public void UpdateCollisionMatrix(int x, int y)
        {
            Collider2D collider = collisionMatrixCells[x, y].GetComponent<Collider2D>();

            if (collider.IsTouchingLayers())
            {
                collisionMatrix[x, y] = 1;
            }
            else
            {
                collisionMatrix[x, y] = 0;
            }
        }

        public bool IsNewPositionPossible(GameObject sender, List<GameObject> sizeMatrix, int moveX, int moveY, MovementType curMovementType)
        {   
            foreach (GameObject cell in sizeMatrix)
            {
                Cell sizeMatrixCellScript = cell.GetComponent<Cell>();
                int newX = sizeMatrixCellScript.x + moveX;
                int newY = sizeMatrixCellScript.y + moveY;
                
                if (collisionMatrix[newX, newY] != 0)
                {
                    Cell collisionMatrixCellScript = collisionMatrixCells[newX, newY].GetComponent<Cell>();   

                    if (collisionMatrixCellScript.collidesWith != null && collisionMatrixCellScript.collidesWith != sender)    
                    {
                        return false;
                    }
                }
            }   

            return true;
        }
    
        public void ToggleCellsVisibility()
        {
            areCellsVisible = !areCellsVisible;

            if (areCellsVisible)
            {
                cellButton.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
            }
            else
            {
                cellButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    collisionMatrixCells[i, j].GetComponent<Cell>().CheckVisibility();
                }
            }
        }

        public void ShowTrajectory(CardSO card)
        {
            Character curChar = LevelManager.instance.charactersOnQueue[LevelManager.instance.curQueueIndex - 1];
            int blocksToDraw = card.cardProjectile.maxBlocks;
            int initX = 0;
            int initY = 0;

            if (curChar.cardOne == card)
            {
                initX = curChar.cardOneSpawnPoint.GetComponent<SpawnPoint>().collidingCell.x;
                initY = curChar.cardOneSpawnPoint.GetComponent<SpawnPoint>().collidingCell.y;
            }
            else if (curChar.cardTwo == card)
            {
                initX = curChar.cardTwoSpawnPoint.GetComponent<SpawnPoint>().collidingCell.x;
                initY = curChar.cardTwoSpawnPoint.GetComponent<SpawnPoint>().collidingCell.y;
            }
            else if (curChar.cardThree == card)
            {
                initX = curChar.cardThreeSpawnPoint.GetComponent<SpawnPoint>().collidingCell.x;
                initY = curChar.cardThreeSpawnPoint.GetComponent<SpawnPoint>().collidingCell.y;
            }
            
            SetTrajectory(curChar, initX, initY, blocksToDraw, true);
        }

        public void HideTrajectory(CardSO card)
        {
            Character curChar = LevelManager.instance.charactersOnQueue[LevelManager.instance.curQueueIndex - 1];
            int blocksToDraw = card.cardProjectile.maxBlocks;
            int initX = 0;
            int initY = 0;

            if (curChar.cardOne == card)
            {
                initX = curChar.cardOneSpawnPoint.GetComponent<SpawnPoint>().collidingCell.x;
                initY = curChar.cardOneSpawnPoint.GetComponent<SpawnPoint>().collidingCell.y;
            }
            else if (curChar.cardTwo == card)
            {
                initX = curChar.cardTwoSpawnPoint.GetComponent<SpawnPoint>().collidingCell.x;
                initY = curChar.cardTwoSpawnPoint.GetComponent<SpawnPoint>().collidingCell.y;
            }
            else if (curChar.cardThree == card)
            {
                initX = curChar.cardThreeSpawnPoint.GetComponent<SpawnPoint>().collidingCell.x;
                initY = curChar.cardThreeSpawnPoint.GetComponent<SpawnPoint>().collidingCell.y;
            }
            
            SetTrajectory(curChar, initX, initY, blocksToDraw, false);
        }

        private void SetTrajectory(Character curChar, int initX, int initY, int blocksToDraw, bool show)
        {
            for (int i = 0; i < blocksToDraw; i++)
            {
                Cell cellObj = null;

                if (curChar.curFacingDirection == FacingDirection.Left)
                {
                    cellObj = collisionMatrixCells[initX - i, initY].GetComponent<Cell>();
                }
                else
                {
                    cellObj = collisionMatrixCells[initX + i, initY].GetComponent<Cell>();
                }

                if (show)
                {
                    cellObj.SetAsTrajectory();
                }
                else
                {
                    cellObj.RemoveAsTrajectory();
                }
            }
        }

    #endregion

}