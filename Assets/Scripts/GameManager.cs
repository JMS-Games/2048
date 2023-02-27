using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private int score;
    void Awake()
    {
        instance = this;
        score = 2;
    }

    public void SetScore(int newScore)
    {
        score = newScore;
    }

    public int GetScore()
    {
        return score;
    }
}
