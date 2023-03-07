using System;
using Unity.Mathematics;
using UnityEngine;

public class FinishLineIndicator : MonoBehaviour
{
    [SerializeField] private GameObject player1IndicatorPrefab;
    [SerializeField] private GameObject player2IndicatorPrefab;
    
    private GameObject _p1GameObject;
    private GameObject _p2GameObject;
    private float _player1IndicatorPos;
    private float _player2IndicatorPos;
    private ScoreTracker _scoreTracker;
    private Transform _player;

    private void OnEnable()
    {
        PlayerManager.OnSwap += CalculateIndicatorPos;
    }

    private void OnDisable()
    {
        PlayerManager.OnSwap -= CalculateIndicatorPos;
    }

    private void Start()
    {
        _player = FindObjectOfType<PlayerController2D>().transform;
        _scoreTracker = FindObjectOfType<ScoreTracker>();
        _p1GameObject = Instantiate(player1IndicatorPrefab, new Vector3(500, 0, 0), quaternion.identity);
        _p2GameObject = Instantiate(player2IndicatorPrefab, new Vector3(500, 0, 0), quaternion.identity);
        _p2GameObject.SetActive(false);
    }

    private void CalculateIndicatorPos(bool flip)
    {
        if (!flip)
        {
            _p1GameObject.SetActive(true);
            _player1IndicatorPos = 500 +  _player.position.x - _scoreTracker.Player1Score;
            _p1GameObject.transform.position = new Vector3(_player1IndicatorPos, 0, 0);
            _p2GameObject.SetActive(false);
        }
        else
        {
            _p2GameObject.SetActive(true);
            _player2IndicatorPos = 500 +  _player.position.x - _scoreTracker.Player2Score;
            _p2GameObject.transform.position = new Vector3(_player2IndicatorPos, 0, 0);
            _p1GameObject.SetActive(false);
        }
    }
}
