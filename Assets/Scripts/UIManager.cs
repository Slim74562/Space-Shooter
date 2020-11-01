using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _fireballImage;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Image _homingMissileImg;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Text _liveText;
    [SerializeField]
    private Text _waveText;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _waveText.enabled = false;
        _scoreText.text = "Score: 0";
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        _liveText.enabled = false;

        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is Null");
        }
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateAmmo(int playerAmmo)
    {
        _ammoText.text = "Ammo: " + playerAmmo + " / 15";
    }

    public void UpdateWave(string currentWave)
    {
        _waveText.enabled = true;
        _waveText.text = "Wave: " + currentWave;
        StartCoroutine(WaveTextDisableCountdown());
    }

    IEnumerator WaveTextDisableCountdown()
    {
        yield return new WaitForSeconds(1f);
        _waveText.enabled = false;
        yield return new WaitForSeconds(1f);
        _waveText.color = Color.blue;
        _waveText.enabled = true;
        yield return new WaitForSeconds(1f);
        _waveText.color = new Color(114, 0, 0, 150);
        _waveText.enabled = false;
        GameObject.FindGameObjectWithTag("Respawn").GetComponent<SpawnManager>().StartSpawning();
    }

    public void UpdateLives(int currentLives)
    {       
        if (currentLives > 0 && currentLives <= 3)
        {
            _liveText.enabled = false;
            _livesImg.sprite = _liveSprites[currentLives];           
            _livesImg.enabled = true;
        }
        else if (currentLives > 3)
        {
            _liveText.text = "Lives: " + currentLives;
            _livesImg.enabled = false;
            _liveText.enabled = true;            
        }
        if (currentLives == 0)
        {
            _livesImg.sprite = _liveSprites[currentLives];
            GameOverSequence();
        }
    }

    public void UpdateFireball(bool has)
    {
        if (has)
        {
            _fireballImage.enabled = true;
        }
        else
        {
            _fireballImage.enabled = false;
        }

    }

    public void UpdateHomingMissile(bool has)
    {
        if (has)
        {
            _homingMissileImg.enabled = true;
        }
        else
        {
            _homingMissileImg.enabled = false;
        }
    }

    void GameOverSequence()
    {

        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());        
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "Game Over";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
