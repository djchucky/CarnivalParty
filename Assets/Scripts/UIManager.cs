using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Singleton setting
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if(_instance == null)
            {
                Debug.Log("Instance is null on UIManager");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        if (_chargeBar != null)
            _chargeBar.sprite = _chargeBarSprites[0];

    }

    //Variables

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _buttons;

    [Header("ChargeBar")]
    [SerializeField] private Sprite[] _chargeBarSprites;
    [SerializeField] private Image _chargeBar;

    private void OnEnable()
    {
        Launcher.OnChargeBar += UpdateChargeBar;
    }

    private void OnDisable()
    {
        Launcher.OnChargeBar -= UpdateChargeBar;
    }

    public void UpdateTimer(float timer)
    {
        _timerText.text = "Timer: " + timer.ToString();
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateTargets(int targetHit,int targetLeft)
    {
        _targetText.text = "Target: " + targetHit.ToString() + " / " + targetLeft.ToString();
    }

    public void WinScreen()
    {
        _winScreen.SetActive(true);
        _buttons.SetActive(true);
    }

    public void GameOverScreen()
    {
        _loseScreen.SetActive(true);
        _buttons.SetActive(true);
    }

    public void UpdateChargeBar(float power,float minPower, float maxPower)
    {
        Debug.Log("Event called");
        // Clamp values
        power = Mathf.Clamp(power, minPower, maxPower);

        // Normalize values between 0 and 1
        float normalized = power / maxPower;

        // Calculate next step
        int index = Mathf.RoundToInt(normalized * (_chargeBarSprites.Length - 1));

        // Update sprite
        _chargeBar.sprite = _chargeBarSprites[index];
    }
}
