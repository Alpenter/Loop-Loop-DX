using UnityEngine;

public static class PlaySound {

    public static void NoLoop(AudioClip snd){
        GameObject ao = new GameObject();
        ao.name = "audio";
        AudioSource a = ao.AddComponent<AudioSource>();
        if (Game.tootMode) {
            a.volume = Game.soundVolume/1.5f;
            a.clip = FartSound();
        } else { 
            a.volume = Game.soundVolume;
            a.clip = snd; 
        }
        a.spatialBlend = 0f;
        a.loop = false;
        a.Play();
        Object.Destroy(ao, a.clip.length);
    }

    public static void NoLoopRandomPitch(AudioClip snd, float min, float max) {
        GameObject ao = new GameObject();
        ao.name = "audio";
        AudioSource a = ao.AddComponent<AudioSource>();
        if (Game.tootMode) {
            a.clip = FartSound();
            a.volume = Game.soundVolume/1.5f;
        } else { 
            a.clip = snd;
            a.volume = Game.soundVolume;
        }
        a.loop = false;
        a.spatialBlend = 0f;
        a.pitch = Random.Range(min, max);
        a.Play();
        Object.Destroy(ao, a.clip.length);
    }
	
	public static void Damage(){
		GameObject ao = new GameObject();
        ao.name = "audio";
		AudioClip[] pool = {
			Resources.Load("SFX/Monster Hit 0") as AudioClip,
			Resources.Load("SFX/Monster Hit 1") as AudioClip,
			Resources.Load("SFX/Monster Hit 2") as AudioClip,
			Resources.Load("SFX/Monster Hit 3") as AudioClip,
			Resources.Load("SFX/Monster Hit 4") as AudioClip,
			Resources.Load("SFX/Monster Hit 5") as AudioClip,
			Resources.Load("SFX/Monster Hit 6") as AudioClip,
			Resources.Load("SFX/Monster Hit 7") as AudioClip,
			Resources.Load("SFX/Monster Hit 8") as AudioClip,
			Resources.Load("SFX/Monster Hit 9") as AudioClip,
		};
		AudioSource a = ao.AddComponent<AudioSource>();
        if (Game.tootMode) {
            a.clip = FartSound();
            a.volume = Game.soundVolume/1.5f;
        } else {
            a.clip = pool[Random.Range(0, pool.Length)];
            a.volume = Game.soundVolume;
        }
		a.loop = false;
        a.spatialBlend = 0f;
        a.Play();
        Object.Destroy(ao, a.clip.length);
	}

    public static AudioClip FartSound() {
        int i = Random.Range(0, 6);
        AudioClip a = Resources.Load("SFX/fart"+i.ToString()) as AudioClip;
        return a;
    }
}
