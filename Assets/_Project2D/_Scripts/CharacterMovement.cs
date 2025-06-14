using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyGame;

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

        [Header("AUTO")]

            [Header("Data")]
            public GameObject closestOpponent;

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
        }

        /// <summary>
        /// Called once per frame.
        /// Use for logic that needs to run every frame, such as user input or animations.
        /// </summary>
        private void Update()
        {
            if (chrStats.onTurn)
            {
                if (Input.GetKeyDown(InputManager.instance.addStepKey))
                {
                    AddStep();
                }
                else if (Input.GetKeyDown(InputManager.instance.removeStepKey))
                {
                    RemoveStep();
                }
            }
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

        public void StartAuto()
        {
            StartCoroutine(Auto());
        }

        private IEnumerator Auto()
        {
            yield return new WaitForSeconds(1f);

            List<CardSO> canInteractCards = new List<CardSO>();

            for (int i = 1; i <= 3; i++)
            {
                if (i == 1)
                {
                    TilemapManager.instance.ShowTrajectory(chrCards.cardOne);     

                    if (chrCards.cardOne.canInteract)
                    {
                        TilemapManager.instance.HideTrajectory(chrCards.cardOne);       
                        canInteractCards.Add(chrCards.cardOne);
                    }  
                    
                    TilemapManager.instance.HideTrajectory(chrCards.cardOne);       
                }
                else if (i == 2)
                {
                    TilemapManager.instance.ShowTrajectory(chrCards.cardTwo);     

                    if (chrCards.cardTwo.canInteract)
                    {
                        TilemapManager.instance.HideTrajectory(chrCards.cardTwo);       
                        canInteractCards.Add(chrCards.cardTwo);
                    }  
                    
                    TilemapManager.instance.HideTrajectory(chrCards.cardTwo);       
                }
                else if (i == 3)
                {
                    TilemapManager.instance.ShowTrajectory(chrCards.cardThree);     

                    if (chrCards.cardThree.canInteract)
                    {
                        TilemapManager.instance.HideTrajectory(chrCards.cardThree);       
                        canInteractCards.Add(chrCards.cardThree);
                    }  
                    
                    TilemapManager.instance.HideTrajectory(chrCards.cardThree);       
                }
            }

            if (canInteractCards.Count != 0)
            {
                Data.ShuffleList(canInteractCards);
                bool canAttack = false;

                foreach (CardSO canInteractCard in canInteractCards)
                {   
                    if (canInteractCard == chrCards.cardOne && chrStats.curStamina >= chrCards.cardOne.staminaCost)
                    {
                        chrCards.FirstCard();
                        canAttack = true;
                        break;
                    }
                    else if (canInteractCard == chrCards.cardTwo && chrStats.curStamina >= chrCards.cardTwo.staminaCost)
                    {
                        chrCards.SecondCard();
                        canAttack = true;
                        break;
                    }
                    else if (canInteractCard == chrCards.cardThree && chrStats.curStamina >= chrCards.cardThree.staminaCost)
                    {
                        chrCards.ThirdCard();
                        canAttack = true;
                        break;
                    }
                }

                if (!canAttack)
                {
                    chrStats.Wait();
                }
            }
            else
            {
                Transform closestOppTransform = FindClosestOpponent().transform;

                if (transform.position.x > closestOppTransform.position.x && curFacingDirection == FacingDirection.Right)
                {             
                    Flip();
                }
                else if (transform.position.x >= closestOppTransform.position.x && curFacingDirection == FacingDirection.Left)
                {
                    int rndMoveSteps = UnityEngine.Random.Range(1, 5);
                    moveSteps = rndMoveSteps;
                    int moveCost = moveSteps * 10 - 10;

                    if (transform.position.y > closestOppTransform.position.y)
                    {
                        if (IsThisNewPositionPossible(-1, 1))
                        {
                            Move(-1, 1);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else
                        {
                            chrStats.Wait();
                        }
                    }
                    else if (transform.position.y < closestOppTransform.position.y)
                    {
                        if (IsThisNewPositionPossible(-1, -1))
                        {
                            Move(-1, -1);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else
                        {
                            chrStats.Wait();
                        }
                    }
                    else if (transform.position.y == closestOppTransform.position.y)
                    {
                        if (IsThisNewPositionPossible(-1, 1))
                        {
                            Move(-1, 1);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else if (IsThisNewPositionPossible(-1, -1))
                        {
                            Move(-1, -1);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else if (IsThisNewPositionPossible(-1, 0))
                        {
                            Move(-1, 0);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else
                        {
                            chrStats.Wait();
                        }
                    }
                }
                else if (transform.position.x < closestOppTransform.position.x && curFacingDirection == FacingDirection.Left)
                {             
                    Flip();
                }
                else if (transform.position.x <= closestOppTransform.position.x && curFacingDirection == FacingDirection.Right)
                {
                    int rndMoveSteps = UnityEngine.Random.Range(1, 5);
                    moveSteps = rndMoveSteps;
                    int moveCost = moveSteps * 10 - 10;

                    if (transform.position.y > closestOppTransform.position.y)
                    {
                        if (IsThisNewPositionPossible(1, -1))
                        {
                            Move(1, -1);                        
                            chrStats.RemoveStamina(moveCost);
                        }
                        else if (IsThisNewPositionPossible(1, 0))
                        {
                            Move(1, 0);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else
                        {
                            chrStats.Wait();
                        }
                    }
                    else if (transform.position.y < closestOppTransform.position.y)
                    {
                        if (IsThisNewPositionPossible(1, 1))
                        {
                            Move(1, 1);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else if (IsThisNewPositionPossible(1, 0))
                        {
                            Move(1, 0);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else
                        {
                            chrStats.Wait();
                        }
                    }
                    else if (transform.position.y == closestOppTransform.position.y)
                    {
                        if (IsThisNewPositionPossible(1, -1))
                        {
                            Move(1, -1);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else if (IsThisNewPositionPossible(1, 1))
                        {
                            Move(1, 1);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else if (IsThisNewPositionPossible(1, 0))
                        {
                            Move(1, 0);   
                            chrStats.RemoveStamina(moveCost);
                        }
                        else
                        {
                            chrStats.Wait();
                        }
                    }
                }
            }
        }

        public GameObject FindClosestOpponent()
        {
            if (chrStats.curCharacterType == CharacterType.Player)
            {
                float ?minDis = null;
                GameObject closestOpp = null;

                foreach (CharacterStatistics opp in LevelManager.instance.enemyCharacters)
                {
                    float dis = Vector2.Distance(transform.position, opp.transform.position);
                    
                    if (minDis == null)
                    {
                        minDis = dis;
                        closestOpp = opp.gameObject;
                    }
                    else
                    {
                        if (dis < minDis)
                        {
                            minDis = dis;
                            closestOpp = opp.gameObject;
                        }
                    }
                }
                
                return closestOpp;
            }
            else if (chrStats.curCharacterType == CharacterType.Enemy)
            {
                float ?minDis = null;
                GameObject closestOpp = null;

                foreach (CharacterStatistics opp in LevelManager.instance.playerCharacters)
                {
                    float dis = Vector2.Distance(transform.position, opp.transform.position);
                    
                    if (minDis == null)
                    {
                        minDis = dis;
                        closestOpp = opp.gameObject;
                    }
                    else
                    {
                        if (dis < minDis)
                        {
                            minDis = dis;
                            closestOpp = opp.gameObject;
                        }
                    }
                }
                
                return closestOpp;
            }

            return null;
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

                if (Input.GetKeyDown(InputManager.instance.upperLeftKey))
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
                else if (Input.GetKeyDown(InputManager.instance.upperKey))
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
                else if (Input.GetKeyDown(InputManager.instance.upperRightKey))
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
                else if (Input.GetKeyDown(InputManager.instance.midLeftKey))
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
                else if (Input.GetKeyDown(InputManager.instance.midRightKey))
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
                else if (Input.GetKeyDown(InputManager.instance.lowerLeftKey))
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
                else if (Input.GetKeyDown(InputManager.instance.lowerKey))
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
                else if (Input.GetKeyDown(InputManager.instance.lowerRightKey))
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

                if (Input.GetKeyDown(InputManager.instance.flipKey))
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

        public void Move(int moveX, int moveY, bool checkFall = true, bool specialCheck = false)
        {
            Vector2 curPos = transform.position;
            Vector2 newPos = new Vector2(curPos.x + moveX, curPos.y + moveY);

            if (TilemapManager.instance.IsNewPositionPossible(gameObject, sizeMatrix, moveX, moveY, curMovementType))
            {
                StartCoroutine(MoveToNewPos(newPos, moveX, moveY, checkFall, specialCheck));
            }
        }

        private IEnumerator MoveToNewPos(Vector3 newPos, int moveX, int moveY, bool checkFall = true, bool specialCheck = false)
        {
            if (checkFall && !specialCheck) 
            {
                chrAnim.SetRunningAnimation();            
                SFXManager.PlaySFX(Resources.Load<AudioClip>("SFX/Walking"), transform, 1f);
            }
            moveSteps--;    

            if (specialCheck) 
            {
                moveSteps = 1;
                StartCheckFall();
                yield break;
            }
            
            while (transform.parent.position != newPos)
            {
                transform.parent.position = Vector2.MoveTowards(transform.parent.position, newPos, 0.05f);

                yield return new WaitForSeconds(0.01f);
            }

            transform.parent.position = newPos;

            if (curMovementType == MovementType.Ground)
            {
                if (moveSteps > 0)
                {
                    if (IsThisNewPositionPossible(moveX, moveY))
                    {
                        Move(moveX, moveY);
                    }
                    else
                    {
                        moveSteps = 0;
                        if (checkFall) StartCheckFall();
                    }
                }
                else
                {
                    if (checkFall) StartCheckFall();
                }
            }
            else
            {
                if (moveSteps > 0)
                {
                    if (IsThisNewPositionPossible(moveX, moveY))
                    {
                        Move(moveX, moveY);
                    }
                    else
                    {
                        if (checkFall) FinishMovement();
                    }
                }
                else
                {
                    if (checkFall) FinishMovement();
                }
            }
        }

        public void StartCheckFall()
        {   
            StartCoroutine(CheckFall());   
        }

        public IEnumerator CheckFall()
        {
            yield return new WaitForSeconds(0.01f);

            if (IsThisNewPositionPossible(0, -1))
            {
                Move(0, -1);   
            }
            else
            {
                FinishMovement();
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

            if (QueueManager.instance.fallingCharacters.Count == 0)
            {
                QueueManager.instance.CheckFallingCharacters();
            }
            else
            {
                QueueManager.instance.FindFallingCharacters();
                QueueManager.instance.MoveFallQueue();
            }
        }

        public bool IsThisNewPositionPossible(int moveX, int moveY)
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