using System;
using System.Collections;
using System.Collections.Generic;
using BuildSystem;
using Manager;
using TMPro;
using UI.Tab;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldScene
{
    public class WorldSceneUI : MonoBehaviour
    {
        [field: Header("Create World")]
        [SerializeField] private Canvas _SystemCanvas;
        [SerializeField] private Canvas _CreateWorldCanvas;
        [SerializeField] private Button _CreateWorldButton;

        private const string SPEED_DISPLAY = "Speed: 1:";
        private const string PAUSE_DISPLAY = "PAUSE";
        
        [field: Header("System"), SerializeField]
        private Button _SpeedUp;
        [SerializeField] private Button _SpeedDown;
        [SerializeField] private Button _Pause;
        [SerializeField] private TMP_Text _SpeedTextDisplay;
        
        [field: Header("Transition")]
        [SerializeField] private Canvas _TransitionCanvas;
        [SerializeField] private Button _StartWaveButton;
        [SerializeField] private TMP_Text _WaveInfoText;
        private const string LEVEL_INFO_PRE_TEXT = "Level: ";
        
        [field: Header("Play")]
        [SerializeField] private Canvas _PlayCanvas;
        [SerializeField] private TMP_Text _EnemyCountInfo;
        [SerializeField] private TMP_Text _GoldAmountInfo;
        [SerializeField] private TMP_Text _PlayerHealthInfo;
        private const string ENEMY_COUNT_PRE_TEXT = "Enemy count: ";
        private const string GOLD_AMOUNT_PRE_TEXT = "Gold: ";
        private const string PLAYER_HEALTH_PRE_TEXT = "Health: ";
        
        [field: Header("Building")]
        [SerializeField] private Canvas _BuildingCanvas;
        [SerializeField] public Transform _BuildingListFrame;
        [SerializeField] public TabGroup _SystemTabGroup;
        
                
        [field: Header("GameOver")]
        [SerializeField] private Canvas _GameOverCanvas;
        [SerializeField] public Button _ResetButton;
        [SerializeField] public Button _MenuButton;

        private void Awake()
        {
            MenuUIManager.WorldSceneUI = this;
            
            _CreateWorldButton.onClick.AddListener(CreateWorld);
            _SpeedUp.onClick.AddListener(SpeedUp);
            _Pause.onClick.AddListener(PauseGame);
            _SpeedDown.onClick.AddListener(SpeedDown);
            _StartWaveButton.onClick.AddListener(StartWave);
            _ResetButton.onClick.AddListener(ResetWorld);
            _MenuButton.onClick.AddListener(BackToMenu);
            GameManager.Instance.OnLose += GameOverScreen;
            
            // Building
            _SystemTabGroup.OnTabChange += () => StartCoroutine(LastFrame(() =>
                GameManager.Instance.SetBuildingManager(_BuildingCanvas.gameObject.activeInHierarchy)));
            
            GameManager.Instance.OnWorldSettingChange += UpdateCanvas;
            
            SetupPlay();
        }

        private void GameOverScreen()
        {
            _GameOverCanvas.gameObject.SetActive(true);
            _BuildingCanvas.gameObject.SetActive(false);
            _PlayCanvas.gameObject.SetActive(false);
            _TransitionCanvas.gameObject.SetActive(false);
            _SystemCanvas.gameObject.SetActive(false);
        }

        private void BackToMenu()
        {
            GameManager.Instance.MenuReset();
        }

        private void ResetWorld()
        {
            throw new NotImplementedException();
        }

        private void UpdateCanvas(GameManager.WorldSetting stage)
        {
            _PlayCanvas.gameObject.SetActive(false);
            
            switch (stage)
            {
                case GameManager.WorldSetting.Menu:
                    break;
                case GameManager.WorldSetting.Create:
                    _CreateWorldCanvas.gameObject.SetActive(true);
                    break;
                case GameManager.WorldSetting.Play:
                    _PlayCanvas.gameObject.SetActive(true);
                    break;
                case GameManager.WorldSetting.Transition:
                    _PlayCanvas.gameObject.SetActive(true);
                    _TransitionCanvas.gameObject.SetActive(true);
                    PortrayLevelInfo();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
            }
        }

        private void SetupPlay()
        {
            // Player info
            var playerClass = GameManager.Instance.GetPlayerClassInfo();
            playerClass.OnGoldChange += SetGoldAmountText;
            playerClass.OnHealthChange += SetPlayerHealthText;
            SetGoldAmountText(playerClass.Gold);
            SetPlayerHealthText(playerClass.Health);
            
            // Enemy info
            GameManager.Instance.SubscribeToEnemyAmountChange(SetEnemyCountText);
        }
        
        private void PortrayLevelInfo()
        {
            _WaveInfoText.text = LEVEL_INFO_PRE_TEXT + GameManager.Instance.CurrentLevel;
        }

        private void SetPlayerHealthText(uint health) => _PlayerHealthInfo.text = PLAYER_HEALTH_PRE_TEXT + health;
        private void SetEnemyCountText(uint health) => _EnemyCountInfo.text = ENEMY_COUNT_PRE_TEXT + health;
        private void SetGoldAmountText(uint health) => _GoldAmountInfo.text = GOLD_AMOUNT_PRE_TEXT + health;

        private void StartWave()
        {
            _TransitionCanvas.gameObject.SetActive(false);
            GameManager.Instance.StartWave();
        }

        private void CreateWorld()
        {
            if (!GameManager.Instance.CreateWorld()) return;
            _CreateWorldCanvas.gameObject.SetActive(false);
            _SystemCanvas.gameObject.SetActive(true);
        }

        private void SpeedUp()
        {
            if(GameManager.Instance.IsPaused) return;
            GameManager.Instance.SpeedUp();
            _SpeedTextDisplay.text = SPEED_DISPLAY + GameManager.Instance.GameSpeed;
        }
        
        private void PauseGame()
        {
            GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
            _SpeedTextDisplay.text = GameManager.Instance.IsPaused
                ? PAUSE_DISPLAY
                : SPEED_DISPLAY + GameManager.Instance.GameSpeed;
        }
        
        private void SpeedDown()
        {
            if(GameManager.Instance.IsPaused) return;
            GameManager.Instance.SpeedDown();
            _SpeedTextDisplay.text = SPEED_DISPLAY + GameManager.Instance.GameSpeed;
        }
        
        private void OnDestroy()
        {
            var playerClass = GameManager.Instance.GetPlayerClassInfo();
            playerClass.OnGoldChange -= SetGoldAmountText;
            playerClass.OnHealthChange -= SetPlayerHealthText;
            
            GameManager.Instance.UnsubscribeToEnemyAmountChange(SetEnemyCountText);
            
        }

        public IEnumerator LastFrame(Action call)
        {
            yield return new WaitForEndOfFrame();
            call?.Invoke();
        }
    }
}
