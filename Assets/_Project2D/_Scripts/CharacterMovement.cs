using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MovementType
{
    Ground,
    Air
}

public enum FacingDirection
{
    Right,
    Left
}

public class CharacterMovement : MonoBehaviour
{

    #region FIELDS

        [Header("NOTES")] [TextArea(4, 10)]
        public string notes;

        [Header("VARIABLES")]
            
            [Header("Basic Variables")]
            public List<GameObject> sizeMatrix;
            [ShowOnly] public int moveSteps;
            [ShowOnly] public int moveStaminaCost;
            [ShowOnly] public MovementType curMovementType;
            [ShowOnly] public FacingDirection curFacingDirection;
            [ShowOnly] public bool canSwitchMoveTypes;
            private Coroutine movementCrt;

        [Header("REFERENCES")]

            [Header("Scripts")]
            private CharacterAnimation chrAnim;
            private CharacterStatistics chrStats;
            private CharacterCards chrCards;

    #endregion

    #region LIFE CYCLE METHODS

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Useful for initialization before the game starts.
        /// </summary>
        private void Awake()
        {
            chrAnim = GetComponent<CharacterAnimation>();
            chrStats = GetComponent<CharacterStatistics>();
            chrCards = GetComponent<CharacterCards>();
        }

        /// <summary>
        /// Called before the first frame update.
        /// Useful for initialization once the game starts.
        /// </summary>
        private void Start()
        {
            sizeMatrix = new List<GameObject>();
            canSwitchMoveTypes = false;     

            UpdateMovementType();
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

        public void UpdateMovementType()
        {
            if (curMovementType == MovementType.Ground)
            {
                Sprite icon = LevelManager.instance.groundIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                chrStats.moveTypeIcon.sprite = icon;
            }
            else if (curMovementType == MovementType.Air)
            {
                Sprite icon = LevelManager.instance.airIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                chrStats.moveTypeIcon.sprite = icon;
            }
            
            LevelManager.instance.switchMovementTypesButton.interactable = canSwitchMoveTypes;
        }

        public void AddCellToSizeMatrix(GameObject cell)
        {
            if (!sizeMatrix.Contains(cell)) sizeMatrix.Add(cell);
        }

        public void RemoveCellFromSizeMatrix(GameObject cell)
        {
            if (sizeMatrix.Contains(cell)) sizeMatrix.Remove(cell);
        }

    #endregion

    #region AUTO

        public void StartAutoAttack()
        {
            StartCoroutine(AutoAttack());
        }

        private IEnumerator AutoAttack()
        {
            yield return new WaitForSeconds(1f);

            bool canAttack = false;

            for (int i = 1; i <= 3; i++)
            {
                if (i == 1)
                {
                    if (chrStats.curStamina >= chrCards.cardOne.staminaCost)
                    {
                        chrCards.FirstCard();
                        canAttack = true;
                        break;
                    }
                }
                else if (i == 2)
                {
                    if (chrStats.curStamina >= chrCards.cardTwo.staminaCost)
                    {
                        chrCards.SecondCard();
                        canAttack = true;
                        break;
                    }
                }
                else if (i == 3)
                {
                    if (chrStats.curStamina >= chrCards.cardThree.staminaCost)
                    {
                        chrCards.ThirdCard();
                        canAttack = true;
                        break;
                    }
                }
            }

            if (!canAttack) 
            {
                int rndNum = UnityEngine.Random.Range(1, 3);
                
                if (rndNum == 1)
                {
                    Flip();
                }
                else
                {
                    chrStats.Wait();
                }
            }
        }

        public void StartAutoMove()
        {
            StartCoroutine(AutoMove());
        }

        private IEnumerator AutoMove()
        {
            yield return new WaitForSeconds(1f);

            bool canMove = false;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    if (IsThisNewPositionPossible(i, j))
                    {
                        Move(i, j);   
                        canMove = true;
                        break;
                    }
                }

                if (canMove) break;
            }

            if (!canMove)
            {   
                int rndNum = UnityEngine.Random.Range(1, 3);
                
                if (rndNum == 1)
                {
                    Flip();
                }
                else
                {
                    chrStats.Wait();
                }
            }
        }

    #endregion

    #region MANUAL
    
        public void StartManualMovement()
        {
            movementCrt = StartCoroutine(ManualMovement());
        }

        public void StopManualMovement()
        {
            if (movementCrt != null)
            {
                StopCoroutine(movementCrt);
                movementCrt = null;
            }
        }

        private IEnumerator ManualMovement() 
        {
            while (true)
            {   
                int newX = 0;
                int newY = 0;

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    newX = -1;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    newX = 0;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    newX = 1;
                    newY = 1;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    newX = -1;
                    newY = 0;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    newX = 1;
                    newY = 0;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z))
                {
                    newX = -1;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {                    
                    newX = 0;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.C))
                { 
                    newX = 1;
                    newY = -1;

                    if (IsThisNewPositionPossible(newX, newY) && chrStats.curStamina >= moveStaminaCost)
                    {
                        chrStats.CheckCards();
                        Move(newX, newY);
                        chrStats.RemoveStamina(moveStaminaCost);
                        yield break;
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    yield return null;
                    chrStats.CheckCards();
                    yield return null;
                    Flip();
                    yield break;
                }

                yield return null;
            }
        }
        
    #endregion

    #region MOVEMENT

        public void Move(int moveX, int moveY)
        {
            Vector2 curPos = transform.position;
            Vector2 newPos = new Vector2(curPos.x + moveX, curPos.y + moveY);

            if (TilemapManager.instance.IsNewPositionPossible(gameObject, sizeMatrix, moveX, moveY, curMovementType))
            {
                StartCoroutine(MoveToNewPos(newPos, moveX, moveY));
            }
        }

        private IEnumerator MoveToNewPos(Vector3 newPos, int moveX, int moveY)
        {
            chrAnim.SetRunningAnimation();
            moveSteps--;    
            
            while (transform.parent.position != newPos)
            {
                transform.parent.position = Vector2.MoveTowards(transform.parent.position, newPos, 0.05f);

                yield return new WaitForSeconds(0.01f);
            }

            transform.parent.position = newPos;   

            if (curMovementType == MovementType.Ground)
            {
                yield return new WaitForSeconds(0.1f);

                if (moveSteps > 0)
                {
                    if (IsThisNewPositionPossible(moveX, moveY))
                    {
                        yield return new WaitForSeconds(0.05f);

                        Move(moveX, moveY);
                    }
                    else
                    {
                        moveSteps = 0;
                        
                        if (IsThisNewPositionPossible(0, -1))
                        {
                            yield return new WaitForSeconds(0.05f);

                            Move(0, -1);   
                        }
                        else
                        {
                            FinishMovement();
                        }
                    }
                }
                else
                {
                    if (IsThisNewPositionPossible(0, -1))
                    {
                        yield return new WaitForSeconds(0.05f);

                        Move(0, -1);   
                    }
                    else
                    {
                        FinishMovement();
                    }
                }
            }
            else
            {
                if (moveSteps > 0)
                {
                    if (IsThisNewPositionPossible(moveX, moveY))
                    {
                        yield return new WaitForSeconds(0.05f);

                        Move(moveX, moveY);
                    }
                    else
                    {
                        FinishMovement();
                    }
                }
                else
                {
                    FinishMovement();
                }
            }
        }

        public void AddStep()
        {   
            if (chrStats.curStamina >= moveStaminaCost + 10)
            {
                moveSteps++;
                moveStaminaCost += 10;
            }

            UpdateStepsText();
        }

        public void RemoveStep()
        {
            moveSteps--;
            moveStaminaCost -= 10;

            if (moveSteps < 1) 
            {
                moveSteps = 1;
                moveStaminaCost = 0;
            }

            UpdateStepsText();
        }

        public void SwitchMovementTypes()
        {
            if (curMovementType == MovementType.Ground)
            {
                curMovementType = MovementType.Air;
                Sprite icon = LevelManager.instance.airIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                chrStats.moveTypeIcon.sprite = icon;
            }
            else if (curMovementType == MovementType.Air)
            {
                curMovementType = MovementType.Ground;
                Sprite icon = LevelManager.instance.groundIcon;
                LevelManager.instance.switchMovementTypesButton.GetComponent<Image>().sprite = icon;
                chrStats.moveTypeIcon.sprite = icon;
            }
        }

        public void UpdateStepsText()
        {
            LevelManager.instance.stepsNumber.text = moveSteps.ToString();
            LevelManager.instance.moveStaminaCost.text = moveStaminaCost.ToString();
        }

        private void FinishMovement()
        {
            chrStats.TurnFinished();
            chrAnim.SetIdleAnimation();
            QueueManager.instance.MoveQueue();
        }

        private bool IsThisNewPositionPossible(int moveX, int moveY)
        {
            Vector2 curPos = transform.position;
            Vector2 newPos = new Vector2(curPos.x + moveX, curPos.y + moveY);

            return TilemapManager.instance.IsNewPositionPossible(gameObject, sizeMatrix, moveX, moveY, curMovementType);
        }

        public void Flip(bool useTurnFinished = true)
        {
            StartCoroutine(FlipCoroutine(useTurnFinished));
        }

        private IEnumerator FlipCoroutine(bool useTurnFinished = true)
        {
            if (curFacingDirection == FacingDirection.Right)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                curFacingDirection = FacingDirection.Left;
            }
            else 
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                curFacingDirection = FacingDirection.Right;
            }

            if (useTurnFinished) 
            {
                chrStats.CheckCards();
                yield return new WaitForSeconds(1f);
                chrStats.TurnFinished();
                QueueManager.instance.MoveQueue();
            }
        }

    #endregion

}