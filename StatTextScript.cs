using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatTextScript : MonoBehaviour {

	public enum Stat {
		LegacyLowestLayer,
		LegacyDeaths,
		LegacyShotsFired,
		LegacyMaxTime,
		MaxTime,
        Deaths,
        ShotsFired,
        EnemiesKilled,
        MoneyBag,
	}
	public Stat s;
	string t = "";
	TimeSpan timeSpan;
	
	//On every frame.
	void Update () {
        switch (s) {
            case Stat.LegacyLowestLayer:
                t = "Lowest Layer Reached: " + Game.lowestLayer.ToString();
                break;
            case Stat.LegacyDeaths:
                t = "Deaths: " + Game.deaths.ToString();
                break;
            case Stat.LegacyShotsFired:
                t = "Shots Fired: " + Game.shots.ToString();
                break;
            case Stat.LegacyMaxTime:
                t = "Maximum Time Alive: " + Game.highMaxTimeAliveLegacy.ToString() + " seconds";
                break;
            case Stat.MaxTime:
                timeSpan = TimeSpan.FromSeconds(Game.highMaxTimeAlive);
                t = string.Format("Maximum Time Alive: [{0:D1}:{1:D2}]", timeSpan.Minutes, timeSpan.Seconds);
                break;
            case Stat.Deaths:
                t = "Deaths: " + Game.newDeaths.ToString();
                break;
            case Stat.ShotsFired:
                t = "Shots Fired: " + Game.newShotCount.ToString();
                break;
            case Stat.EnemiesKilled:
                t = "Enemies Killed: " + Game.enemiesKilled.ToString();
                break;
            case Stat.MoneyBag:
                t = "Most Money Bags Collected: " + Game.maxMoneyBagsCollected.ToString();
                break;
        }
		GetComponent<TextMesh>().text = t;
	}
}