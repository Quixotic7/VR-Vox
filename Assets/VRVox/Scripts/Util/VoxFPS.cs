using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class VoxFPS : MonoBehaviour {

	public float FramesPerSecond { get { return _guiFPS; } }

	public float FPS;

	private const float UpdateInterval = 0.5f;
	private int _guiFrames = 0;
	private float _guiFPS = 0;

	private int _fixedFrames = 0;
	private float _fixedFPS = 0;

	private Stopwatch _guiWatch = new Stopwatch();
	private Stopwatch _fixedWatch = new Stopwatch();


	// Use this for initialization
	void OnEnable() {

		_guiWatch.Start();
		_fixedWatch.Start();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateFPS();

		FPS = _guiFPS;
	}

	void UpdateFPS()
	{
		var ts = _guiWatch.Elapsed;
		var totalSeconds = (float)ts.TotalSeconds;

		++_guiFrames;

		// Interval ended - update GUI text and start new interval
		if (totalSeconds >= UpdateInterval)
		{
			_guiWatch.Reset();
			_guiWatch.Start();
			_guiFPS = 1.0f / (totalSeconds / _guiFrames);
			_guiFrames = 0;
		}
	}
}
