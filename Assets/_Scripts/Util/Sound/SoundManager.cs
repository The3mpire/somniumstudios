using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    public AudioSource fxSource;                   //Drag a reference to the audio source which will play the sound effects.
    public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.

    public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.             
    public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.f
    public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.

    [SerializeField]
    private AudioClip menuSong;

	private float musicVol;


    void Awake() {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);


        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

    }
    
    void Start() {
       
        Cursor.visible = true;

        instance.musicSource.clip = menuSong;
        instance.musicSource.Play();
    }

	/// <summary>
	/// Fades the music out.
	/// </summary>
	public void FadeOutMusic()
	{
		musicVol = musicSource.volume;
		StartCoroutine(FadeMusicOutCo());
	}

	/// <summary>
	/// Fades the music in to the previous volume.
	/// </summary>
	public void FadeInMusic(){
		StartCoroutine(FadeMusicInCo());
	}

	/// <summary>
	/// Fades the music in. (Coroutine)
	/// </summary>
	/// <returns>The music in co.</returns>
	private IEnumerator FadeMusicInCo(){
		Debug.Log ("Fading in");
		while (musicSource.volume < musicVol) {
			musicSource.volume = Mathf.Lerp(musicSource.volume, musicVol, Time.deltaTime);
			yield return musicVol;
		}
		musicSource.volume = musicVol;
	}

	/// <summary>
	/// Fades the music out. (Coroutine)
	/// </summary>
	/// <returns>The music out co.</returns>
	private IEnumerator FadeMusicOutCo()
	{
		Debug.Log ("Fading out");

		while(musicSource.volume > .05F)
		{
			musicSource.volume = Mathf.Lerp(musicSource.volume, 0F, Time.deltaTime);
			yield return 0;
		}
		musicSource.volume = 0;
		//perfect opportunity to insert an on complete hook here before the coroutine exits.
	}

    /// <summary>
    /// Check if we loaded the mainMenu
    /// </summary>
    void OnLevelWasLoaded() {
        // make sure we play the menu song when the player goes back to the menu
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            if (instance.musicSource.clip != menuSong) {
                instance.musicSource.clip = menuSong;
                instance.musicSource.Play();
            }
        }
    }

    /// <summary>
    /// The correct song for the scene
    /// </summary>
    /// <param name="song"></param>
    public void SceneMusic(AudioClip song) {
        //check to make sure we're not already playing that song
        if(instance.musicSource != null || !instance.musicSource.clip.Equals(song)){
            instance.musicSource.clip = song;
            instance.musicSource.Play();
        }
    }
    

    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip, float volume = 1f) {
		fxSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Sets the volume of sfx
    /// </summary>
    /// <param name="vol"></param>
	public void SetSFXVolume(float vol = 1f){
		fxSource.volume = vol;
	}

    /// <summary>
    /// Gets the current volume of the music
    /// </summary>
    /// <returns></returns>
    public float GetSFXVolume() {
        return fxSource.volume;
    }

    /// <summary>
    /// Sets the volume of the music
    /// </summary>
    /// <param name="vol"></param>
	public void SetMusicVolume(float vol = .8f){
		musicSource.volume = vol;
	}

    /// <summary>
    /// Gets the current volume of the music
    /// </summary>
    /// <returns></returns>
    public float GetMusicVolume() {
        return musicSource.volume;
    }

    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips) {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        fxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        fxSource.clip = clips[randomIndex];

        //Play the clip.
        fxSource.Play();
    }
}
