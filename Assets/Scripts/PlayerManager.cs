using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject ninjaPrefab;
    [SerializeField] private GameObject ghostPrefab;
    private PlayerInput _ninja;
    private PlayerInput _ghost;
    private InputDevice _ninjaInput = Keyboard.current;
    private InputDevice _ghostInput = Keyboard.current;

    private string _ghostActionMap = "Player2";
    private bool _flip;

    private PlayerInputManager _playerInputManager;
    private ScoreTracker _scoreTracker;
    
    public float rumbleLowFrequency = 1f;
    public float rumbleHighFrequency = 1f;
    public float rumbleTime = 0.15f;
    public int iterations = 3;
    public float timeBetweenIterations = 0.2f;
    public RuntimeAnimatorController blueGhostAnimator;
    public RuntimeAnimatorController redGhostAnimator;
    public Material LineRendererBlue;
    public Material LineRendererRed;

    private SwapAnimation swapAnimation;
    private ScoreTracker _ScoreTracker;
    private SwapAnimation _swapAnimation;
    private MultiPlayerCamera _camera;
    
    public delegate void PerformSwap(bool flip);
    public static event PerformSwap OnSwap;



    private void Start()
    {
        _ScoreTracker = FindObjectOfType<ScoreTracker>();
        _swapAnimation = FindObjectOfType<SwapAnimation>();
        _camera = FindObjectOfType<MultiPlayerCamera>();
    }

    private void Awake()
    {
        swapAnimation = FindObjectOfType<SwapAnimation>();
       
        _scoreTracker = FindObjectOfType<ScoreTracker>();
        _playerInputManager = GetComponent<PlayerInputManager>();
        
        switch (Gamepad.all.Count)
        {
            case 1:
                _ninjaInput = Gamepad.all[0];
                break;
            case 2:
                _ninjaInput = Gamepad.all[0];
                _ghostInput = Gamepad.all[1];
                break;
            default:
                _ninjaInput = Keyboard.current;
                _ghostInput = Keyboard.current;
                break;
        }

        _playerInputManager.playerPrefab = ninjaPrefab;
        _ninja = _playerInputManager.JoinPlayer(-1, -1, controlScheme: null, _ninjaInput);
        _playerInputManager.playerPrefab = ghostPrefab;
        _ghost = _playerInputManager.JoinPlayer(-1, -1, controlScheme: null, _ghostInput);

        InputSystem.onDeviceChange += DeviceChange;
        InputUser.onChange += InputUserChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= DeviceChange;
        InputUser.onChange -= InputUserChange;
    }

    private void Update()
    {
        if (_ninja.GetComponent<Health>().health <= 0)
        {
            _ninja.GetComponent<Health>().health = 5;
            SwapInput();
        }
    }

    private void DeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
    {
        if (inputDeviceChange == InputDeviceChange.Added || inputDeviceChange == InputDeviceChange.Reconnected)
        {
            if (inputDevice == _ninjaInput)
                _ninja.SwitchCurrentControlScheme("Controller", inputDevice);
            if (inputDevice == _ghostInput)
                _ghost.SwitchCurrentControlScheme("Controller", inputDevice);

            if (_ninja.currentControlScheme == "Keyboard")
            {
                _ninja.SwitchCurrentControlScheme("Controller", inputDevice);
                _ninjaInput = inputDevice;
            }
            else if (_ghost.currentControlScheme == "Keyboard")
            {
                _ghost.SwitchCurrentControlScheme("Controller", inputDevice);
                _ghostInput = inputDevice;
            }
        }
    }

    private void InputUserChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (user == _ninja.user)
        {
            if (change == UnityEngine.InputSystem.Users.InputUserChange.DeviceLost)
                _ninja.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
        }
        if (user == _ghost.user)
        {
            if (change == UnityEngine.InputSystem.Users.InputUserChange.DeviceLost)
                _ghost.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
        }
    }

    public void SwapInput()
    {
        RespawnGhostAndRemoveEnemy();

        swapAnimation.Swap = true;
        Sound_Manager.playSound("Lightning");
        _camera.shake = 0.5;
        
        if (_ninja.currentControlScheme == "Keyboard" && _ghost.currentControlScheme == "Keyboard")
        {
            ModifyEntityInput(_ninja, "Keyboard", _ninja.currentActionMap.name == "Player1" ? "Player2" : "Player1", Keyboard.current);
            ModifyEntityInput(_ghost, "Keyboard", _ninja.currentActionMap.name == "Player1" ? "Player2" : "Player1", Keyboard.current);
        }
        else if (_ninja.currentControlScheme == "Controller" && _ghost.currentControlScheme == "Keyboard")
        {
            ModifyEntityInput(_ninja, "Keyboard", "Player2", Keyboard.current );
            ModifyEntityInput(_ghost, "Controller", "Player2", Gamepad.current);
        }
        else if (_ninja.currentControlScheme == "Keyboard" && _ghost.currentControlScheme == "Controller")
        {
            ModifyEntityInput(_ninja, "Controller", "Player1", Gamepad.current );
            ModifyEntityInput(_ghost, "Keyboard", "Player2", Keyboard.current );
        }
        else if (_ninja.currentControlScheme == "Controller" && _ghost.currentControlScheme == "Controller")
        {
            ModifyEntityInput(_ninja, "Controller", "Player1", Gamepad.all[Convert.ToInt32(!_flip)]);
            ModifyEntityInput(_ghost, "Controller", "Player2", Gamepad.all[Convert.ToInt32(_flip)]);
        }

        _flip = !_flip;
        _ghostActionMap = _ghost.currentActionMap.name;
        _scoreTracker.Player1Turn = !_scoreTracker.Player1Turn;
        StartCoroutine(StartBothRumble());
        OnSwap?.Invoke(_flip);
    }

    public void InstantiateEnemyWithInput(GameObject enemy, Vector3 pos)
    {
        var newEnemy = InstantiateEntity(enemy, _ghost.currentControlScheme, _ghost.currentActionMap.name, _ghostInput);
        newEnemy.SwitchCurrentActionMap(_ghost.currentActionMap.name);
        newEnemy.transform.position = pos;
        newEnemy.name = enemy.name;
    }

    public void RespawnGhostAndRemoveEnemy()
    {
        var enemyPos = Vector3.zero;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy.GetComponent<enemy_Health>().isPossessed)
            {
                enemyPos = enemy.transform.position;
                Destroy(enemy.gameObject);
            }
        }

        if (_ghost) return;

        _ghost = InstantiateEntity(ghostPrefab, _ghost.currentControlScheme, _ghostActionMap ,_ghostInput);
        _ghost.transform.position = enemyPos;
        if (_ScoreTracker.Player1Turn == true)
        {
            GameObject.FindGameObjectWithTag("Ghost").GetComponent<Animator>().runtimeAnimatorController = blueGhostAnimator;
            GameObject.Find("LR").GetComponent<Renderer>().material = LineRendererBlue;
            GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Stop();
            GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Clear();
            GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Play();
        }
        else if (_scoreTracker.Player1Turn == false)
        {
            GameObject.Find("LR").GetComponent<Renderer>().material = LineRendererRed;
            GameObject.FindGameObjectWithTag("Ghost").GetComponent<Animator>().runtimeAnimatorController = redGhostAnimator;
            GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Play();
            GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Stop();
            GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Clear();
        }
    }
    
    public void RespawnGhost(Vector3 pos)
    {
        _ghost = InstantiateEntity(ghostPrefab, _ghost.currentControlScheme, _ghostActionMap ,_ghostInput);
        _ghost.transform.position = pos;
        if (_ScoreTracker.Player1Turn == true)
        {
            GameObject.FindGameObjectWithTag("Ghost").GetComponent<Animator>().runtimeAnimatorController = blueGhostAnimator;
            GameObject.Find("LR").GetComponent<Renderer>().material = LineRendererBlue;
            GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Stop();
            GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Clear();
            GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Play();
        }
        else if (_scoreTracker.Player1Turn == false)
        {
            GameObject.Find("LR").GetComponent<Renderer>().material = LineRendererRed; 
            GameObject.FindGameObjectWithTag("Ghost").GetComponent<Animator>().runtimeAnimatorController = redGhostAnimator;
            GameObject.Find("redSpritesParticles").GetComponent<ParticleSystem>().Play();
            GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Stop();
            GameObject.Find("blueSpritesParticles").GetComponent<ParticleSystem>().Clear();
        }
            
    }

    private void ModifyEntityInput(PlayerInput player, string controlScheme, string actionMap, InputDevice inputDevice)
    {
        if (player == _ghost) _ghostInput = inputDevice;
        
        player.SwitchCurrentActionMap(actionMap);
        player.SwitchCurrentControlScheme(controlScheme, inputDevice);
    }

    private PlayerInput InstantiateEntity(GameObject entity, string scheme, string actionMap, InputDevice inputDevice)
    {
        if (entity == ghostPrefab) _ghostInput = inputDevice;
        var temp = PlayerInput.Instantiate(entity, controlScheme: scheme, pairWithDevices: inputDevice);
        temp.GetComponent<PlayerInput>().enabled = true;
        temp.SwitchCurrentActionMap(actionMap);
        return temp;
    }

    public void PlayerRumble(float lowFrequency, float highFrequency, float time)
    {
        if (_ninja.currentControlScheme == "Controller")
            StartCoroutine(StartPlayerRumble(lowFrequency, highFrequency, time));
    }

    public void GhostRumble(float lowFrequency, float highFrequency, float time)
    {
        if(_ghost.currentControlScheme == "Controller")
            StartCoroutine(StartGhostRumble(lowFrequency, highFrequency, time));
    }

    IEnumerator StartPlayerRumble(float lowFrequency, float highFrequency, float time)
    {
        Gamepad.all[Convert.ToInt32(_flip)].SetMotorSpeeds(lowFrequency,highFrequency);
        yield return new WaitForSeconds(time);
        Gamepad.all[Convert.ToInt32(_flip)].ResetHaptics();
    }
    
    IEnumerator StartGhostRumble(float lowFrequency, float highFrequency, float time)
    {
        Gamepad.all[Convert.ToInt32(!_flip)].SetMotorSpeeds(lowFrequency,highFrequency);
        yield return new WaitForSeconds(time);
        Gamepad.all[Convert.ToInt32(!_flip)].ResetHaptics();
    }

    IEnumerator StartBothRumble()
    {
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < Gamepad.all.Count; j++)
            {
                Gamepad.all[j].SetMotorSpeeds(1f,1f);
            }
            yield return new WaitForSeconds(rumbleTime);

            foreach (var gamepad in Gamepad.all)
            {
                gamepad.ResetHaptics();
            }

            yield return new WaitForSeconds(timeBetweenIterations);
        }
    }
}
