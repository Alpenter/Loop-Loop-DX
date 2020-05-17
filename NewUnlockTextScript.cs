using UnityEngine;

public class NewUnlockTextScript : MonoBehaviour {

    public enum Unlocked {
        BulletHell,
        Legacy,
        LegacyBulletHell,
        Shop,
    }
    public Unlocked whatMe = Unlocked.BulletHell;
    bool active = false;

    private void Update() {
        switch (whatMe) {
            case Unlocked.BulletHell:
                active = (Game.unlockedBulletHell && !Game.beenToBulletHell);
                break;
            case Unlocked.Legacy:
                active = (Game.unlockedLegacy && !Game.beenToLegacy);
                break;
            case Unlocked.LegacyBulletHell:
                active = (Game.unlockedLegacyBulletHell && !Game.beenToLegacyBulletHell);
                break;
            case Unlocked.Shop:
                active = (Game.unlockedShop && !Game.beenToShop);
                break;
        }

        if (!active) {
            gameObject.GetComponent<TextMesh>().text = "";
        } else {
            gameObject.GetComponent<TextMesh>().text = "< New!";
        }
    }
}
