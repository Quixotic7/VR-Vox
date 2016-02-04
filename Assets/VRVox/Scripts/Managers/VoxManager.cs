using UnityEngine;
using System.Collections;
using VRVox;

public class VoxManager : MonoBehaviour {

	public static Transform View { get; private set; }

	public static Vector3 ViewPosition
	{
		get
		{
			if (!View) return Vector3.zero;
			return View.position;
		}
	}

	public static Vector3 ViewForward
	{
		get
		{
			if(!View) return Vector3.forward;
			return View.forward;
		}
	}

	public static Vector3 ViewRight
	{
		get
		{
			if (!View) return Vector3.right;
			return View.right;
		}
	}

	public static Vector3 ViewUp
	{
		get
		{
			if (!View) return Vector3.up;
			return View.up;
		}
	}

	public static Ray ViewRay
	{
		get
		{
			if (!View) return new Ray();
			return new Ray(ViewPosition, ViewForward);
		}
	}

	public static Transform Player
	{
		get
		{
			return instance.UseRift ? instance.ovrGO.transform : instance.nonRiftGO.transform;
		}
	}

	public static bool InitialBuildFinished
	{
		get { return instance.voxWorld.InitialBuildFinished; }
	}

	public Transform ovrCenterTransform;
	public Transform mainCameraTransform;
	public VoxPlayerController playerController;

	public float InitialPlayerHeight = 5;

	public GameObject nonRiftGO;
	public GameObject ovrGO;
	public World voxWorld;
	public VoxMessageQueue messageQueue;
	public bool UseRift;
	public bool LockCursor;

	public VoxMenuButton toggleMusicButton;
	public VoxMenuButton nextTrackButton;

	public AudioSource backgroundMusic;

	public AudioClip[] songs;
	public string[] songNames;

	public float messageQueueDistance = 1.5f;

	protected int songIndex = 0;

	protected static VoxManager instance;

	public static void ShowMessage(string message)
	{
		instance.messageQueue.DisplayMessge(message);
		instance.messageQueue.transform.position = GetPointInView(instance.messageQueueDistance);
		instance.messageQueue.transform.forward = ViewForward;
	}

	public static Vector3 GetPointInView(float distanceFromView)
	{
		if (!View) return Vector3.zero;
		return View.position + View.forward * distanceFromView;
	}

	protected void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
		}

		if (UseRift)
		{
			View = ovrCenterTransform;
			ovrGO.SetActive(true);
		}
		else
		{
			View = mainCameraTransform;
			nonRiftGO.SetActive(true);
		}

		if (LockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		toggleMusicButton.SubscribeToButton(OnToggleMusicButton);
		nextTrackButton.SubscribeToButton(OnNextSong);
	}

	protected void OnToggleMusicButton(VoxMenuButton button, bool state)
	{
		if (!state) return;

		if (button.isSelected)
		{
			backgroundMusic.enabled = true;
			if(!backgroundMusic.isPlaying) backgroundMusic.Play();
		}
		else
		{
			backgroundMusic.Stop();
			backgroundMusic.enabled = false;
		}
	}

	protected void DoResetPlayer()
	{
		playerController.Reset();

		var newRot = ovrGO.transform.rotation.eulerAngles;
		newRot.y = 45;
		ovrGO.transform.rotation = Quaternion.Euler(newRot);

		//Player.position = new Vector3(0, -100, 0);
	}

	public void DoTeleportPlayer(Vector3 newPos)
	{
		playerController.Teleport(newPos);
	}


	public static void TeleportPlayer(Vector3 newPos)
	{
		instance.DoTeleportPlayer(newPos);
	}

	public static void ResetPlayer()
	{
		instance.DoResetPlayer();
	}

    protected bool songsMissing;

	// Use this for initialization
	void Start ()
	{
		backgroundMusic.loop = false;
		backgroundMusic.clip = songs[songIndex];
		backgroundMusic.Play();

		if (songIndex < songNames.Length)
		{
			//ShowMessage(songNames[songIndex]);

			toggleMusicButton.description = songNames[songIndex];
		}

        // Check if songs are missing, don't play music if they are
        // I removed the songs I used, but left in the song names if you want to go download them. 
        // "Groove through time and space" by Marco Koeller www.cyan-music.com
        // "Lylac Disport" by Wolf Asylum
        // "Natural Life Essence" by Juan Pablo www.cyan-music.com
        // "Love Sprouts" by Podington Bear
        foreach (var song in songs)
	    {
	        if (song == null)
	        {
	            songsMissing = true;
	            break;
	        }
	    }

	}

	protected void OnNextSong(VoxMenuButton button, bool state)
	{
		if (!state) return;

		NextSong();
	}

	public void NextSong()
	{
		songIndex++;
		if (songIndex >= songs.Length) songIndex = 0;

		backgroundMusic.clip = songs[songIndex];

		backgroundMusic.Play();

		if (songIndex < songNames.Length)
		{
			ShowMessage(songNames[songIndex]);

			toggleMusicButton.description = songNames[songIndex];
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Player.transform.position.y < -80) DoResetPlayer();


		if (backgroundMusic.enabled && !songsMissing)
		{
			if (!backgroundMusic.isPlaying)
			{
				NextSong();
			}
		}
	}
}
