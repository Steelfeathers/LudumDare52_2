using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FirebirdGames.Utilities;
using FirebirdGames.Utilities.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LudumDare52_2
{
    public static class WordDictionary
    {
        public static readonly string[] THREE_LETTER_WORDS =
        {
            "hoe", "dig", "cow", "cat", "dog", "pig", "sun", "hay", "row", "sow", "ewe", "ram", "pea", "tea", "hen", "bee", "egg",
            "hog", "rye", "oat"
        };
        public static readonly string[] FOUR_LETTER_WORDS =
        {
            "crop", "farm", "seed", "barn", "rain", "plant", "calf", "foal", "mare", "bull", "rose", "taro", "wild", "okra", "lily", 
            "root", "rice", "clay", "silt", "corn", "soil", "till", "hemp", "pear", "bush", "tree", "iris", "beet", "grow", "milk",
            "goat", "weed", "plow", "acre", "wasp", "coop", "crow", "duck", "food", "gate", "lamb", "land", "mule", "oats", "pail",
            "plow", "pick", "ripe", "rake", "reap", "silo", "tend"
        };
        public static readonly string[] FIVE_LETTER_WORDS =
        {
            "tulip", "fence", "horse", "sheep", "plant", "steer", "wagon", "basil", "wheat", "poppy", "chard", "onion", "daisy", "water", 
            "berry", "apple", "melon", "thyme", "grape", "chick", "bread", "llama", "beans", "breed", "dairy", "field", "flock", "fruit",
            "goose", "geese", "grain", "honey", "mower", "mulch", "ranch", "roost", "swine", "udder"
        };
        public static readonly string[] SIX_LETTER_WORDS =
        {
            "potato", "tomato", "shovel", "harness", "pepper", "cherry", "radish", "coffee", "turnip", "squash", "barley", "cactus", 
            "carrot", "garlic", "cotton", "growth", "scythe", "piglet", "animal", "bucket", "cattle", "donkey", "fallow", "farmer",
            "gander", "grains", "meadow", "rabbit", "shears", "sickle", "stable", "turkey", "trough", "tiller", "trowl"
        };
        public static readonly string[] SEVEN_LETTER_WORDS =
        {
            "soybean", "tomato", "harvest", "tractor", "rooster", "lettuce", "pumpkin", "cabbage", "drought", "beehive", "chicken",
            "combine", "orchard", "poultry", "pasture", "produce"
        }; 
    }

    public class GameDialog : Dialog
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Transform lifeWidgetHolder;
        [SerializeField] private LifeWidget lifeWidgetPrefab;

        [Header("Field")] 
        [SerializeField] private ParticleSystem tractorSmokeFX;
        [SerializeField] private Transform tractorHolder;
        [SerializeField] private RectTransform pickupTrigger;
        [SerializeField] private RectTransform endLevelTrigger;
        [SerializeField] private RectTransform showEndDialogTrigger;
        [SerializeField] private Transform cropRowsHolder;
        [SerializeField] private GameObject cropSlotPrefab;
        [SerializeField] private List<Transform> cropRows;

        private List<LifeWidget> lifeWidgets = new List<LifeWidget>();
      
        private List<Crop> crops = new List<Crop>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<Vector2> obstacleGridPositions = new List<Vector2>();

        private List<string[]> wordArrayList = new List<string[]>();

        private DifficultySettings curLevelSettings;
        
        private Obstacle curObstacle;
        private string curInput;
        private string prevInput;

        private int curLives;
        private float tractorDamagedTimer;

        private bool isDone;
        private bool isGameOver;
        private bool isVictory;
        private bool isPaused;

        public override void Initialize(object config = null, Action onClosedCallback = null)
        {
            base.Initialize(config, onClosedCallback);
            
            inputField.ActivateInputField();

            wordArrayList = new List<string[]>()
            {
                WordDictionary.THREE_LETTER_WORDS, WordDictionary.FOUR_LETTER_WORDS,
                WordDictionary.FIVE_LETTER_WORDS, WordDictionary.SIX_LETTER_WORDS,
                WordDictionary.SEVEN_LETTER_WORDS
            };

            SetupLevel(); 
        }

        public override void AnimateInComplete()
        {
            base.AnimateInComplete();
            AnimUtils.PlayAllChildTweensWithID(tractorHolder.gameObject, "Idle");
        }

        public void SetupLevel()
        {
            //fetch the difficulty settings
            curLevelSettings = MyGameRoot.Instance.LevelDifficultySettings[MyGameRoot.Instance.CurLevel];
            
            //Setup lives display
            curLives = curLevelSettings.MaxLives;
            for (int i = 0; i < curLives; i++)
            {
                var lifeWidget = GameObject.Instantiate(lifeWidgetPrefab, lifeWidgetHolder).GetComponent<LifeWidget>();
                lifeWidgets.Add(lifeWidget);
            }
            
            //Setup score display
            MyGameRoot.Instance.CurScore = 0;
            UpdateScore();

            int obstaclesInCol = 0;
            int colsWithoutObstacles = 0;
            bool spawnedObstacleInThisCol = false;
            for (int x = 0; x < curLevelSettings.LevelLength; x++) //down first, then across, in 'waves'
            {
                if (!spawnedObstacleInThisCol) colsWithoutObstacles += 1;
                spawnedObstacleInThisCol = false;
                obstaclesInCol = 0;
                
                for (int y = 0; y < cropRows.Count; y++)
                {
                    var slot = GameObject.Instantiate(cropSlotPrefab, cropRows[y]); //spawn placeholder to make sure everything sizes correctly
                    
                    //Spawn *something*
                    if (Random.Range(0f, 1f) < curLevelSettings.GeneralSpawnChance) 
                    {
                        //If there was already an obstacle spawned right next to this one, reduce the chance of spawning again
                        bool obstLeft = obstacleGridPositions.Contains(new Vector2(x - 1, y));
                        bool obstUp = obstacleGridPositions.Contains(new Vector2(x, y - 1));
                        float calcedSpawnChance = obstLeft || obstUp
                            ? curLevelSettings.ObstacleSpawnChance * 0.5f
                            : curLevelSettings.ObstacleSpawnChance;

                        if (colsWithoutObstacles > curLevelSettings.MaxColsWithoutObstacles)
                        {
                            float t = (float)(colsWithoutObstacles - curLevelSettings.MaxColsWithoutObstacles);// / (float)curLevelSettings.MaxColsWithoutObstacles;
                            calcedSpawnChance += t * 0.33f; //= Mathf.Lerp(calcedSpawnChance, 1f, t);
                        }

                        //spawn obstacle
                        if (x > curLevelSettings.ColsBeforeObstacles && obstaclesInCol < curLevelSettings.MaxObstaclesPerCol && Random.Range(0f, 1f) < calcedSpawnChance)
                        {
                            var wordList = wordArrayList.GetWeightedRandom(curLevelSettings.WordListWeights).ToList();

                            var obstaclePrefab = curLevelSettings.ObstaclePrefabs.GetWeightedRandom(curLevelSettings.ObstaclePrefabWeights);
                            var obstacle = GameObject.Instantiate(obstaclePrefab.gameObject, slot.transform).GetComponent<Obstacle>();
                            obstacle.Setup(wordList.GetRandom());
                            obstacles.Add(obstacle);

                            obstacleGridPositions.Add(new Vector2(x, y));
                            obstaclesInCol += 1;
                            spawnedObstacleInThisCol = true;
                            colsWithoutObstacles = 0;
                        }
                        //spawn crop
                        else
                        {
                            var cropPrefab = curLevelSettings.CropPrefabs.GetWeightedRandom(curLevelSettings.CropPrefabWeights);
                            var crop = GameObject.Instantiate(cropPrefab.gameObject, slot.transform).GetComponent<Crop>();
                            crop.gameObject.SetActive(true);
                            crops.Add(crop);
                        }
                    }
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(cropRowsHolder.gameObject.RectTransform());
        }

        public void OnInputDeselect()
        {
            inputField.ActivateInputField();
        }

        public void OnInputChanged()
        {
            if (isPaused || isGameOver) return;
            
            //Get new input
            curInput = inputField.text;
            
            //Clear progress on all obstacles
            curObstacle = null;
            obstacles.ForEach(o => { if (o != null) o.ClearProgress(); });
            
            //When backspacing, remove entire word, clear any progress
            if (!string.IsNullOrEmpty(prevInput) && curInput.Length < prevInput.Length)
            {
                inputField.text = string.Empty;
                curInput = string.Empty;
            }

            if (!string.IsNullOrEmpty(curInput))
            {
                //check if the typed string matches any words - if there are multiple matches, select the one closest to the pickup point
                float minDist = Single.PositiveInfinity;
                foreach (var obstacle in obstacles)
                {
                    //Word too long for this obstacle or obstacle already removed
                    if (obstacle == null || obstacle.WasRemoved || curInput.Length > obstacle.MyWord.Length)
                        continue;
                    
                    if (string.Equals(obstacle.MyWord[..curInput.Length], curInput, StringComparison.OrdinalIgnoreCase))
                    {
                        float dist = Mathf.Abs(pickupTrigger.position.x - obstacle.gameObject.transform.position.x);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            curObstacle = obstacle;
                            
                            //check for exact match, then remove
                            if (string.Equals(curObstacle.MyWord, curInput, StringComparison.OrdinalIgnoreCase))
                            {
                                curObstacle.WasRemoved = true;
                                curObstacle.PlayRemoveFX();
                                curObstacle = null;

                                inputField.text = string.Empty;
                                curInput = string.Empty;
                                break;
                            }
                        }
                    }
                }

                //If not exact match, update progress on the word
                if (curObstacle != null)
                {
                    curObstacle.UpdateProgress(curInput.Length);
                }
            }

            prevInput = curInput;
            
        }

        private void UpdateScore(int amt=0)
        {
            MyGameRoot.Instance.CurScore += amt;
            scoreText.text = $"Score: {MyGameRoot.Instance.CurScore}";
        }

        private void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPaused = !isPaused;
                inputField.enabled = !isPaused;
            }
            #endif
            
            if (isPaused || isDone) return;

            //On victory, keep plowing until the end of the row
            if (isGameOver && isVictory)
            {
                if (Utils.RectOverlaps(pickupTrigger, showEndDialogTrigger))
                {
                    isDone = true;
                    //UIManager.Instance.ShowOverlayDialog(MyGameRoot.Instance.GameVictoryDialogPrefab);
                }
                else
                {
                    tractorHolder.transform.position = new Vector3(tractorHolder.transform.position.x + (Time.deltaTime * curLevelSettings.TractorMoveSpeed), tractorHolder.transform.position.y, tractorHolder.transform.position.z);
                }
                return;
            }

            //Increment tractor damaged cooldown timer
            if (tractorDamagedTimer > 0)
            {
                tractorDamagedTimer -= Time.deltaTime;
                if (tractorDamagedTimer <= 0)
                {
                    PlayTractorFixedFX();
                }
            }
            
            //Move all the crops across the screen
            float moveAmt = curLevelSettings.TractorMoveSpeed * Time.deltaTime;
            cropRowsHolder.transform.position = new Vector3(cropRowsHolder.position.x - moveAmt, cropRowsHolder.position.y, cropRowsHolder.position.z);

            //Harvest crops and add to score if past the pickup point
            foreach (var crop in crops)
            {
                if (crop.WasHarvested) continue;
                if (Utils.RectOverlaps(crop.gameObject.RectTransform(), pickupTrigger))
                {
                    if (tractorDamagedTimer > 0) //tractor stalled, don't harvest crop
                    {
                        crop.WasHarvested = true;
                    }
                    else
                    {
                        UpdateScore(crop.PointValue);
                        crop.WasHarvested = true;
                        crop.PlayHarvestFX();
                    }
                }
            }
            //Check if any obstacles hit the machine, and if so, deduct life and stall the machine
            foreach (var obstacle in obstacles)
            {
                if (obstacle.WasRemoved) continue;
                if (Utils.RectOverlaps(obstacle.gameObject.RectTransform(), pickupTrigger))
                {
                    tractorDamagedTimer = curLevelSettings.TractorDamageCooldown;
                    LoseLife();
                    PlayTractorDamagedFX();
                    obstacle.WasRemoved = true;
                    obstacle.PlayDestroyedFX();
                }
            }
            
            //check if we've reached the end of the level
            if (Utils.RectOverlaps(pickupTrigger, endLevelTrigger))
            {
                EndGame(true);
            }
        }

        private void LoseLife()
        {
            curLives -= 1;
            if (curLives < 0) curLives = 0;
            
            for (int i = curLevelSettings.MaxLives - 1; i >= curLives; i--)
            {
                lifeWidgets[i].SetFull(false);
            }

            if (curLives <= 0)
            {
                EndGame(false);
            }
        }

        private void EndGame(bool victory)
        {
            isGameOver = true;
            isVictory = victory;
            inputField.enabled = false;

            if (victory)
            { 
                UIManager.Instance.ShowOverlayDialog(MyGameRoot.Instance.GameVictoryDialogPrefab);
            }
            else
            {
                isDone = true;
                UIManager.Instance.ShowOverlayDialog(MyGameRoot.Instance.GameLossDialogPrefab);
            }
        }
        
        private void PlayTractorDamagedFX()
        {
            tractorSmokeFX.gameObject.SetActive(true);
            tractorSmokeFX.Play();
            AnimUtils.KillAllChildTweensWithID(tractorHolder.gameObject, "Idle");
            AnimUtils.PlayAllChildTweensWithID(tractorHolder.gameObject, "Damaged");
        }

        private void PlayTractorFixedFX()
        {
            AnimUtils.KillAllChildTweensWithID(tractorHolder.gameObject, "Damaged");
            AnimUtils.PlayAllChildTweensWithID(tractorHolder.gameObject, "Idle");
            tractorSmokeFX.Stop();
        }


    }
}
