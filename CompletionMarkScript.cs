using UnityEngine;

public class CompletionMarkScript : MonoBehaviour {

    public enum CompletionType {
        None,
        Adventure, 
        BulletHell,
        Legacy,
        LegacyBulletHell,
        ShipShop,
    }
    public CompletionType toComplete = CompletionType.None;
    bool show = false;
    GameObject puppet = null;

    private void Awake() {
        puppet = transform.Find("puppet").gameObject;
    }

    private void Update() {
        
        switch (toComplete) {
            case CompletionType.None:
                show = false;
                break;
            case CompletionType.Adventure:
                show = Game.beatFinalBoss;
                break;
            case CompletionType.BulletHell:
                show = (Game.unlockedAchievements[30] && Game.unlockedAchievements[48] && Game.unlockedBulletHell);
                break;
            case CompletionType.Legacy:
                show = (Game.lowestLayer <= 0 && Game.unlockedAchievements[41] && Game.unlockedLegacy);
                break;
            case CompletionType.LegacyBulletHell:
                show = (Game.highMaxTimeAliveLegacy >= 60f);
                break;
            case CompletionType.ShipShop:
                show = (Game.unlockedAchievements[25] && Game.unlockedShop);
                break;
        }

        puppet.SetActive(show);
    }

}
