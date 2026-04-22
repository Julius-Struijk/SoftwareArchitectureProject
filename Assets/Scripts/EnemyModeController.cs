using System;
using UnityEngine;

public enum EnemyMode
{
    NORMAL = 0,
    ALERT
}

/// <summary>
/// Controls a global mode for all enemies, when the player enters/stays in
/// any enemy's collider trigger mode is switched to ALERT, and after alertDuration
/// the mode is switched back to NORMAL.
/// </summary>
public class EnemyModeController : MonoBehaviour
{
    public static float alertTime = 0f;
    [SerializeField]
    private float alertDuration = 60f;
    private static EnemyMode _enemyMode;
    public static EnemyMode enemyMode
    {
        set
        {
            //Reset the timer if Enemymode is alert
            if (value == EnemyMode.ALERT)
                alertTime = 0f;
            if (value != _enemyMode)
            {
                _enemyMode = value;
                onEnemyModeChanged?.Invoke(value);
            }
        }
        get => _enemyMode;
    }

    public static Action<EnemyMode> onEnemyModeChanged;


    private void Update()
    {
        if (enemyMode == EnemyMode.ALERT)
        {
            if (alertTime < alertDuration)
            {
                alertTime += Time.deltaTime;
                if (alertTime > alertDuration)
                {
                    enemyMode = EnemyMode.NORMAL;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyMode = EnemyMode.ALERT;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemyMode = EnemyMode.ALERT;
        }
    }
}
