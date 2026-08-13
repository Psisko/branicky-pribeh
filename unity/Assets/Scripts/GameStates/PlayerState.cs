using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerState{
    public UnityEvent<int, int> livesChangeEvent = new();
    public UnityEvent<int> moneyChangeEvent = new();
    public UnityEvent<int> healthpacksChangeEvent = new();
    public UnityEvent<int> damagePotionsChangeEvent = new();
    public UnityEvent<int> damagePotionstimerChangeEvent = new();
    public UnityEvent<int> speedPotionsChangeEvent = new();
    public UnityEvent<int> speedPotionstimerChangeEvent = new();

    private int maxLives = 10;
    private int lives = 10; 
    private int money = 0;
    private int healthPotions = 0;
    private int damagePotions = 0;
    private int speedPotions = 0;

    private float levelStartingTime;
    private int levelStartingmoney;
    private int levelStartingEnemiesDefeated;

    /// <summary>
    /// Sets starting parameters on the start of a level for EndingStats
    /// </summary>
    public void SetLevelStartingParameters()
    {
        levelStartingTime = Time.time;
        levelStartingmoney = money;
        levelStartingEnemiesDefeated = 1;
    }

    public void AddDefeatedEnemies() { levelStartingEnemiesDefeated++;}

    public float GetFinalEnemies() { return levelStartingEnemiesDefeated; }
    public float GetFinalTime() { return Time.time - levelStartingTime; }
    public int GetFinalMoney() { return money - levelStartingmoney; }

    public int GetMaxLives() { return maxLives; }

    public (int, int, int, int, int) GetPlayerResources() 
    {
        return (lives, money, healthPotions, damagePotions, speedPotions);
    }

    public int GetLives() { return lives; }

    public int GetMoney() { return money;}

    public int GetHealthpacks() {  return healthPotions; }
    public int GetDamagePotions() { return damagePotions; }

    public int GetSpeedPotions() { return speedPotions; }

    public bool ChangeMoney(int amount) {
        if (money + amount < 0) {
            return false;
        }

        this.money += amount;
        moneyChangeEvent.Invoke(money);
        return true;
    }

    public bool ChangeHealthPotions(int amount) {  
        if (healthPotions + amount < 0) {
            return false;
        }

        this.healthPotions += amount;
        healthpacksChangeEvent.Invoke(healthPotions);
        return true;
    }

    public bool ChangeDamagePotions(int amount)
    {
        if (damagePotions + amount < 0)
        {
            return false;
        }

        this.damagePotions += amount;
        damagePotionsChangeEvent.Invoke(damagePotions);
        return true;
    }

    public bool ChangeSpeedPotions(int amount)
    {
        if (speedPotions + amount < 0)
        {
            return false;
        }

        this.speedPotions += amount;
        speedPotionsChangeEvent.Invoke(speedPotions);
        return true;
    }

    public void ChangeLives(int amount) {
        lives += amount;
        if (lives < 0) {
            lives = 0;
        }
        if (lives > maxLives) {
            lives = maxLives;
        }
        livesChangeEvent.Invoke(lives, maxLives);
    }

    public void AddDamagePotion()
    {
        damagePotions++;
        damagePotionsChangeEvent.Invoke(damagePotions);
    }

    public void AddSpeedPotion()
    {
        speedPotions++;
        speedPotionsChangeEvent.Invoke(speedPotions);
    }

    public void UseHealthpack()
    {
        healthPotions--;
        if (healthPotions < 0)
        {
            healthPotions = 0;
        }
        // heal o 3 životy
        ChangeLives(3);

        healthpacksChangeEvent.Invoke(healthPotions);
    }

    public void UseDamagePotion()
    {
        damagePotions--;
        if (damagePotions < 0)
        {
            damagePotions = 0;
        }

        damagePotionsChangeEvent.Invoke(damagePotions);
        damagePotionstimerChangeEvent.Invoke(damagePotions);
    }

    public void UseSpeedPotion()
    {
        speedPotions--;
        if (speedPotions < 0)
        {
            speedPotions = 0;
        }

        speedPotionsChangeEvent.Invoke(speedPotions);
        speedPotionstimerChangeEvent.Invoke(speedPotions);

    }

    public void LoadPlayerResources(int new_lives, int new_money, int new_healthPotions, int new_damagePotions, int new_speedPotions)
    {
        lives = new_lives;
        money = new_money;
        healthPotions = new_healthPotions;
        damagePotions = new_damagePotions;
        speedPotions = new_speedPotions;


        livesChangeEvent.Invoke(lives, maxLives);
        moneyChangeEvent.Invoke(money);
        healthpacksChangeEvent.Invoke(healthPotions);
        damagePotionsChangeEvent.Invoke(damagePotions);
        speedPotionsChangeEvent.Invoke(speedPotions);

    }

    public void SetLives(int lives) {
        this.lives = lives;
        livesChangeEvent.Invoke(lives, maxLives);
    }

    public void SetMoney(int money) { 
        this.money = money;
        moneyChangeEvent.Invoke(money);
    }

}
